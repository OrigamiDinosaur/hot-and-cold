using UnityEngine;

namespace Apache.Core {
	public class ReflectionProbeRenderer : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const float DEFAULT_DELAY_ON_AWAKE_TO_RENDER_PROBE = 0.01f;

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField] protected ReflectionProbe reflectionProbe;
		[SerializeField] protected float delayOnAwakeToRenderProbe = DEFAULT_DELAY_ON_AWAKE_TO_RENDER_PROBE;

		//-----------------------------------------------------------------------------------------
		//	Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() {
			InvokeAction(delayOnAwakeToRenderProbe, () => {
				reflectionProbe.RenderProbe();
			});
		}
	}
}