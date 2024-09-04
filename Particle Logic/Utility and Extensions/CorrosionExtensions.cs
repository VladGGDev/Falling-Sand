public static class CorrosionExtensions
{
	public static bool IsCorrodible(this ParticleGrid grid, Point destination)
	{
		if (!grid.IsInsideBounds(destination))
			return false;
		ICorrodible particle = grid.GetParticle(destination) as ICorrodible;
		return particle != null;
	}

	public static bool IsCorrodible(this ParticleGrid grid, Point destination, out ICorrodible corrodibleParticle)
	{
		if (!grid.IsInsideBounds(destination))
		{
			corrodibleParticle = null;
			return false;
		}
		corrodibleParticle = grid.GetParticle(destination) as ICorrodible;
		return corrodibleParticle != null;
	}



	public static bool Corrode(this ParticleGrid grid, ICorrosive thisParticle, Point from, Point to)
	{
		if (!grid.IsInsideBounds(to))
			return false;

		ICorrodible particle = grid.GetParticle(to) as ICorrodible;
		if (particle.CanBeCorroded(thisParticle))
		{
			grid.MoveParticle(from, to);
			return true;
		}
		return false;
	}

	public static bool Corrode(this ParticleGrid grid, ICorrosive thisParticle, Point from, ICorrodible otherParticle, Point to)
	{
		if (!grid.IsInsideBounds(to))
			return false;

		if (otherParticle.CanBeCorroded(thisParticle))
		{
			grid.MoveParticle(from, to);
			return true;
		}
		return false;
	}






	public static bool CanCorrode(this ParticleGrid grid, ICorrosive thisParticle, Point destination)
	{
		if (!grid.IsInsideBounds(destination))
			return false;
		ICorrodible particle = grid.GetParticle(destination) as ICorrodible;
		return particle?.CanBeCorroded(thisParticle) ?? false;
	}






	public static bool TryCorrode(this ParticleGrid grid, ICorrosive thisParticle, Point from, Point to)
	{
		if (IsCorrodible(grid, to, out ICorrodible particle))
		{
			if (thisParticle != particle)
				grid.UpdateSurroundingChunks(from);
			return Corrode(grid, thisParticle, from, particle, to);
		}
		return false;
	}

	public static bool TryCorrodeOrMove(this ParticleGrid grid, ICorrosive thisParticle, Point from, Point to)
	{
		if (TryCorrode(grid, thisParticle, from, to))
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
