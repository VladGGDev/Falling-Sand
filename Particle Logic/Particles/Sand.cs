using System;

public class Sand : IParticle, IDensity
{
	public int Density { get; init; } = 10000;
	public float Viscosity { get; init; } = 0.5f;

	public Color GetColor(ParticleGrid grid, Point position)
	{
		return Color.Lerp(Color.Yellow, new(1f, 0.7f, 0), ParticleUtility.NoiseGrid(grid, position));
	}

	public void Update(ParticleGrid grid, Point position)
	{
		Point down = position + new Point(0, 1);
		Point downRight = position + new Point(1, 1);
		Point downLeft = position + new Point(-1, 1);
		if (DoCalculation(grid, position, down)) return;
		else if(DoCalculation(grid, position, ParticleUtility.RandomBool() ? downLeft : downRight)) return;
		else if(DoCalculation(grid, position, downRight)) return;
		else if(DoCalculation(grid, position, downLeft)) return;
	}

	bool DoCalculation(ParticleGrid grid, Point position, Point other)
	{
		if (!grid.IsInsideBounds(other))
			return false;

		IDensity particle = grid.GetParticle(other) as IDensity;
		if (particle == null)
		{
			if (grid.IsEmpty(other))
			{
				grid.MoveParticle(position, other);
				return true;
			}
		}
		else if (particle.CanBePassedThrough(this))
		{
			grid.SwapParticle(position, other);
			return true;
		}

		return false;
	}
}
