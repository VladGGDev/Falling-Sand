using System;
using System.Collections.Generic;

public class ParticleGrid
{
	static Random _rand = new();
	static Dictionary<int, IParticle> _particles = [];
	static Dictionary<IParticle, int> _ids = [];

	public Vector2 Position { get; set; } = Vector2.Zero;
	public float Scale { get; set; } = 1f;

	public int Width { get; }
	public int Height { get; }
	public Point Dimensions => new(Width, Height);
	int[,] _grid;
	bool[,] _updated;
	// TODO chunks
	public Texture2D Texture { get; private set; }

	public float[,] NoiseGrid { get; }

	public ParticleGrid(int width, int height)
	{
		Width = width;
		Height = height;

		Texture = new(Main.Instance.GraphicsDevice, width, height);

		// Grids are [y, x]
		_grid = new int[height, width];
		_updated = new bool[height, width];

		NoiseGrid = new float[height, width];
		
		for (int y = 0; y < height; y++)
			for (int x = 0; x < width; x++)
				NoiseGrid[y, x] = _rand.NextSingle();
	}



	#region Updating
	public virtual void Update()
	{
		// Starting from the bottom, going right and left alternatively
		for (int y = Height - 1; y >= 0; y--)
		{
			int startX, targetX, dx;
			if (y % 2 == 0)
			{
				startX = 0;
				targetX = Width - 1;
				dx = 1;
			}
			else
			{
				startX = Width - 1;
				targetX = 0;
				dx = -1;
			}

			for (int x = startX; (dx > 0 ? x <= targetX : x >= targetX); x += dx)
			{
				if (_grid[y, x] == 0) // Skip over empty particles
					continue;
				if (_updated[y, x]) // Don't update the same particle multiple times
					continue;

				IParticle current = ParticleFromId(_grid[y, x]);
				current.Update(this, new(x, y));
			}
		}
		Draw();
		ResetUpdated();
	}

	protected virtual void Draw()
	{
		Color[] pixels = new Color[Width * Height];
		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				if (_grid[y, x] == 0) // Skip over empty particles
					continue;
				IParticle current = ParticleFromId(_grid[y, x]);
				pixels[y * Width + x] = current.GetColor(this, new(x, y));
			}
		}
		Texture.SetData(pixels);
	}

	void ResetUpdated()
	{
		for (int y = 0; y < Height; y++)
			for (int x = 0; x < Width; x++)
				_updated[y, x] = false;
	}
	#endregion



	#region Static functions
	public static void RegisterParticle(IParticle particle, int id)
	{
		if (id == 0)
			throw new ArgumentException("Id cannot be 0");
		if (_particles.ContainsKey(id))
			throw new ArgumentException($"Id {id} is already registered.");
		_particles[id] = particle;
		_ids[particle] = id;
	}

	public static void RegisterParticle(IParticle particle)
	{
		int id;
		do
		{
			id = _rand.Next();
		} while (id == 0 || _particles.ContainsKey(id));

		_particles[id] = particle;
		_ids[particle] = id;
	}



	public static IParticle ParticleFromId(int id) => _particles[id];
	public static int IdFromParticle(IParticle particle) => _ids[particle];
	#endregion



	#region Particle grid manipulation
	public void CreateParticle(int particleId, Point poisition) => CreateParticle(particleId, poisition.X, poisition.Y);
	public void CreateParticle(int particleId, int x, int y)
	{
		_grid[y, x] = particleId;
	}

	public void DeleteParticle(Point position) => DeleteParticle(position.X, position.Y);
	public void DeleteParticle(int x, int y)
	{
		_grid[y, x] = 0;
	}

	public void MoveParticle(int x1, int y1, int x2, int y2) => MoveParticle(new(x1, y1), new(x2, y2));
	public void MoveParticle(Point from, Point to)
	{
		CreateParticle(_grid[from.Y, from.X], to.X, to.Y);
		DeleteParticle(from);
		_updated[to.Y, to.X] = true;
	}

	public void SwapParticle(Point a, Point b) => SwapParticle(a.X, a.Y, b.X, b.Y);
	public void SwapParticle(int x1, int y1, int x2, int y2)
	{
		int temp = _grid[y1, x1];
		_grid[y1, x1] = _grid[y2, x2];
		_grid[y2, x2] = temp;
		_updated[y1, x1] = _updated[y2, x2] = true;
	}

	public void Fill(IParticle particle) => Fill(IdFromParticle(particle));
	public void Fill(int id)
	{
		for (int y = 0; y < Height; y++)
			for (int x = 0; x < Width; x++)
				_grid[y, x] = id;
	}
	public void Clear() => Fill(0);



	public int GetId(Point position) => GetId(position.X, position.Y);
	public int GetId(int x, int y) => _grid[y, x];

	public IParticle GetParticle(Point position) => GetParticle(position.X, position.Y);
	public IParticle GetParticle(int x, int y)
	{
		if (_grid[y, x] == 0)
			return null;
		return ParticleFromId(_grid[y, x]);
	}

	public bool AnyParticle(Point position) => AnyParticle(position.X, position.Y);
	public bool AnyParticle(int x, int y)
	{
		return _grid[y, x] != 0;
	}

	public bool IsEmpty(Point position) => !AnyParticle(position.X, position.Y);
	public bool IsEmpty(int x, int y) => !AnyParticle(x, y);



	public bool IsInsideBounds(Point position) => IsInsideBounds(position.X, position.Y);
	public bool IsInsideBounds(int x, int y)
	{
		return x >= 0 && x < Width && y >= 0 && y < Height;
	}

	public Point ClampInsideBounds(Point position)
	{
		return new(Math.Clamp(position.X, 0, Width - 1), Math.Clamp(position.Y, 0, Height - 1));
	}
	public void ClampInsideBounds(ref int x, ref int y)
	{
		x = Math.Clamp(x, 0, Width - 1);
		y = Math.Clamp(y, 0, Height - 1);
	}
	#endregion
}
