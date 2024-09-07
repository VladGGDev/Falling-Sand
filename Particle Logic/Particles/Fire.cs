using System;

public class Fire : IParticle
{
	public int Lifetime { get; init; } = 45;

	public Color GetColor(ParticleGrid grid, Point position)
	{
		Color normalColor = Color.Lerp(new(255, 0, 0), new(255, 190, 31), grid.NoiseGrid(position));
		float lifePassed = (float)grid.Data(position, new SingleParticleData(Lifetime)).Data / Lifetime;
		lifePassed = MathF.Pow(lifePassed, 0.25f);
		return Color.Lerp(normalColor, new(128, 128, 128), 1 - lifePassed);
	}

	public void Update(ParticleGrid grid, Point position)
	{
		SingleParticleData data = grid.Data(position, new SingleParticleData(Lifetime));
		data.Data--;
		if (data.Data <= 0)
		{
			grid.DeleteParticle(position);
			if (ParticleUtility.RandomBool(0.25f))
				grid.CreateParticle(ParticleGrid.IdFromName("Smoke"), position);
			return;
		}

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

		if (grid.ValidMovePosition(up) && ParticleUtility.RandomBool(0.25f))
			grid.MoveParticle(position, up);
		else if (grid.ValidMovePosition(direction) && ParticleUtility.RandomBool(0.5f))
			grid.MoveParticle(position, direction);
	}
}
