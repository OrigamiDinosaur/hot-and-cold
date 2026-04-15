using Apache.Core;
using UnityEngine;

/// <summary>Performs very high-level initialisation of the scene and sets up <c>PreEvents</c>.</summary>
/// <remarks>Beause of our execution order, we are guaranteed to run before all other components.</remarks>
public class SceneInit : ApacheComponent {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected Scenes scene;

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public Scenes Scene => scene;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void Awake() {
		AppController.SetNewlyActiveScene(scene);
		PreEvents.OnPreAwake();
	}

	protected void OnEnable() {
		PreEvents.OnPreOnEnable();
	}

	protected void Start() {
		PreEvents.OnPreStart();
	}

	protected void OnDisable() {
		PreEvents.OnPreOnDisable();
	}

	protected void OnDestroy() {
		PreEvents.OnPreOnDestroy();
	}
}