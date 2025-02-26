﻿using System;

public class Smoke : IParticle, IDensity
{
	public int Density { get; init; } = 100;
	public float PassThroughChance { get; init; } = 0.9f;

	public int Lifetime { get; init; } = 600;

	public Color GetColor(ParticleGrid grid, Point position)
	{
		Color normalColor = Color.Lerp(new(64, 64, 64), new(128, 128, 128), grid.NoiseGrid(position));
		float alpha = (float)grid.Data(position, new SingleParticleData(Lifetime)).Data / Lifetime;
		alpha = MathF.Min(alpha * 2, 1f);
		return new Color(normalColor, alpha);
	}

	public void Update(ParticleGrid grid, Point position)
	{
		grid.UpdateSurroundingChunks(position);
		SingleParticleData data = grid.Data(position, new SingleParticleData(Lifetime));
		data.Data--;
		if (data.Data <= 0)
		{
			grid.DeleteParticle(position);
			return;
		}

		Point up = position + new Point(0, -1);
		Point upRight = position + new Point(1, -1);
		Point upLeft = position + new Point(-1, -1);
		Point right = position + new Point(1, 0);
		Point left = position + new Point(-1, 0);
		bool goRight = ParticleUtility.RandomBool();
		
		if (ParticleUtility.RandomBool(0.5f))
		{
			//if (grid.AnyValidMovePosition(up, upRight, upLeft, right, left))
			//	grid.UpdateSurroundingChunks(position);
			return;
		}

		if (grid.ValidMovePosition(up) && ParticleUtility.RandomBool(0.1f))
			grid.MoveParticle(position, up);
		else if (grid.ValidMovePosition(goRight ? upRight : upLeft))
			grid.MoveParticle(position, goRight ? upRight : upLeft);
		else if (grid.ValidMovePosition(upRight))
			grid.MoveParticle(position, upRight);
		else if (grid.ValidMovePosition(upLeft))
			grid.MoveParticle(position, upLeft);
		else if (grid.ValidMovePosition(goRight ? right : left))
			grid.MoveParticle(position, goRight ? right : left);
		else if (grid.ValidMovePosition(right))
			grid.MoveParticle(position, right);
		else if (grid.ValidMovePosition(left))
			grid.MoveParticle(position, left);
	}
}
