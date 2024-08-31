using System;

public static class ParticleUtility
{
	static Random _rand = new();

	public static int RandomRange(int minInclusive, int maxExclusive) => _rand.Next(minInclusive, maxExclusive);
	public static float RandomValue() => _rand.NextSingle();
	public static bool RandomBool(float trueChance = 0.5f) => _rand.NextSingle() < trueChance;
}
