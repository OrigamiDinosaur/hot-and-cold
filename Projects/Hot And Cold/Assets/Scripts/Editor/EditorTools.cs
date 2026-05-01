using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class EditorTools {

	//-----------------------------------------------------------------------------------------
	// Constants:
	//-----------------------------------------------------------------------------------------

	public const string MENU_ITEM_LOCATION = "Ori";
	public const int MENU_ITEM_STARTING_PRIORITY = 0;
	
	public const string DIVIDER_GAME_OBJECT_NAME = "--------------------";

	public const string EDITOR_ONLY_TAG = "EditorOnly";

	// N.B. this regex includes a capture group on the name and the number, in case we want to use that for sorting.
	// N.B. the first group's contents is lazy so that it doesn't gobble up the first occurrence of '(', '[', or '{', or other whitespace.
	private static readonly Regex NAME_AND_NUMBER_SUFFIX_REGEX = new Regex(@"^(.*?)[\(\[\{\<]*\s*#?\s*([0-9]+)\s*[\)\]\}\>]*\s*$");

	// N.B. this regex includes a capture group only on the name.
	// N.B. the first group's contents is lazy so that it doesn't gobble up the first occurrence of '(', '[', or '{', or other whitespace.
	private static readonly Regex NAME_WITH_OPTIONAL_NUMBER_SUFFIX_REGEX = new Regex(@"^(.*?)[\(\[\{\<]*\s*#?\s*[0-9]*\s*[\)\]\}\>]*\s*$");

	private const string RENAME_BY_ORDER_NAME_SUFFIX_FORMAT = " ({0})";

	//-----------------------------------------------------------------------------------------
	// Editor Methods:
	//-----------------------------------------------------------------------------------------

	// create methods.
	
	[MenuItem(MENU_ITEM_LOCATION + "/Create Divider in Root %#d", false, MENU_ITEM_STARTING_PRIORITY)]
	public static void CreateDividerInRoot() {

		// create the game object and place it at the origin, then apply editor only tag.
		GameObject gameObject = new GameObject { name = DIVIDER_GAME_OBJECT_NAME };
		Originise(gameObject);
		gameObject.tag = EDITOR_ONLY_TAG;

		// select the object.
		Selection.activeGameObject = gameObject;

		// register object creation with undo.
		Undo.RegisterCreatedObjectUndo(gameObject, "Create Divider in Root");
	}

	// special functionality.

	/// <summary>Renames a list of objects based on the first object selected.</summary>
	[MenuItem(MENU_ITEM_LOCATION + "/Rename Objects by Order %&r", false, MENU_ITEM_STARTING_PRIORITY + 33)]
	public static void RenameObjectsByOrder() {

		// grab all selected objects, backing out if they're empty.
		Transform[] selectedTransforms = Selection.transforms;
		if (selectedTransforms.IsNullOrEmpty()) return;

		// create and populate a list to be sorted into sibling index.
		List<Transform> sortedBySiblingIndex = selectedTransforms.ToList();

		// sort our list by sibling index.
		sortedBySiblingIndex.Sort((t1, t2) => t1.GetSiblingIndex().CompareTo(t2.GetSiblingIndex()));
		
		// grab our base name from the first (highest) item we have selected and trim whitespace from beginning and end.
		string baseName = sortedBySiblingIndex[0].name.Trim();

		// match on the name with an optional number.
		Match baseNameMatch = NAME_WITH_OPTIONAL_NUMBER_SUFFIX_REGEX.Match(baseName);
		if (baseNameMatch.Success && baseNameMatch.Groups.Count > 0) {
			
			// N.B. the first match group represents the entire string, where as an ID of 1 represents the first capture group.
			const int NAME_MATCH_GROUP_ID = 1;

			// set base name to be everything except the numbers on the end, trimmed.
			baseName = baseNameMatch.Groups[NAME_MATCH_GROUP_ID].Value.Trim();
		}

		// create an undo group so that all of our actions are linked undo one undo order. 
		const string GROUP_UNDO_GROUP_NAME = "Rename Objects by Order";
		Undo.SetCurrentGroupName(GROUP_UNDO_GROUP_NAME);
		int groupUndoGroupId = Undo.GetCurrentGroup();

		// foreach sorted item in our selection...
		for (int i = 0; i < sortedBySiblingIndex.Count; i++) {

			// register an undo event and rename the item based on our base name, our index, and our index format.
			Undo.RegisterCompleteObjectUndo(sortedBySiblingIndex[i].gameObject, GROUP_UNDO_GROUP_NAME);
			sortedBySiblingIndex[i].gameObject.name = baseName + string.Format(RENAME_BY_ORDER_NAME_SUFFIX_FORMAT, (i + 1));
		}

		// collapse our undo group to create our undo object. 
		Undo.CollapseUndoOperations(groupUndoGroupId);
	}

	[MenuItem(MENU_ITEM_LOCATION + "/Order Alphabetically %&o", false, MENU_ITEM_STARTING_PRIORITY + 33)]
	public static void OrderAlphabetically() {

		// grab all selected objects, backing out if they're empty.
		Transform[] selectedTransforms = Selection.transforms;
		if (selectedTransforms.IsNullOrEmpty()) return;

		// sort them by our sorting function as a list.
		List<Transform> sortedByAlphabet = selectedTransforms.ToList();
		sortedByAlphabet.Sort(OrderAlphabeticallyWithNumberSuffixSort);

		// find the lowest sibling index, which will use as a base value and add to later.
		int lowestSiblingIndex = int.MaxValue;
		foreach (Transform sorted in sortedByAlphabet) {
			if (sorted.GetSiblingIndex() < lowestSiblingIndex) {
				lowestSiblingIndex = sorted.GetSiblingIndex();
			}
		}

		// create an undo group so that all of our actions are linked undo one undo order. 
		const string GROUP_UNDO_GROUP_NAME = "Order Alphabetically";
		Undo.SetCurrentGroupName(GROUP_UNDO_GROUP_NAME);
		int groupUndoGroupId = Undo.GetCurrentGroup();

		// iterate through transforms and set their sibling index to lowest plus an incrememnt.
		for (int i = 0; i < sortedByAlphabet.Count; i++) {
			Undo.SetTransformParent(sortedByAlphabet[i], sortedByAlphabet[i].parent, GROUP_UNDO_GROUP_NAME);
			sortedByAlphabet[i].SetSiblingIndex(lowestSiblingIndex + i);
		}

		// collapse our undo group to create our undo object. 
		Undo.CollapseUndoOperations(groupUndoGroupId);
	}
	
	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------
	
	public static void Originise(GameObject gameObject) {
		Transform gameObjectTransform = gameObject.transform;
		gameObjectTransform.localPosition = Vector3.zero;
		gameObjectTransform.localEulerAngles = Vector3.zero;
		gameObjectTransform.localScale = Vector3.one;
	}

	public static float Round(float value) {
		float intRoundPadding = (value >= 0) ? 0.5f : -0.5f;
		return (int)(value + intRoundPadding);
	}

	public static Vector3 Round(Vector3 vector3) {
		return new Vector3(Round(vector3.x), Round(vector3.y), Round(vector3.z));
	}
	
	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------
	
	private static int OrderAlphabeticallyWithNumberSuffixSort(Component c1, Component c2) {
		return OrderAlphabeticallyWithNumberSuffixSort(c1.name, c2.name);
	}

	private static int OrderAlphabeticallyWithNumberSuffixSort(string s1, string s2) {

		// trim strings so whitespace at the beginning and end of each string is not factored in.
		s1 = s1.Trim();
		s2 = s2.Trim();

		// work out default string sort, which we'll return at numerous places if we don't have regex matches.
		int defaultAlphabeticalSortValue = string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);

		// work out if both transforms have names and number suffixes.
		Match t1Match = NAME_AND_NUMBER_SUFFIX_REGEX.Match(s1);
		Match t2Match = NAME_AND_NUMBER_SUFFIX_REGEX.Match(s2);

		// if we failed to get a match or a group capture, return default alphabetical sort.
		if (!t1Match.Success || !t2Match.Success || t1Match.Groups.Count == 0 || t2Match.Groups.Count == 0)
			return defaultAlphabeticalSortValue;

		// N.B. the first match group represents the entire string, where as an ID of 1 represents the first capture group.
		const int NAME_MATCH_GROUP_ID = 1;

		// if we got a match but the names are not the same (which is the string excluding its number), return default sort.
		string s1Name = t1Match.Groups[NAME_MATCH_GROUP_ID].Value.Trim();
		string s2Name = t2Match.Groups[NAME_MATCH_GROUP_ID].Value.Trim();
		if (s1Name != s2Name) return defaultAlphabeticalSortValue;

		const int NUMBER_MATCH_GROUP_ID = 2;

		// we have a match so try to extract the number capture groups as ints, returning default alphabetical sort if we fail.
		// ReSharper disable InlineOutVariableDeclaration
		int s1Number;
		int s2Number;
		// ReSharper restore InlineOutVariableDeclaration
		if (!int.TryParse(t1Match.Groups[NUMBER_MATCH_GROUP_ID].Value, out s1Number) || !int.TryParse(t2Match.Groups[NUMBER_MATCH_GROUP_ID].Value, out s2Number))
			return defaultAlphabeticalSortValue;

		// we extracted the number, so return a match with corresponding sort value.
		if (s1Number > s2Number) return 1;
		if (s1Number < s2Number) return -1;
		return 0;
	}
	
	//-----------------------------------------------------------------------------------------
	// Classes:
	//-----------------------------------------------------------------------------------------

	public class GameObjectSiblingIndexSort : IComparer<GameObject> {

		public int Compare(GameObject gameObjectA, GameObject gameObjectB) {
			
			// if they're the same, then there's no comparison.
			if (gameObjectA == gameObjectB) return 0;

			// if a is null and b is not, a goes first.
			if (gameObjectA == null) return -1;

			// if b is null and a is not, a goes after.
			if (gameObjectB == null) return 1;

			// grab transform references, using short aliases.
			Transform a = gameObjectA.transform;
			Transform b = gameObjectB.transform;

			// if b is a child of a, a goes first.
			if (b.IsChildOf(a)) return -1;

			// if a is a child of b, a goes after.
			if (a.IsChildOf(b)) return 1;

			// get lists of a and b's parents.
			List<Transform> aParents = GetParents(a);
			List<Transform> bParents = GetParents(b);

			// for all of a's parents...
			for (int aIndex = 0; aIndex < aParents.Count; aIndex++) {

				// if b is a child of any of a's parents, work out the index based on the shared parents of a and b.
				if (b.IsChildOf(aParents[aIndex])) {
					int bIndex = bParents.IndexOf(aParents[aIndex]) - 1;
					--aIndex;
					return aParents[aIndex].GetSiblingIndex() - bParents[bIndex].GetSiblingIndex();
				}
			}

			// otherwise it's just the difference between the last parent (i.e. most top-level ancestor) of a and b.
			return aParents[aParents.Count - 1].GetSiblingIndex() - bParents[bParents.Count - 1].GetSiblingIndex();
		}

		private static List<Transform> GetParents(Transform transform) {

			// make a list for collecting parents.
			List<Transform> parents = new List<Transform> { transform };

			// while our transform parent is not the root, keep adding transforms and then do the same on the parent recursively.
			while (transform.parent != null) {
				parents.Add(transform.parent);
				transform = transform.parent;
			}

			return parents;
		}
	}
}
