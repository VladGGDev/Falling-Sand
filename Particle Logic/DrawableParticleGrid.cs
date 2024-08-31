using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel.Design;

// TODO
public class DrawableParticleGrid : ParticleGrid
{
	Point _mouseGridPos;
	Point _prevMouseGridPos;

	public static int ParticleId { get; set; } = 0;
	public static int BrushSize { get; set; } = 5;

	public DrawableParticleGrid(Vector2 position, float scale, int width, int height) : base(width, height)
	{
		Position = position;
		Scale = scale;
	}

	public override void Update()
	{
		// Brush size
		if (Input.GetKey(Keys.LeftShift) || Input.GetKey(Keys.LeftControl))
			BrushSize = Math.Clamp(BrushSize + (int)Input.MouseScrollDelta, 1, int.MaxValue);

		// Get mouse position
		Vector2 relMousePos = Position - Input.MousePosition;
		Vector2 roundedMousePos = Vector2.Floor(-relMousePos / Scale) * Scale;
		_mouseGridPos = (roundedMousePos / Scale).ToPoint();

		// Draw
		if (IsInsideBounds(_mouseGridPos))
		{
			bool erasing = Input.GetMouseButton(1);
			if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
			{
				if (Input.GetKey(Keys.LeftShift))
					DrawSquare(erasing);
				else if (Input.GetKey(Keys.LeftControl))
					DrawCircle(erasing);
				else
					ParticleUtility.DrawLine(this, erasing ? 0 : ParticleId, ClampInsideBounds(_prevMouseGridPos), _mouseGridPos);
			}
		}

		_prevMouseGridPos = _mouseGridPos;
		base.Update();
	}

	void DrawSquare(bool erasing)
	{
		for (int y = -BrushSize / 2; y < (int)Math.Ceiling((float)BrushSize / 2); y++)
		{
			for (int x = -BrushSize / 2; x < (int)Math.Ceiling((float)BrushSize / 2); x++)
			{
				Point offset = new(x, y);
				ParticleUtility.DrawLine(this, erasing ? 0 : ParticleId, ClampInsideBounds(_prevMouseGridPos + offset), ClampInsideBounds(_mouseGridPos + offset));
			}
		}
	}

	void DrawCircle(bool erasing)
	{
		for (int y = -BrushSize / 2; y < (int)Math.Ceiling((float)BrushSize / 2); y++)
		{
			for (int x = -BrushSize / 2; x < (int)Math.Ceiling((float)BrushSize / 2); x++)
			{
				if (x * x + y * y > (BrushSize / 2)*(BrushSize / 2))
					continue;
				Point offset = new(x, y);
				ParticleUtility.DrawLine(this, erasing ? 0 : ParticleId, ClampInsideBounds(_prevMouseGridPos + offset), ClampInsideBounds(_mouseGridPos + offset));
			}
		}
		ParticleUtility.DrawLine(this, erasing ? 0 : ParticleId, ClampInsideBounds(_prevMouseGridPos), _mouseGridPos);
	}
}
