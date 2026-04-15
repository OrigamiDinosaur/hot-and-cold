using UnityEngine;

namespace Apache.Core {

	// N.B. we do not define an empty or generic delegate because this is precisely what System.Action, System.Action<T> and <T1,...T16> is for.

	//-----------------------------------------------------------------------------------------
	// Delegates - Core:
	//-----------------------------------------------------------------------------------------
	
	public delegate void BoolDelegate(bool value);

	public delegate void IntDelegate(int value);

	public delegate void FloatDelegate(float value);

	public delegate void StringDelegate(string value);

	public delegate void DoubleDelegate(double value);

	//-----------------------------------------------------------------------------------------
	// Delegates - Unity:
	//-----------------------------------------------------------------------------------------

	public delegate void Vector2Delegate(Vector2 value);

	public delegate void Vector3Delegate(Vector3 value);

	public delegate void Vector4Delegate(Vector4 value);

	public delegate void Vector2IntDelegate(Vector2Int value);

	public delegate void Vector3IntDelegate(Vector3Int value);

	public delegate void QuaternionDelegate(Quaternion value);

	public delegate void ColorDelegate(Color colour);

	public delegate void Color32Delegate(Color32 colour);

	public delegate void TextureDelegate(Texture texture);

	public delegate void Texture2DDelegate(Texture2D texture);

	public delegate void RenderTextureDelegate(RenderTexture renderTexture);
	
	//-----------------------------------------------------------------------------------------
	// Delegates - Apache:
	//-----------------------------------------------------------------------------------------

	public delegate void LateralDelegate(Laterals lateral);
	
	public delegate void FloatRangeDelegate(FloatRange value);

	public delegate void Vector4IntDelegate(Vector4Int value);
}