public static class DensityExtensions
{
	public static bool CanDoDensity(this ParticleGrid grid, Point destination)
	{
		if (!grid.IsInsideBounds(destination))
			return false;
		IDensity particle = grid.GetParticle(destination) as IDensity;
		return particle != null;
	}

	public static bool CanDoDensity(this ParticleGrid grid, Point destination, out IDensity densityParticle)
	{
		if (!grid.IsInsideBounds(destination))
		{
			densityParticle = null;
			return false;
		}
		densityParticle = grid.GetParticle(destination) as IDensity;
		return densityParticle != null;
	}



	public static bool DensityMove(this ParticleGrid grid, IDensity thisParticle, Point from, Point to)
	{
		if (!grid.IsInsideBounds(to))
			return false;

		IDensity particle = grid.GetParticle(to) as IDensity;
		if (particle.CanBePassedThrough(thisParticle))
		{
			grid.SwapParticle(from, to);
			return true;
		}
		return false;
	}

	public static bool DensityMove(this ParticleGrid grid, IDensity thisParticle, Point from, IDensity otherParticle, Point to)
	{
		if (!grid.IsInsideBounds(to))
			return false;

		if (otherParticle.CanBePassedThrough(thisParticle))
		{
			grid.SwapParticle(from, to);
			return true;
		}
		return false;
	}



	public static bool TryDensityMove(this ParticleGrid grid, IDensity thisParticle, Point from, Point to)
	{
		if (CanDoDensity(grid, to, out IDensity particle))
		{
			if (particle.CanBePassedThrough(thisParticle))
			{
				grid.SwapParticle(from, to);
				return true;
			}
		}
		else
		{
			if (grid.ValidMovePosition(to))
			{
				grid.MoveParticle(from, to);
				return true;
			}
		}
		return false;
	}
}
