using System;

public class Fire : IParticle
{
	public Color GetColor(ParticleGrid grid, Point position)
	{
		return Color.Lerp(new(255, 0, 0), new(255, 210, 31), grid.NoiseGrid(position));
	}

	public void Update(ParticleGrid grid, Point position)
	{
		// Update always because fire has a small lifetime
		grid.UpdateSurroundingChunks(position);


		Point up = position + new Point(0, -1);
		Point direction = ParticleUtility.RandomRange(0, 8) switch
		{
			0 => new Point(-1, -1),
			1 => new Point(0, -1),
			2 => new Point(1, -1),
			3 => new Point(1, 0),
			4 => new Point(1, 1),
			5 => new Point(0, 1),
			6 => new Point(-1, 1),
			7 => new Point(-1, 0),
			_ => Point.Zero
		} + position;

		if (grid.ValidMovePosition(up) && ParticleUtility.RandomBool(0.5f))
			grid.MoveParticle(position, up);
		else if (grid.ValidMovePosition(direction))
			grid.MoveParticle(position, direction);
	}
}
