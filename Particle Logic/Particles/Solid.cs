using System;

public class Solid : IParticle, ICorrodible
{
	public int CorrosionResistance { get; init; } = 50;
	public float CorrosionChanceMultiplier { get; init; } = 0.15f;

	public Color GetColor(ParticleGrid grid, Point position)
	{
		return Color.Lerp(new(133, 75, 42), new(110, 42, 22), grid.NoiseGrid(position));
	}

	public void Update(ParticleGrid grid, Point position)
	{
		// Nothing
	}
}
