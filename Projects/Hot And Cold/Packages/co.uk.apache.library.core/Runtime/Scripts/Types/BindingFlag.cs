using System.Reflection;

namespace Apache.Core {
	public static class BindingFlag {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Search criteria encompassing all public and non-public members, including base members.
		/// Note that you also need to specify either the Instance or Static flag.
		/// </summary>
		public const BindingFlags ANY_VISIBILITY = BindingFlags.Public | BindingFlags.NonPublic;

		/// <summary>
		/// Search criteria encompassing all public instance members, including base members.
		/// </summary>
		public const BindingFlags INSTANCE_PUBLIC = BindingFlags.Public | BindingFlags.Instance;

		/// <summary>
		/// Search criteria encompassing all non-public instance members, including base members.
		/// </summary>
		public const BindingFlags INSTANCE_NON_PUBLIC = BindingFlags.NonPublic | BindingFlags.Instance;

		/// <summary>
		/// Search criteria encompassing all public and non-public instance members, including base members.
		/// </summary>
		public const BindingFlags INSTANCE_ANY_VISIBILITY = ANY_VISIBILITY | BindingFlags.Instance;

		/// <summary>
		/// Search criteria encompassing all public static members, including base members.
		/// </summary>
		public const BindingFlags STATIC_PUBLIC = BindingFlags.Public | BindingFlags.Static;

		/// <summary>
		/// Search criteria encompassing all non-public static members, including base members.
		/// </summary>
		public const BindingFlags STATIC_NON_PUBLIC = BindingFlags.NonPublic | BindingFlags.Static;

		/// <summary>
		/// Search criteria encompassing all public and non-public static members, including base members.
		/// </summary>
		public const BindingFlags STATIC_ANY_VISIBILITY = ANY_VISIBILITY | BindingFlags.Static;

		/// <summary>
		/// Search criteria encompassing all public instance members, excluding base members.
		/// </summary>
		public const BindingFlags INSTANCE_PUBLIC_DECLARED_ONLY = INSTANCE_PUBLIC | BindingFlags.DeclaredOnly;

		/// <summary>
		/// Search criteria encompassing all non-public instance members, excluding base members.
		/// </summary>
		public const BindingFlags INSTANCE_NON_PUBLIC_DECLARED_ONLY = INSTANCE_NON_PUBLIC | BindingFlags.DeclaredOnly;

		/// <summary>
		/// Search criteria encompassing all public and non-public instance members, excluding base members.
		/// </summary>
		public const BindingFlags INSTANCE_ANY_DECLARED_ONLY = INSTANCE_ANY_VISIBILITY | BindingFlags.DeclaredOnly;

		/// <summary>
		/// Search criteria encompassing all public static members, excluding base members.
		/// </summary>
		public const BindingFlags STATIC_PUBLIC_DECLARED_ONLY = STATIC_PUBLIC | BindingFlags.DeclaredOnly;

		/// <summary>
		/// Search criteria encompassing all non-public static members, excluding base members.
		/// </summary>
		public const BindingFlags STATIC_NON_PUBLIC_DECLARED_ONLY = STATIC_NON_PUBLIC | BindingFlags.DeclaredOnly;

		/// <summary>
		/// Search criteria encompassing all public and non-public static members, excluding base members.
		/// </summary>
		public const BindingFlags STATIC_ANY_DECLARED_ONLY = STATIC_ANY_VISIBILITY | BindingFlags.DeclaredOnly;

		/// <summary>
		/// Search criteria encompassing all members, including base and static members.
		/// </summary>
		public const BindingFlags STATIC_INSTANCE_ANY_VISIBILITY = INSTANCE_ANY_VISIBILITY | BindingFlags.Static;

		/// <summary>
		/// Search criteria encompassing all members (public and non-public, instance and static), including base members.
		/// </summary>
		public const BindingFlags ALL_MEMBERS = STATIC_INSTANCE_ANY_VISIBILITY | BindingFlags.FlattenHierarchy;
	}
}