namespace Apache.Core {
	public interface IPoolable {

		//-----------------------------------------------------------------------------------------
		// Interface Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// The poolable has been returned to the pool and is disabled.
		/// </summary>
		void OnReturnedToPool();

		/// <summary>
		/// The poolable object is being retrieved from the pool. At this point, it becomes enabled and usable again.
		/// </summary>
		void OnRetrievedFromPool();
	}
}