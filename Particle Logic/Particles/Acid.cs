using System;

public class Acid : IParticle, IDensity, ICorrosive
{
	public int Density { get; init; } = 1500;
	public float PassThroughChance { get; init; } = 0.1f;

	public int CorrosionPower { get; init; } = 100;
	public float CorrosionChance { get; init; } = 0.25f;

	public Color GetColor(ParticleGrid grid, Point position)
	{
		return Color.Lerp(new(40, 255, 33), new(43, 245, 37), grid.NoiseGrid(position));
	}

	public void Update(ParticleGrid grid, Point position)
	{
		bool goRight = ParticleUtility.RandomBool();
		Point down = position + new Point(0, 1);
		Point downRight = position + new Point(1, 1);
		Point downLeft = position + new Point(-1, 1);
		Point right = position + new Point(1, 0);
		Point left = position + new Point(-1, 0);
		if (grid.TryCorrode(this, position, down))
			return;
		else if (grid.TryDensityOrMove(this, position, down))
			return;
		else if (grid.TryCorrode(this, position, goRight ? downRight : downLeft))
			return;
		else if (grid.TryDensityOrMove(this, position, goRight ? downRight : downLeft))
			return;
		else if(grid.TryCorrode(this, position, downRight))
			return;
		else if (grid.TryDensityOrMove(this, position, downRight))
			return;
		else if(grid.TryCorrode(this, position, downLeft))
			return;
		else if (grid.TryDensityOrMove(this, position, downLeft))
			return;
		else if(grid.TryCorrode(this, position, goRight ? right : left))
			return;
		else if (grid.TryDensityOrMove(this, position, goRight ? right : left))
			return;
		else if(grid.TryCorrode(this, position, right))
			return;
		else if (grid.TryDensityOrMove(this, position, right))
			return;
		else if(grid.TryCorrode(this, position, left))
			return;
		else if (grid.TryDensityOrMove(this, position, left))
			return;
	}
}
