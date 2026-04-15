namespace Apache.Core {
	public class ParentRefAttribute : GetComponentInRefAttribute {

		//-----------------------------------------------------------------------------------------
		// Protected Properties:
		//-----------------------------------------------------------------------------------------

		protected override Scopes Scope => Scopes.Parent;
	}
}