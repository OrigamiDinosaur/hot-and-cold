using System;
using Apache.Core.Extensions;
using UnityEngine;

namespace Apache.Core {

	/// <summary>
	/// Handles tinting the screen with a colour, or fading between tint colours, using a separate, high-depth-order camera which renders nothing
	/// other than this tint and stays disabled when not in use.
	/// </summary>
	/// <inheritdoc />
	[RequireComponent(typeof(Camera))]
	public class ScreenTintController : ComponentSingleton<ScreenTintController> {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------
		
		private const string TINT_PROPERTY_NAME = "_Tint";
		private static readonly int TINT_PROPERTY = Shader.PropertyToID(TINT_PROPERTY_NAME);

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[ComponentRef, SerializeField] protected Camera tintCamera;
		[SerializeField] protected Shader shader;

		//-----------------------------------------------------------------------------------------
		// Backing Fields:
		//-----------------------------------------------------------------------------------------

		private Material _material;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private Color fadeStartColour;
		private Color fadeEndColour;

		private bool isMostRecentFadeHighPriority;

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public Color Tint { get; private set; }

		//-----------------------------------------------------------------------------------------
		// Private Properties:
		//-----------------------------------------------------------------------------------------

		private bool IsEnabled => enabled;
		
		private Material Material {
			get {

				// if we have an existing material return it.
				if (_material != null) return _material;

				// N.B. here we create the material for the first time. We're lazy loading as, depending on the scene, this class may not be used,
				// resulting in unnecessary work if done in Awake. If this class *is* used, it will most likely be used in Start to do a scene fade,
				// so the work is happening close to initialisation time anyway.

				// throw error if shader is missing.
				if (shader == null) {
					Debug.LogError("Screen tint shader not supplied to " + nameof(ScreenTintController) + ".", gameObject);
				}

				// create the material from the given shader, throwing an error if it cannot be created.
				_material = new Material(shader);
				if (_material == null) {
					Debug.LogError("Screen tint material cannot be created.", gameObject);
				}

				// throw an error if the given shader does not have a tint property.
				if (!_material.HasProperty(TINT_PROPERTY)) {
					Debug.LogError("Screen tint shader does not have \"" + TINT_PROPERTY + "\" property.", gameObject);
				}

				return _material;
			}
		}
		
		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void Awake() {
			base.Awake();

			// ensure we start disabled so we're not constantly issuing a draw call.
			EnableOrDisable(false);
		}

		protected void OnRenderImage(RenderTexture source, RenderTexture destination) {
			
			// perform a blit with our material, which handles drawing the tint.
			Graphics.Blit(source, destination, Material);
			
			// if we make it here and the sequence is no longer running while nothing is visibly being rendered to the screen, disable.
			if (!sequence.IsRunning && Tint.a == 0) {
				EnableOrDisable(false);
			}
		}
		
