using System.Collections.Generic;

public static class CollectionExtensions {
	
	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------
	
	/// <summary>Returns <c>true</c> if the list is either null or empty, otherwise <c>false</c>.</summary>
	public static bool IsNullOrEmpty<T>(this IList<T> self) {
		return (self == null || self.Count == 0);
	}
}