using System;

public static class ParticleGridExtensions
{
	public static float NoiseGrid(this ParticleGrid grid, Point position) => grid.NoiseGrid[position.Y, position.X];

	public static void DrawLine(this ParticleGrid grid, int id, Point from, Point to, bool updateChunk = false)
	{
		int width = Math.Abs(to.X - from.X);
		int height = -Math.Abs(to.Y - from.Y);

		int dirX = from.X < to.X ? 1 : -1;
		int dirY = from.Y < to.Y ? 1 : -1;

		float error = width + height;


		while (true)
		{
			grid.CreateParticle(id, from.X, from.Y, updateChunk);

			if (from.X == to.X && from.Y == to.Y)
				break;

			float e2 = 2 * error;

			if (e2 >= height)
			{
				if (from.X == to.X)
					break;

				error += height;
				from.X += dirX;
			}

			if (e2 <= width)
			{
				if (from.Y == to.Y)
					break;

				error += width;
				from.Y += dirY;
			}
		}
	}

	public static bool ValidMovePosition(this ParticleGrid grid, Point position)
	{
		return grid.IsInsideBounds(position) && !grid.AnyParticle(position);
	}

	public static bool AnyValidMovePosition(this ParticleGrid grid, params Point[] positions)
	{
		for (int i = 0; i < positions.Length; i++)
		{
			if (grid.IsInsideBounds(positions[i]) && !grid.AnyParticle(positions[i]))
				return true;
		}
		return false;
	}

	public static bool AllValidMovePosition(this ParticleGrid grid, params Point[] positions)
	{
		for (int i = 0; i < positions.Length; i++)
		{
			if (!grid.IsInsideBounds(positions[i]) || grid.AnyParticle(positions[i]))
				return false;
		}
		return true;
	}

	public static void DrawParticleGrid(this SpriteBatch spriteBatch, ParticleGrid grid, float layerDepth = 0)
	{
		spriteBatch.Draw(
			grid.Texture,
			grid.Position,
			null,
			Color.White,
			0,
			Vector2.Zero,
			grid.Scale,
			SpriteEffects.None,
			layerDepth);
	}

	public static void Draw(this ParticleGrid grid, SpriteBatch spriteBatch, float layerDepth = 0)
	{
		spriteBatch.Draw(
			grid.Texture,
			grid.Position,
			null,
			Color.White,
			0,
			Vector2.Zero,
			grid.Scale,
			SpriteEffects.None,
			layerDepth);
	}

	public static bool IsTouching(this ParticleGrid grid, Point position, int particleId, int min = 1)
	{
		int touches = 0;
		Point up = position + new Point(0, -1);
		Point down = position + new Point(0, 1);
		Point right = position + new Point(1, 0);
		Point left = position + new Point(-1, 0);
		if (grid.IsInsideBounds(up) && grid.GetId(up) == particleId) 
			touches++;
		if (grid.IsInsideBounds(down) && grid.GetId(down) == particleId)
			touches++;
		if (grid.IsInsideBounds(right) && grid.GetId(right) == particleId)
			touches++;
		if (grid.IsInsideBounds(left) && grid.GetId(left) == particleId)
			touches++;
		return touches >= min;

	}

	public static bool IsSurrounded(this ParticleGrid grid, Point position, int particleId) => IsTouching(grid, position, particleId, 4);
}
