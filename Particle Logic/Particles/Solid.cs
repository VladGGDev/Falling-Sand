using System;

public class Solid : IParticle
{
	public Color GetColor(ParticleGrid grid, Point position)
	{
		return Color.Lerp(new(133, 75, 42), new(110, 42, 22), ParticleUtility.NoiseGrid(grid, position));
	}

	public void Update(ParticleGrid grid, Point position)
	{
		// Nothing
	}
}
