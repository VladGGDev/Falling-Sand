using System;
using System.Collections.Generic;

public class ParticleGrid
{
	static Random _rand = new();
	static Dictionary<int, IParticle> _particles = new() { [0] = null };
	static Dictionary<IParticle, int> _ids = []; // new() { [null] = 0 };
	static Dictionary<string, int> _names = new() { ["Air"] = 0 };

	public Vector2 Position { get; set; } = Vector2.Zero;
	public float Scale { get; set; } = 1f;

	public Texture2D Texture { get; private set; }

	public int Width { get; }
	public int Height { get; }
	public Point Dimensions => new(Width, Height);
	int[,] _grid;
	bool[,] _updated;

	public int ChunkSize { get; }
	public int ChunksX { get; }
	public int ChunksY { get; }
	bool[,] _chunks;
	bool[,] _chunksLastFrame;
	public static bool ChunkDebug { get; set; } = false;

	public float[,] NoiseGrid { get; }


	public ParticleGrid(int width, int height, int chunkSize = 8)
	{
		Width = width;
		Height = height;

		Texture = new(Main.Instance.GraphicsDevice, width, height);

		// Grids are [y, x]
		_grid = new int[height, width];
		_updated = new bool[height, width];
		ChunkSize = chunkSize;
		ChunksX = width / chunkSize + 1;
		ChunksY = height / chunkSize + 1;
		_chunks = new bool[ChunksY, ChunksX];
		_chunksLastFrame = new bool[ChunksY, ChunksX];

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
				if (!IsChunkUpdated(x, y))
					continue;
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
				{
					if (ChunkDebug && !IsChunkUpdated(x, y))
						pixels[y * Width + x] = Color.Gray;
					continue;
				}
				IParticle current = ParticleFromId(_grid[y, x]);
				Color col = current.GetColor(this, new(x, y));
				pixels[y * Width + x] = 
					ChunkDebug && !IsChunkUpdated(x, y) ? 
					Color.Lerp(col, Color.Gray, 0.5f) : 
					col;
			}
		}
		Texture.SetData(pixels);
	}

	void ResetUpdated()
	{
		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
				_updated[y, x] = false;
		}
		for (int y = 0; y < ChunksY; y++)
		{
			for (int x = 0; x < ChunksX; x++)
			{
				_chunksLastFrame[y, x] = _chunks[y, x];
				_chunks[y, x] = false;
			}
		}
	}
	#endregion



	#region Static functions
	public static void RegisterParticle(IParticle particle, string name, int id)
	{
		if (id == 0)
			throw new ArgumentException("Id cannot be 0");
		if (_particles.ContainsKey(id))
			throw new ArgumentException($"Id {id} is already registered.");
		if (_names.ContainsKey(name))
			throw new ArgumentException($"Name {name} is already registered.");
		_particles[id] = particle;
		_names[name] = id;
		_ids[particle] = id;
	}

	public static void RegisterParticle(IParticle particle, string name)
	{
		int id;
		do
		{
			id = _rand.Next();
		} while (id == 0 || _particles.ContainsKey(id));

		_particles[id] = particle;
		_names[name] = id;
		_ids[particle] = id;
	}



	public static IParticle ParticleFromId(int id) => _particles[id];
	public static int IdFromParticle(IParticle particle)
	{
		if (particle == null)
			return 0;
		return _ids[particle];
	}
	public static int IdFromName(string name) => _names[name];
	public static IParticle ParticleFromName(string name) => ParticleFromId(_names[name]);
	#endregion



	#region Chunks
	protected Point PositionToChunk(Point position) => PositionToChunk(position.X, position.Y);
	protected Point PositionToChunk(int x, int y)
	{
		return new Point(x / ChunkSize, y / ChunkSize);
	}
	protected void PositionToChunk(Point position, out Point chunkPosition)
	{
		chunkPosition = new(position.X / ChunkSize, position.Y / ChunkSize);
	}
	protected void PositionToChunk(int x, int y, out int chunkX, out int chunkY)
	{
		chunkX = x / ChunkSize;
		chunkY = y / ChunkSize;
	}


	protected ref bool PositionToChunkRef(int x, int y) => ref _chunks[y / ChunkSize, x / ChunkSize];
	protected ref bool PositionToChunkLastFrameRef(int x, int y) => ref _chunksLastFrame[y / ChunkSize, x / ChunkSize];


	protected void UpdateChunk(Point position) => UpdateChunk(position.X, position.Y);
	protected void UpdateChunk(int x, int y)
	{
		PositionToChunkRef(x, y) = true;
	}

	public void UpdateSurroundingChunks(Point position) => UpdateSurroundingChunks(position.X, position.Y);
	public void UpdateSurroundingChunks(int x, int y)
	{
		PositionToChunk(x, y, out int chunkX, out int chunkY);
		UpdateChunk(x, y); // Center
		if (chunkX - 1 >= 0)
			UpdateChunk(x - 1, y); // Left
		if (chunkX + 1 < ChunksX)
			UpdateChunk(x + 1, y); // Right
		if (chunkY - 1 >= 0)
			UpdateChunk(x, y - 1); // Up
		if (chunkY + 1 < ChunksY)
			UpdateChunk(x, y + 1); // Down
	}


	public bool IsChunkUpdated(Point position) => IsChunkUpdated(position.X, position.Y);
	public bool IsChunkUpdated(int x, int y)
	{
		return PositionToChunkRef(x, y) || PositionToChunkLastFrameRef(x, y);
	}
	#endregion



	#region Particle grid manipulation
	public void CreateParticle(int particleId, Point poisition) => CreateParticle(particleId, poisition.X, poisition.Y);
	public void CreateParticle(int particleId, int x, int y)
	{
		_grid[y, x] = particleId;
	}
	public void CreateParticle(int particleId, Point poisition, bool updateChunk = false) => CreateParticle(particleId, poisition.X, poisition.Y, updateChunk);
	public void CreateParticle(int particleId, int x, int y, bool updateChunk = false)
	{
		_grid[y, x] = particleId;
		if (updateChunk)
			UpdateSurroundingChunks(x, y);
	}

	public void DeleteParticle(Point position) => DeleteParticle(position.X, position.Y);
	public void DeleteParticle(int x, int y)
	{
		_grid[y, x] = 0;
		UpdateSurroundingChunks(x, y);
	}

	public void MoveParticle(int x1, int y1, int x2, int y2) => MoveParticle(new(x1, y1), new(x2, y2));
	public void MoveParticle(Point from, Point to)
	{
		CreateParticle(_grid[from.Y, from.X], to.X, to.Y, true);
		DeleteParticle(from);
		_updated[to.Y, to.X] = true;
		//UpdateSurroundingChunks(to.X, to.Y);
	}

	public void SwapParticle(Point a, Point b) => SwapParticle(a.X, a.Y, b.X, b.Y);
	public void SwapParticle(int x1, int y1, int x2, int y2)
	{
		int temp = _grid[y1, x1];
		_grid[y1, x1] = _grid[y2, x2];
		_grid[y2, x2] = temp;
		_updated[y1, x1] = _updated[y2, x2] = true;
		UpdateSurroundingChunks(x2, y2);
	}

	public void Fill(IParticle particle) => Fill(IdFromParticle(particle));
	public void Fill(int id)
	{
		for (int y = 0; y < Height; y++)
			for (int x = 0; x < Width; x++)
				CreateParticle(id, x, y, true);
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
