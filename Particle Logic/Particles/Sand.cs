using System;

public class Sand : IParticle, IDensity, ICorrodible
{
	public int Density { get; init; } = 10000;
	public float PassThroughChance { get; init; } = 0.9f;

	public int CorrosionResistance { get; init; } = 10;
	public float CorrosionChanceMultiplier { get; init; } = 0.75f;

	public Color GetColor(ParticleGrid grid, Point position)
	{
		return Color.Lerp(Color.Yellow, new(1f, 0.7f, 0), grid.NoiseGrid(position));
	}

	public void Update(ParticleGrid grid, Point position)
	{
		Point down = position + new Point(0, 1);
		Point downRight = position + new Point(1, 1);
		Point downLeft = position + new Point(-1, 1);
		if (grid.TryDensityMove(this, position, down)) return;
		else if(grid.TryDensityMove(this, position, ParticleUtility.RandomBool() ? downLeft : downRight)) return;
		else if(grid.TryDensityMove(this, position, downRight)) return;
		else if(grid.TryDensityMove(this, position, downLeft)) return;
	}
}
