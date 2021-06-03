using UnityEngine;

[System.Serializable]
public struct Rule
{
	public Vector2Int radiusMinMax;
	public Vector2 aliveMinMax;
	public Vector2 deadMinMax;
}