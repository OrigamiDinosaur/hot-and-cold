namespace Apache.Core {

	/// <summary>
	/// An object in a super scene which should be disabled when a high-render-priority subscene loads. Examples of such objects might be
	/// lights and main cameras, which we want to keep around if the subscene is not loaded, but definitely want disabled if it is.
	/// </summary>
	public class SuperSceneLowRenderPriorityObject : ApacheComponent { }
}