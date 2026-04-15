using UnityEngine;

namespace Apache.Core {
	public class TransformWidget {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const string PREFAB_RESOURCE_PATH = "Transform Widget";

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private GameObject _prefab;

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public GameObject Prefab {
			get {
				if (_prefab != null) return _prefab;
				_prefab = Resources.Load<GameObject>(PREFAB_RESOURCE_PATH);
				return _prefab;
			}
			set => _prefab = value;
		}

		//-----------------------------------------------------------------------------------------
		// Constructors:
		//-----------------------------------------------------------------------------------------

		private TransformWidget() : this(Vector3.zero, Quaternion.identity) { }

		private TransformWidget(Vector3 position) : this(position, Quaternion.identity) { }

		private TransformWidget(Vector3 position, Quaternion rotation) {

			// instantiate copy from prefab.
			GameObject gameObject = Object.Instantiate(Prefab);
			Transform transform = gameObject.transform;

			// set position and rotation.
			transform.position = position;
			transform.rotation = rotation;
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public static TransformWidget New(bool shouldDebugBreak = false) {
			if (shouldDebugBreak) {
				Debug.Break();
			}
			return new TransformWidget();
		}

		public static TransformWidget New(Vector3 position, bool shouldDebugBreak = false) {
			if (shouldDebugBreak) {
				Debug.Break();
			}
			return new TransformWidget(position);
		}

		public static TransformWidget New(Vector3 position, Quaternion rotation, bool shouldDebugBreak = false) {
			if (shouldDebugBreak) {
				Debug.Break();
			}
			return new TransformWidget(position, rotation);
		}
	}
}