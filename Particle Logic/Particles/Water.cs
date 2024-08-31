using System;

public class Water : IParticle, IDensity
{
	public int Density { get; init; } = 1000;
	public float Viscosity { get; init; } = 0.4f;

	public Color GetColor(ParticleGrid grid, Point position)
	{
		return Color.Lerp(new(0, 136, 255), new(0, 110, 255), ParticleUtility.NoiseGrid(grid, position));
	}

	public void Update(ParticleGrid grid, Point position)
	{
		bool goRight = ParticleUtility.RandomBool();
		Point down = position + new Point(0, 1);
		Point downRight = position + new Point(1, 1);
		Point downLeft = position + new Point(-1, 1);
		Point right = position + new Point(1, 0);
		Point left = position + new Point(-1, 0);
		if (ParticleUtility.ValidMovePosition(grid, down))
			grid.MoveParticle(position, down);
		else if (ParticleUtility.ValidMovePosition(grid, goRight ? downRight : downLeft))
			grid.MoveParticle(position, goRight ? downRight : downLeft);
		else if (ParticleUtility.ValidMovePosition(grid, downRight))
			grid.MoveParticle(position, downRight);
		else if (ParticleUtility.ValidMovePosition(grid, downLeft))
			grid.MoveParticle(position, downLeft);
		else if (ParticleUtility.ValidMovePosition(grid, goRight ? right : left))
			grid.MoveParticle(position, goRight ? right : left);
		else if (ParticleUtility.ValidMovePosition(grid, right))
			grid.MoveParticle(position, right);
		else if (ParticleUtility.ValidMovePosition(grid, left))
			grid.MoveParticle(position, left);
	}
}
