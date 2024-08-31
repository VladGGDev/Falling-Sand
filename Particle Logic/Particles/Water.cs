using System;

public class Water : IParticle, IDensity
{
	public int Density { get; init; } = 1000;
	public float PassThroughChance { get; init; } = 0.4f;

	public Color GetColor(ParticleGrid grid, Point position)
	{
		return Color.Lerp(new(0, 136, 255), new(0, 110, 255), grid.NoiseGrid(position));
	}

	public void Update(ParticleGrid grid, Point position)
	{
		bool goRight = ParticleUtility.RandomBool();
		Point down = position + new Point(0, 1);
		Point downRight = position + new Point(1, 1);
		Point downLeft = position + new Point(-1, 1);
		Point right = position + new Point(1, 0);
		Point left = position + new Point(-1, 0);
		if (grid.ValidMovePosition(down))
			grid.MoveParticle(position, down);
		else if (grid.ValidMovePosition(goRight ? downRight : downLeft))
			grid.MoveParticle(position, goRight ? downRight : downLeft);
		else if (grid.ValidMovePosition(downRight))
			grid.MoveParticle(position, downRight);
		else if (grid.ValidMovePosition(downLeft))
			grid.MoveParticle(position, downLeft);
		else if (grid.ValidMovePosition(goRight ? right : left))
			grid.MoveParticle(position, goRight ? right : left);
		else if (grid.ValidMovePosition(right))
			grid.MoveParticle(position, right);
		else if (grid.ValidMovePosition(left))
			grid.MoveParticle(position, left);
	}
}
