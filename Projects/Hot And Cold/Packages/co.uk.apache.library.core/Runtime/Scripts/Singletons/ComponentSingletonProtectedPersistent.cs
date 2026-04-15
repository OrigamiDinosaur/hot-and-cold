namespace Apache.Core {

	/// <inheritdoc />
	/// <remarks>With <c>ComponentSingletonProtectedPersistent</c>, the game object is persisted across scenes.</remarks>
	public abstract class ComponentSingletonProtectedPersistent<T> : ComponentSingletonProtected<T> where T : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Protected Properties:
		//-----------------------------------------------------------------------------------------

		protected override bool ShouldNotDestroyOnLoad => true;
	}
}