using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComputeShaderUtility;

[CreateAssetMenu]
public class Settings : ScriptableObject
{
	public const int numRules = 8;
	public int stepsPerFrame = 1;
	public Vector2 noiseOffset;
	public Rule[] rules;

	public void RandomizeConditions(int seed)
	{
		System.Random prng = new System.Random(seed);
		if (rules == null || rules.Length != numRules)
		{
			rules = new Rule[numRules];
		}


		for (int i = 0; i < rules.Length; i++)
		{

			rules[i].radiusMinMax = RandomRadii(prng);

			rules[i].aliveMinMax = CalculateMinMaxPair(prng);
			rules[i].deadMinMax = CalculateMinMaxPair(prng);
		}
	}

	static Vector2Int RandomRadii(System.Random prng)
	{
		const int maxPossibleRadius = 10;
		int radiusA = prng.Next(0, maxPossibleRadius);
		int radiusB = prng.Next(0, maxPossibleRadius);
		int minRadius = (radiusA < radiusB) ? radiusA : radiusB;
		int maxRadius = (radiusA > radiusB) ? radiusA : radiusB;
		return new Vector2Int(minRadius, maxRadius);
	}

	static Vector2 CalculateMinMaxPair(System.Random prng)
	{
		float a = (float)prng.NextDouble();
		float b = (float)prng.NextDouble();

		if (a > b)
		{
			(a, b) = (b, a);
		}

		return new Vector2(a, b);
	}
}
