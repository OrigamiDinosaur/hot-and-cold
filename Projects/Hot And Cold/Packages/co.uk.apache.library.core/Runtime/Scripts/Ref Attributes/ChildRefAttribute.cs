
namespace Apache.Core {
	public class ChildRefAttribute : GetComponentInRefAttribute {

		//-----------------------------------------------------------------------------------------
		// Protected Properties:
		//-----------------------------------------------------------------------------------------

		protected override Scopes Scope => Scopes.Child;
	}
}