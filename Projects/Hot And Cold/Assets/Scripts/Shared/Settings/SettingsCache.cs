using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Apache.Core.Extensions;
using UnityEngine;

namespace Apache.Core {
	public static class SettingsCache {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const int MAX_NULL_FIX_RECURSION_DEPTH = 6;

		public const string DEFAULT_RELATIVE_DIRECTORY_PATH = "../Resources/Settings";
		public const string PERSISTENT_RELATIVE_DIRECTORY_PATH = "Resources/Settings";
		
		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private static readonly Dictionary<string, SerialisedElement> serialisedElements = new Dictionary<string, SerialisedElement>();

		//-----------------------------------------------------------------------------------------
		// Constructors:
		//-----------------------------------------------------------------------------------------

		public static void Init() {

			// ensure serialised element collection starts clear.
			serialisedElements.Clear();

			// loaded all settings by accessing the getters of static public property in the Settings class.
			// N.B. this will call GetOrCreateSettingsElement with the type of the property (as a generic argument) and the name of the settings object.
			PropertyInfo[] properties = typeof(Settings).GetProperties(BindingFlag.STATIC_PUBLIC);
			MethodInfo[] getters = properties.Select(p => p.GetGetMethod()).ToArray();
			foreach (MethodInfo getter in getters) {
				if (getter == null) continue;
				getter.Invoke(null, null);
			}
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Returns the managed serialised element at the given <c>key</c>. If no element exists, a new one is attempted to be  attempts to create a new one.
		/// </summary>
		public static SerialisedElement<T> GetOrCreateSetting<T>(string key, bool isPersistent = false) {
			if (serialisedElements.ContainsKey(key)) {

				// ReSharper disable once UsePatternMatching
				SerialisedElement<T> cachedElement = serialisedElements[key] as SerialisedElement<T>;
				if (cachedElement != null) return cachedElement;

				Debug.LogWarning($"SerialisedElement found at key: { key }, but it is not of type SerialisedElement<{ typeof(T) }>.");
				return null;
			}

			SerialisedElement<T> newElement = NewSetting<T>(key, isPersistent);
			serialisedElements.Add(key, newElement);

			FindAndFixNullMembers(newElement.BaseData, newElement.BaseData.GetType());

			return newElement;
		}

		/// <summary>Loads all settings elements from their source files, irrespective of whether they have been loaded before.</summary>
		public static void ReloadAllSettings() {

			// reload each individual element.
			foreach (SerialisedElement element in serialisedElements.Values) {
				element.Reload();
			}
		}

		/// <summary>Saves all settings elements to their data files and invokes the settings loaded delegate.</summary>
		public static void SaveAllSettings() {

			// save each individual element.
			foreach (SerialisedElement element in serialisedElements.Values) {
				element.Save();
			}
		}

		/// <summary>Saves the given settings element to its data file and invokes the settings loaded delegate.</summary>
		public static void SaveSetting(SerialisedElement element, bool shouldSaveInEditor = false) {

			// save the element if necessary.
			// N.B. having the ability to not actually save it but still invoke settings loaded makes a lot of sense in the editor.
			if (shouldSaveInEditor || !Application.isEditor) {
				element.Save();
			}
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Simple helper method that returns a new <c>SerializedElement</c> while making it unnecessary to specify name and folder path in the project-specific partial class.
		/// </summary>
		/// <param name="name">An optional name override. If not provided, the <c>T</c> type name will be converted into title case.</param>
		/// <param name="isPersistent">Are these settings persisted in AppData so that they can survive across build installs?</param>
		private static SerialisedElement<T> NewSetting<T>(string name = null, bool isPersistent = false) {

			// work out name either from the given name or a Title Cased type name.
			name = name.IsNullOrWhitespace() ? typeof(T).ToString().CamelOrPascalToTitleCase() : name;

			// create serialised element passing through both default and persistent paths and whether we should persist.
			return new SerialisedElement<T>(name, DEFAULT_RELATIVE_DIRECTORY_PATH, isPersistent, PERSISTENT_RELATIVE_DIRECTORY_PATH);
		}

		//-----------------------------------------------------------------------------------------
		// Fixing Null Members:
		//-----------------------------------------------------------------------------------------

		private static void FindAndFixNullMembers(object instance, Type type, int depth = 0) {

			// check against max depth to avoid infinite loops/cyclic.
			if (depth > MAX_NULL_FIX_RECURSION_DEPTH || type == null) return;

			const BindingFlags PUBLIC_INSTANCE = BindingFlags.Instance | BindingFlags.Public;

			// iterate across all fields.
			foreach (FieldInfo fieldInfo in type.GetFields(PUBLIC_INSTANCE)) {

				// box the loop variable to avoid compiler issues.
				FieldInfo boxedFieldInfo = fieldInfo;

				// ReSharper disable ConvertToLocalFunction
				Action<object> setValue = obj => boxedFieldInfo.SetValue(instance, obj);
				Func<object>   getValue = ()  => boxedFieldInfo.GetValue(instance);
				// ReSharper restore ConvertToLocalFunction

				CheckMemberInfo(fieldInfo.FieldType, setValue, getValue, depth);
			}

			// iterate across all properties.
			foreach (PropertyInfo propertyInfo in type.GetProperties(PUBLIC_INSTANCE)) {

				// box the loop variable to avoid compiler issues.
				PropertyInfo boxedPropertyInfo = propertyInfo;

				// ReSharper disable ConvertToLocalFunction
				Action<object> setValue = obj => boxedPropertyInfo.SetValue(instance, obj, null);
				Func<object>   getValue = ()  => boxedPropertyInfo.GetValue(instance, null);
				// ReSharper restore ConvertToLocalFunction

				CheckMemberInfo(propertyInfo.PropertyType, setValue, getValue, depth);
			}
		}

		private static void CheckMemberInfo(Type type, Action<object> setValue, Func<object> getValue, int depth) {

			object value = getValue();

			// if we have a null string, use an empty string value.
			if (type == typeof(string) && value == null) {
				setValue(string.Empty);
				return;
			}

			// if we have a null string array, use an array of one empty string.
			if (type == typeof(string[]) && value == null) {
				setValue(new[] { string.Empty });
				return;
			}

			// if the value of the field is not null, check whether the field type is a valid recursion target.
			// valid targets are type which are not base types, are classes, and are not interfaces.
			// we don't need to check structs yet, but maybe one day we might.
			if (value != null) {
				if (ValidNamespace(type.Namespace) && type.IsClass && !type.IsInterface) {
					FindAndFixNullMembers(value, type, depth + 1);
				}
				return;
			}

			// since the field value is null, check to see if there is a default constructor on that fields type, backing out if there isn't.
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor == null) return;

			// if we have a default contructor, let the Activator create an instance.
			object newMemberInstance = Activator.CreateInstance(type);

			// assign the new member instance to the base instance.
			setValue(newMemberInstance);

			// check the new instance for null members too.
			FindAndFixNullMembers(newMemberInstance, type, depth + 1);
		}

		private static bool ValidNamespace(string namespaceString) {
			if (string.IsNullOrEmpty(namespaceString)) return true;

			return !namespaceString.Contains("System") && !namespaceString.Contains("UnityEngine") && !namespaceString.Contains("UnityEditor");
		}
	}
}