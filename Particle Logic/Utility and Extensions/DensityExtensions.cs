// Usage examples:
// if (grid.HasDensity(destination, out other))
//	    grid.DensityMove(this, position, other, destination);
//
// OR
//
// if (grid.CanPassThrough(this, destination, out other))
//	    grid.PassThrough(position, destination);
//
// OR
//
// if (grid.TryDensityOrMove(...)) return;

public static class DensityExtensions
{
	public static bool HasDensity(this ParticleGrid grid, Point destination)
	{
		if (!grid.IsInsideBounds(destination))
			return false;
		IDensity particle = grid.GetParticle(destination) as IDensity;
		return particle != null;
	}

	public static bool HasDensity(this ParticleGrid grid, Point destination, out IDensity densityParticle)
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






	public static bool CanPassThrough(this ParticleGrid grid, IDensity thisParticle, Point destination)
	{
		if (!grid.IsInsideBounds(destination))
			return false;
		IDensity particle = grid.GetParticle(destination) as IDensity;
		return particle?.CanBePassedThrough(thisParticle) ?? false;
	}

	public static bool CanPassThrough(this ParticleGrid grid, IDensity thisParticle, Point destination, out IDensity densityParticle)
	{
		if (!grid.IsInsideBounds(destination))
		{
			densityParticle = null;
			return false;
		}
		densityParticle = grid.GetParticle(destination) as IDensity;
		return densityParticle?.CanBePassedThrough(thisParticle) ?? false;
	}



	public static bool PassThrough(this ParticleGrid grid, Point from, Point to)
	{
		if (!grid.IsInsideBounds(to))
			return false;
		grid.SwapParticle(from, to);
		return true;
	}






	public static bool TryDensity(this ParticleGrid grid, IDensity thisParticle, Point from, Point to)
	{
		if (HasDensity(grid, to, out IDensity particle))
		{
			if (thisParticle != particle)
				grid.UpdateSurroundingChunks(from);
			return DensityMove(grid, thisParticle, from, particle, to);
		}
		return false;
	}

	public static bool TryDensityOrMove(this ParticleGrid grid, IDensity thisParticle, Point from, Point to)
	{
		if (TryDensity(grid, thisParticle, from, to))
		{
			return true;
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
