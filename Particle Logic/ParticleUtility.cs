using System;

public static class ParticleUtility
{
	static Random _rand = new();

	public static int RandomRange(int minInclusive, int maxExclusive) => _rand.Next(minInclusive, maxExclusive);
	public static float RandomValue() => _rand.NextSingle();
	public static bool RandomBool(float trueChance = 0.5f) => _rand.NextSingle() < trueChance;

	public static float NoiseGrid(ParticleGrid grid, Point position) => grid.NoiseGrid[position.Y, position.X];

	public static void DrawLine(ParticleGrid grid, int id, Point from, Point to)
	{
		//if (from == to)
		//{
		//	grid.CreateParticle(id, from);
		//	return;
		//}

		//int dx = from.X < to.X ? 1 : -1;
		//int dy = from.Y < to.Y ? 1 : -1;

		//float upError = 0;
		//float tan;
		//if (from.Y == to.Y)
		//	tan = 0;
		//else if (from.X == to.X)
		//	tan = float.PositiveInfinity;
		//else
		//	tan = Math.Abs(to.Y - from.Y) / Math.Abs(to.X - from.X);

		//int x, y;
		//from.Deconstruct(out x, out y);
		//while (x != to.X)
		//{
		//	bool drawn = false;
		//	while (upError >= 1)
		//	{
		//		upError--;
		//		y += dy;
		//		grid.CreateParticle(id, x, y);
		//		drawn = true;
		//	}
		//	if (!drawn)
		//		grid.CreateParticle(id, x, y);
		//	upError += tan;
		//	x += dx;
		//}




		// Alternative =======================================
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

	public static bool ValidMovePosition(ParticleGrid grid, Point position)
	{
		return grid.IsInsideBounds(position) && !grid.AnyParticle(position);
	}

	public static void DrawParticleGrid(ParticleGrid grid, SpriteBatch spriteBatch, float layerDepth = 0)
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