		protected new void OnDestroy() {
			base.OnDestroy();

			// destroy the material (if any).
			Destroy(_material);
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Instantly set the screen to a given tint colour.
		/// </summary>
		/// <param name="colour">The tint colour.</param>
		public void SetTint(Color colour) {

			// set field then update material.
			Tint = colour;
			Material.SetColor(TINT_PROPERTY, Tint);
			
			// if the colour is not invisible and we're disabled, enable.
			if (colour.a > 0 && !IsEnabled) {
				EnableOrDisable(true);
			}
		}

		/// <summary>
		/// Fade the screen from its existing tint colour to a new tint colour over time and with an ease.
		/// </summary>
		/// <param name="colour">The colour to fade to.</param>
		/// <param name="duration">The duration of the fade operation.</param>
		/// <param name="ease">The ease of the fade.</param>
		/// <param name="onComplete">An optional action to invoke on complete.</param>
		/// <param name="isHighPriority">Whether this fade operation is high priority meaning it cannot be interrupted by other non-high priority fades.</param>
		public void Fade(Color colour, float duration, LeanTweenType ease = LeanTweenType.linear, Action onComplete = null, bool isHighPriority = false) {

			// if we have an existing fade and it's high priority, and this one isn't, invoke on complete and back out.
			if (sequence.IsRunning && isMostRecentFadeHighPriority && !isHighPriority) {
				onComplete?.Invoke();
				return;
			}
			
			// ensure we cancel any existing fade and ensure we're enabled.
			sequence.Cancel();
			EnableOrDisable(true);

			// flag whether this is high priority.
			isMostRecentFadeHighPriority = isHighPriority;

			// if we're already invisible, tint with the target colour but with zero alpha.
			if (Tint.a == 0) {
				SetTint(colour.WithAlpha(0));
			}

			// keep track of our fade start and end colours.
			fadeStartColour = Tint;
			fadeEndColour = colour;
			
			// start tweening the colour via a t value.
			sequence.TweenRealtime(
				LeanTween.value(gameObject, SetTintByFadeStartAndEndColour, 0, 1, duration)
					.setEase(ease)
					.setOnComplete(onComplete)
			);
		}

		/// <summary>
		/// Flash the screen up by fading from its existing tint colour to a given colour and then fading back down to a given colour.
		/// </summary>
		/// <param name="flashUpColour">The colour to initially flash up to.</param>
		/// <param name="flashUpDuration">The duration of the first flash up fade operation.</param>
		/// <param name="flashDownColour">The colour to sequently flash down to, after the flash up.</param>
		/// <param name="flashDownDuration">The duration of the subseqent flash down operation, occurring immediately after flash up.</param>
		/// <param name="flashDownDelay">The delay before initiating the flash down, after flash up.</param>
		/// <param name="onFlashUp">An optional action to invoke when the first flash is completed and the screen is fully tinted with the <c>flashUpColour</c>.</param>
		/// <param name="onComplete">An optional action to invoke on complete, which is to say: after both flash up and subsequent flash down.</param>
		/// <param name="isHighPriority">Whether this flash operation is high priority meaning it cannot be interrupted by other non-high priority fades.</param>
		/// <param name="flashUpEase">The ease with which to perform the first flash up fade.</param>
		/// <param name="flashDownEase">The ease with which to perform the subsequent flash down fade, occurring immediately after flash up.</param>
		public void Flash(Color flashUpColour, float flashUpDuration, Color flashDownColour, float flashDownDuration, float flashDownDelay = 0,
								Action onFlashUp = null, Action onComplete = null, bool isHighPriority = false,
								LeanTweenType flashUpEase = LeanTweenType.linear, LeanTweenType flashDownEase = LeanTweenType.linear) {

			// if we have an existing fade and it's high priority, and this one isn't, invoke on completes and back out.
			if (sequence.IsRunning && isMostRecentFadeHighPriority && !isHighPriority) {
				onFlashUp?.Invoke();
				onComplete?.Invoke();
				return;
			}

			// first, fade to flash up colour...
			Fade(flashUpColour, flashUpDuration, flashUpEase,

				// ...then on complete...
				() => {
					
					// invoke the onFlashUp action if we have one.
					onFlashUp?.Invoke();

					// then perform the flash down with another fade...
					sequence.DoRealtime(flashDownDelay, () =>
						Fade(flashDownColour, flashDownDuration, flashDownEase,
							
							// and invoke the given onComplete action on complete.
							onComplete,

							isHighPriority));
				}, isHighPriority);
			}
		
		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private void EnableOrDisable(bool shouldEnableElseDisable) {

			// if the camera's culling mask is set to nothing, assume its sole purpose is to render the tint, so enable/disable the camera.
			// N.B. disabling when not in use also prevents a draw call constantly being issued throughout the lifetime of the application.
			if (tintCamera.cullingMask == 0) {
				tintCamera.enabled = shouldEnableElseDisable;
			}

			// enable/disable this component so we don't receive OnRenderImage calls (regardless of whether the camera is enabled or not).
			enabled = shouldEnableElseDisable;
		}

		private void SetTintByFadeStartAndEndColour(float t) {
			SetTint(Color.Lerp(fadeStartColour, fadeEndColour, t));
		}
	}
}