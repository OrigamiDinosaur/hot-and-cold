using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Apache.Core.Editor {
	public class LightingSettingsSnapshot {

		//-----------------------------------------------------------------------------------------
		// Type Definitions:
		//-----------------------------------------------------------------------------------------

		public enum SettingsType {
			Render,
			LightmapEditor
		}

		//-----------------------------------------------------------------------------------------
		// Public Fields:
		//-----------------------------------------------------------------------------------------

		// N.B. I'm using public fields instead of properties so I can instantiate and associate a property name without having to write all these settings twice.

		// N.B. the below comments map onto the structure of the Lighting window using a simplified markdown syntax.

		// # Environment

		public readonly Setting<Object> SkyboxMaterial = new Setting<Object>("m_SkyboxMaterial");
		public readonly Setting<Object> SunSource      = new Setting<Object>("m_Sun");

		// Environment Lighting

		public readonly Setting<int> EnvironmentLightingSource = new Setting<int>("m_AmbientMode");

		// > Skybox

		public readonly Setting<float> EnvironmentLightingSkyboxIntensityMultiplier = new Setting<float>("m_AmbientIntensity");

		// N.B. Gradient Sky Colour and Ambient Colour both use this property under the hood.
		public readonly Setting<Color> EnvironmentLightingSkyColour = new Setting<Color>("m_AmbientSkyColor");

		// > Gradient

		// N.B. for Gradient Sky Colour, see EnvironmentLightingSkyColour.

		public readonly Setting<Color> EnvironmentLightingGradientEquatorColour = new Setting<Color>("m_AmbientEquatorColor");
		public readonly Setting<Color> EnvironmentLightingGradientGroundColour  = new Setting<Color>("m_AmbientGroundColor");

		// > Colour

		// N.B. for Colour Ambient Colour, see EnvironmentLightingSkyColour.

		// <

		public readonly Setting<int> EnvironmentLightingAmbientMode = new Setting<int>("m_GISettings.m_EnvironmentLightingMode", SettingsType.LightmapEditor);

		// Environment Reflections

		public readonly Setting<int> EnvironmentReflectionsSource = new Setting<int>("m_DefaultReflectionMode");

		// > Skybox

		public readonly Setting<int> EnvironmentReflectionsSourceResolution = new Setting<int>("m_DefaultReflectionResolution");

		// > Custom

		public readonly Setting<Object> EnvironmentReflectionsSourceCustomCubemap = new Setting<Object>("m_CustomReflection");

		// <

		public readonly Setting<int>   EnvironmentReflectionsCompression         = new Setting<int>  ("m_LightmapEditorSettings.m_ReflectionCompression", SettingsType.LightmapEditor);
		public readonly Setting<float> EnvironmentReflectionsIntensityMultiplier = new Setting<float>("m_ReflectionIntensity");
		public readonly Setting<int>   EnvironmentReflectionsBounces             = new Setting<int>  ("m_ReflectionBounces");

		// N.B. the following seems to have been added in Unity 2017.1. It's not clear what it's for.
		public readonly Setting<Color> EnvironmentLightingIndirectSpecularColour = new Setting<Color>("m_IndirectSpecularColor");

		// # Realtime Lighting

		public readonly Setting<bool> LightingEnableRealtimeLightmaps = new Setting<bool>("m_GISettings.m_EnableRealtimeLightmaps", SettingsType.LightmapEditor);

		// # Mixed Lighting

		public readonly Setting<int>  LightingMixedBakeMode        = new Setting<int> ("m_LightmapEditorSettings.m_MixedBakeMode", SettingsType.LightmapEditor);
		public readonly Setting<bool> LightingUseShadowMask        = new Setting<bool>("m_UseShadowmask",                          SettingsType.LightmapEditor);
		public readonly Setting<bool> LightingEnableBakedLightmaps = new Setting<bool>("m_GISettings.m_EnableBakedLightmaps",      SettingsType.LightmapEditor);

		// N.B. this influences whether the "Auto Generate" checkbox is checked at the bottom right of the Lighting window.
		public readonly Setting<int> LightingGiWorkflowMode = new Setting<int>("m_GIWorkflowMode", SettingsType.LightmapEditor);

		public readonly Setting<int> LightingEnvironmentLightingMode = new Setting<int>("m_GISettings.m_EnvironmentLightingMode", SettingsType.LightmapEditor);

		// > Lighting Mode - Subtractive

		public readonly Setting<Color> LightingRealtimeShadowColour = new Setting<Color>("m_SubtractiveShadowColor");

		// # Lightmapping

		public readonly Setting<int> LightmappingLightmapper = new Setting<int>("m_LightmapEditorSettings.m_BakeBackend", SettingsType.LightmapEditor);

		// > Lightmapper - Progressive

		public readonly Setting<bool> LightmappingProgressiveLightmapperPrioritiseView  = new Setting<bool>("m_LightmapEditorSettings.m_PVRCulling",           SettingsType.LightmapEditor);
		public readonly Setting<int>  LightmappingProgressiveLightmapperDirectSamples   = new Setting<int> ("m_LightmapEditorSettings.m_PVRDirectSampleCount", SettingsType.LightmapEditor);
		public readonly Setting<int>  LightmappingProgressiveLightmapperIndirectSamples = new Setting<int> ("m_LightmapEditorSettings.m_PVRSampleCount",       SettingsType.LightmapEditor);
		public readonly Setting<int>  LightmappingProgressiveLightmapperBounces         = new Setting<int> ("m_LightmapEditorSettings.m_PVRBounces",           SettingsType.LightmapEditor);

		// > Filtering

		public readonly Setting<int> LightmappingProgressiveLightmapperFiltering = new Setting<int>("m_LightmapEditorSettings.m_PVRFilteringMode", SettingsType.LightmapEditor);

		// > Filtering - Advanced

		public readonly Setting<int> LightmappingProgressiveLightmapperFilteringDirectRadius           = new Setting<int>("m_LightmapEditorSettings.m_PVRFilteringGaussRadiusDirect",   SettingsType.LightmapEditor);
		public readonly Setting<int> LightmappingProgressiveLightmapperFilteringIndirectRadius         = new Setting<int>("m_LightmapEditorSettings.m_PVRFilteringGaussRadiusIndirect", SettingsType.LightmapEditor);
		public readonly Setting<int> LightmappingProgressiveLightmapperFilteringAmbientOcclusionRadius = new Setting<int>("m_LightmapEditorSettings.m_PVRFilteringGaussRadiusAO",       SettingsType.LightmapEditor);

		// <

		public readonly Setting<float> LightmappingIndirectResolution = new Setting<float>("m_LightmapEditorSettings.m_Resolution",         SettingsType.LightmapEditor);
		public readonly Setting<float> LightmappingLightmapResolution = new Setting<float>("m_LightmapEditorSettings.m_BakeResolution",     SettingsType.LightmapEditor);
		public readonly Setting<int>   LightmappingLightmapPadding    = new Setting<int>  ("m_LightmapEditorSettings.m_Padding",            SettingsType.LightmapEditor);
#if !UNITY_2018_1_OR_NEWER
		public readonly Setting<int>   LightmappingTextureSize        = new Setting<int>  ("m_LightmapEditorSettings.m_TextureWidth",       SettingsType.LightmapEditor);
#endif
		public readonly Setting<bool>  LightmappingCompressLightmaps  = new Setting<bool> ("m_LightmapEditorSettings.m_TextureCompression", SettingsType.LightmapEditor);

		// > Ambient Occlusion

		public readonly Setting<bool>  LightmappingAmbientOcclusion                     = new Setting<bool> ("m_LightmapEditorSettings.m_AO",                   SettingsType.LightmapEditor);
		public readonly Setting<float> LightmappingAmbientOcclusionMaxDistance          = new Setting<float>("m_LightmapEditorSettings.m_AOMaxDistance",        SettingsType.LightmapEditor);
		public readonly Setting<float> LightmappingAmbientOcclusionIndirectContribution = new Setting<float>("m_LightmapEditorSettings.m_CompAOExponent",       SettingsType.LightmapEditor);
		public readonly Setting<float> LightmappingAmbientOcclusionDirectContribution   = new Setting<float>("m_LightmapEditorSettings.m_CompAOExponentDirect", SettingsType.LightmapEditor);

		// <

		// > Lightmapper - Enlighten - Final Gather

		public readonly Setting<bool> LightmappingFinalGather          = new Setting<bool>("m_LightmapEditorSettings.m_FinalGather",          SettingsType.LightmapEditor);
		public readonly Setting<int>  LightmappingFinalGatherRayCount  = new Setting<int> ("m_LightmapEditorSettings.m_FinalGatherRayCount",  SettingsType.LightmapEditor);
		public readonly Setting<bool> LightmappingFinalGatherDenoising = new Setting<bool>("m_LightmapEditorSettings.m_FinalGatherFiltering", SettingsType.LightmapEditor);

		// <

		public readonly Setting<int>    LightmappingDirectionalMode    = new Setting<int>   ("m_LightmapEditorSettings.m_LightmapsBakeMode",  SettingsType.LightmapEditor);
		public readonly Setting<float>  LightmappingIndirectIntensity  = new Setting<float> ("m_GISettings.m_IndirectOutputScale",            SettingsType.LightmapEditor);
		public readonly Setting<float>  LightmappingAlbedoBoost        = new Setting<float> ("m_GISettings.m_AlbedoBoost",                    SettingsType.LightmapEditor);
		public readonly Setting<Object> LightmappingLightmapParameters = new Setting<Object>("m_LightmapEditorSettings.m_LightmapParameters", SettingsType.LightmapEditor);

		// > Unknown

		// N.B. the following settings pertain to lightmapping according to Unity's decompiled source code, but I cannot see anywhere where they appear in the editor.

		public readonly Setting<float> LightmappingBounceScale                = new Setting<float>("m_GISettings.m_BounceScale",                SettingsType.LightmapEditor);
		public readonly Setting<float> LightmappingTemporalCoherenceThreshold = new Setting<float>("m_GISettings.m_TemporalCoherenceThreshold", SettingsType.LightmapEditor);

		// <

		// Other Settings

		// > Fog

		public readonly Setting<bool>  OtherSettingsFog       = new Setting<bool> ("m_Fog");
		public readonly Setting<Color> OtherSettingsFogColour = new Setting<Color>("m_FogColor");
		public readonly Setting<int>   OtherSettingsFogMode   = new Setting<int>  ("m_FogMode");

		// > Fog - Linear

		public readonly Setting<float> OtherSettingsFogModeLinearStart = new Setting<float>("m_LinearFogStart");
		public readonly Setting<float> OtherSettingsFogModeLinearEnd   = new Setting<float>("m_LinearFogEnd");

		// > Fog - Exponential & Exponential Squared

		public readonly Setting<float> OtherSettingsFogModeExponentialDensity = new Setting<float>("m_FogDensity");

		// <

		public readonly Setting<Object> OtherSettingsHaloTexture    = new Setting<Object>("m_HaloTexture");
		public readonly Setting<float>  OtherSettingsHaloStrength   = new Setting<float> ("m_HaloStrength");
		public readonly Setting<float>  OtherSettingsFlareFadeSpeed = new Setting<float> ("m_FlareFadeSpeed");
		public readonly Setting<float>  OtherSettingsFlareStrength  = new Setting<float> ("m_FlareStrength");
		public readonly Setting<Object> OtherSettingsSpotCookie     = new Setting<Object>("m_SpotCookie");

		// Debug Settings

		// N.B. debug settings are not scene-specific; they are editor settings shared across scenes, so we don't want to support these.

		// Auto Generate

		// N.B. LightingGiWorkflowMode above influences whether the "Auto Generate" checkbox is checked, but it has other uses, so I won't put it here.

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public object[] GetSettingsAsObjects() {

			// get fields of type Setting<?>.
			return GetType()
				.GetFields()
				.Where(p => p.FieldType.IsGenericType && p.FieldType.GetGenericTypeDefinition() == typeof(Setting<>))
				.Select(p => p.GetValue(this))
				.ToArray();
		}

		public object GetValueOnSettingAsObject(object setting) {
			PropertyInfo propertyInfo = setting.GetType().GetProperty("Value");
			return (propertyInfo != null) ? propertyInfo.GetValue(setting, null) : null;
		}

		public object GetPropertyNameOnSettingAsObject(object setting) {
			FieldInfo fieldInfo = setting.GetType().GetField("propertyName", BindingFlags.NonPublic | BindingFlags.Instance);
			return (fieldInfo != null) ? fieldInfo.GetValue(setting) : null;
		}

		public void CallSetValueInSettingsOnSettingAsObject(object setting) {
			MethodInfo methodInfo = setting.GetType().GetMethod("SetValueInSettings");
			if (methodInfo == null) throw new InvalidOperationException();
			methodInfo.Invoke(setting, null);
		}

		public void CallOnAllSettings(Action<object> action) {
			GetSettingsAsObjects().ToList().ForEach(action);
		}

		public void PrintValues() {
			CallOnAllSettings(setting => Debug.Log(GetPropertyNameOnSettingAsObject(setting) + " = " + GetValueOnSettingAsObject(setting)));
		}

		public void SetCurrentSceneLightingSettingsWithValues() {
			CallOnAllSettings(CallSetValueInSettingsOnSettingAsObject);
		}

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		public class Setting<T> {

			//-----------------------------------------------------------------------------------------
			// Constants:
			//-----------------------------------------------------------------------------------------

			private const string GET_RENDER_SETTINGS_METHOD_NAME   = "GetRenderSettings";
			private const string GET_LIGHTMAP_SETTINGS_METHOD_NAME = "GetLightmapSettings";

			private const string SERIALISED_PROPERTY_BOOL_PROPERTY_NAME   = "boolValue";
			private const string SERIALISED_PROPERTY_INT_PROPERTY_NAME    = "intValue";
			private const string SERIALISED_PROPERTY_FLOAT_PROPERTY_NAME  = "floatValue";
			private const string SERIALISED_PROPERTY_OBJECT_PROPERTY_NAME = "objectReferenceValue";
			private const string SERIALISED_PROPERTY_COLOUR_PROPERTY_NAME = "colorValue";

			//-----------------------------------------------------------------------------------------
			// Protected Fields:
			//-----------------------------------------------------------------------------------------

			protected readonly string propertyName;
			protected SettingsType settingsType;

			//-----------------------------------------------------------------------------------------
			// Public Properties:
			//-----------------------------------------------------------------------------------------

			public T Value { get; set; }

			//-----------------------------------------------------------------------------------------
			// Public Methods:
			//-----------------------------------------------------------------------------------------

			public Setting(string aPropertyName, SettingsType aSettingsType = SettingsType.Render) {
				propertyName = aPropertyName;
				settingsType = aSettingsType;

				// get value from serialised property.
				SerializedObject serialisedObject = GetSerialisedObject();
				SerializedProperty serialisedProperty = serialisedObject.FindProperty(propertyName);
				try {
					Value = (T)GetSerialisedPropertyInfo(serialisedProperty).GetValue(serialisedProperty, null);
				}
				catch {
					Debug.LogError("Failed to find " + aPropertyName + ".");
					throw;
				}
			}

			public void SetValueInSettings() {

				// get the serialised property and cache it, then set the value using the appropriate property info.
				SerializedObject serialisedObject = GetSerialisedObject();
				SerializedProperty serialisedProperty = serialisedObject.FindProperty(propertyName);
				GetSerialisedPropertyInfo(serialisedProperty)
					.SetValue(serialisedProperty, Value, null);

				// apply the modified property.
				serialisedObject.ApplyModifiedProperties();
			}

			//-----------------------------------------------------------------------------------------
			// Private Methods:
			//-----------------------------------------------------------------------------------------

			private SerializedObject GetSerialisedObject() {

				// determine type and method name to get the settings based on our settings type.
				Type type;
				string getMethodName;
				switch (settingsType) {
					case SettingsType.Render:
						type = typeof(RenderSettings);
						getMethodName = GET_RENDER_SETTINGS_METHOD_NAME;
						break;
					case SettingsType.LightmapEditor:
						type = typeof(LightmapEditorSettings);
						getMethodName = GET_LIGHTMAP_SETTINGS_METHOD_NAME;
						break;
					default:
						throw new InvalidOperationException();
				}

				// get settings via internal Unity method.
				MethodInfo method = type.GetMethod(getMethodName, BindingFlags.Static | BindingFlags.NonPublic);
				if (method == null) throw new InvalidOperationException();
				Object settingsObject = method.Invoke(null, null) as Object;

				// wrap it up in a serialized object so we can get at its properties safely, then get serialized property.
				return new SerializedObject(settingsObject);
			}

			private static PropertyInfo GetSerialisedPropertyInfo(SerializedProperty serialisedProperty) {

				// get serialised property and type.
				Type serialisedProperyType = serialisedProperty.GetType();

				// get the property for the appropriate type.
				if (typeof(T) == typeof(bool)) {
					return serialisedProperyType.GetProperty(SERIALISED_PROPERTY_BOOL_PROPERTY_NAME);
				}
				if (typeof(T) == typeof(int)) {
					return serialisedProperyType.GetProperty(SERIALISED_PROPERTY_INT_PROPERTY_NAME);
				}
				if (typeof(T) == typeof(float)) {
					return serialisedProperyType.GetProperty(SERIALISED_PROPERTY_FLOAT_PROPERTY_NAME);
				}
				if (typeof(T) == typeof(Object)) {
					return serialisedProperyType.GetProperty(SERIALISED_PROPERTY_OBJECT_PROPERTY_NAME);
				}
				if (typeof(T) == typeof(Color)) {
					return serialisedProperyType.GetProperty(SERIALISED_PROPERTY_COLOUR_PROPERTY_NAME);
				}

				throw new InvalidOperationException();
			}
		}
	}
}