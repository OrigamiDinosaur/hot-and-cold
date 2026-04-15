namespace Apache.Core {

	/// <inheritdoc />
	/// <remarks>With <c>Singleton</c>, its <c>Instance</c> is publicly accessible.</remarks>
	public abstract class Singleton<T> : SingletonProtected<T> where T : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public new static T Instance => SingletonProtected<T>.Instance;

		/// <summary>Determine whether the singleton has an instance without dynamically reinstantiating it.</summary>
		public new static bool HasInstance => SingletonProtected<T>.HasInstance;
	}
}