[System.Serializable]
public struct PlayerAttributes {

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public float DrillValue;
	public float MoveSpeed;

	public float GoldBonus;
	public float ScrapBonus;

	//-----------------------------------------------------------------------------------------
	// Constructors:
	//-----------------------------------------------------------------------------------------

	public PlayerAttributes(float drillValue, float moveSpeed, float goldBonus, float scrapBonus) {

		DrillValue = drillValue;
		MoveSpeed = moveSpeed;
		GoldBonus = goldBonus;
		ScrapBonus = scrapBonus;
	}

	//-----------------------------------------------------------------------------------------
	// Operator Overrides:
	//-----------------------------------------------------------------------------------------

	public static PlayerAttributes operator +(PlayerAttributes left, PlayerAttributes right) {
		return new PlayerAttributes(left.DrillValue + right.DrillValue, left.MoveSpeed + right.MoveSpeed, left.GoldBonus + right.GoldBonus, left.ScrapBonus + right.ScrapBonus);
	}
}