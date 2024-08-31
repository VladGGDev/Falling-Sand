using System;

public static class ParticleGridExtensions
{
	public static float NoiseGrid(this ParticleGrid grid, Point position) => grid.NoiseGrid[position.Y, position.X];

	public static void DrawLine(this ParticleGrid grid, int id, Point from, Point to)
	{
		int width = Math.Abs(to.X - from.X);
		int height = -Math.Abs(to.Y - from.Y);

		int dirX = from.X < to.X ? 1 : -1;
		int dirY = from.Y < to.Y ? 1 : -1;

		float error = width + height;


		while (true)
		{
			grid.CreateParticle(id, from.X, from.Y);

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
}
