namespace Apache.Core {

	/// <inheritdoc />
	/// <remarks>With <c>ComponentSingleton</c>, its <c>Instance</c> is publicly accessible.</remarks>
	public abstract class ComponentSingleton<T> : ComponentSingletonProtected<T> where T : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public new static T Instance => ComponentSingletonProtected<T>.Instance;
	}
}