using System.Collections.Generic;

//-----------------------------------------------------------------------------------------
// Type Definitions:
//-----------------------------------------------------------------------------------------

public enum Subscenes {
	Environment = Scenes.Main + 1
}

//-----------------------------------------------------------------------------------------
// Classes:
//-----------------------------------------------------------------------------------------

public static class SubscenesInfo {

	//-----------------------------------------------------------------------------------------
	// Constants:
	//-----------------------------------------------------------------------------------------

	public static readonly Dictionary<Subscenes, SubsceneInfo> INFOS = new Dictionary<Subscenes, SubsceneInfo> {
		{ Subscenes.Environment, new SubsceneInfo(true) }
	};

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public static bool IsSubscene(int i) {
		return INFOS.ContainsKey((Subscenes)i);
	}

	public static bool IsHighRenderPriority(Subscenes subscene) {
		return INFOS[subscene].IsHighRenderPriority;
	}

	public static bool ShouldLoadInEditor(Subscenes subscene) {
		return INFOS[subscene].ShouldLoadInEditor;
	}

	//-----------------------------------------------------------------------------------------
	// Classes:
	//-----------------------------------------------------------------------------------------

	public class SubsceneInfo {

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public bool IsHighRenderPriority { get; }
		public bool ShouldLoadInEditor   { get; }

		//-----------------------------------------------------------------------------------------
		// Constructors:
		//-----------------------------------------------------------------------------------------

		public SubsceneInfo(bool isHighRenderPriority = false, bool shouldLoadInEditor = true) {
			IsHighRenderPriority = isHighRenderPriority;
			ShouldLoadInEditor   = shouldLoadInEditor;
		}
	}
}