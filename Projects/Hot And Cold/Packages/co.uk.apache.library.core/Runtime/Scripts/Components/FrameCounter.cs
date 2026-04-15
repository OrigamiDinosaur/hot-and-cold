using UnityEngine;

namespace Apache.Core {
	public class FrameCounter : SingletonProtected<FrameCounter> {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const string LOG_FORMAT = "[#{0}] {1}";
		private const string FRAME_COUNTER_LOG_TAG = "Frame Counter Log";

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public int Frame => Time.frameCount;
		
		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public static bool HasLogTag(Component component) {
			return component.CompareTag(FRAME_COUNTER_LOG_TAG);
		}

		public static void Log(object message) {
			Debug.Log(string.Format(LOG_FORMAT, Instance.Frame, message));
		}

		public static void Log(object message, Object context) {
			Debug.Log(string.Format(LOG_FORMAT, Instance.Frame, message), context);
		}

		public static string Message(object message) {
			return string.Format(LOG_FORMAT, Instance.Frame, message);
		}
	}
}