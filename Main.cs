﻿global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

public class Main : Game
{
	public static Main Instance { get; private set; }
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;

	public static Texture2D Pixel;

	Sand sandParticle = new();
	Solid solidParticle = new();
	Water waterParticle = new();
	DrawableParticleGrid grid;

	public static readonly bool IsDebug = false;
	public static readonly double FPS = 60;

	public Main()
	{
		Instance = this;
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
	}

	protected override void Initialize()
	{
		IsMouseVisible = true;
		IsFixedTimeStep = true;
		TargetElapsedTime = TimeSpan.FromSeconds(1.0 / FPS);
		Window.AllowAltF4 = true;
		Window.AllowUserResizing = true;

		_graphics.PreferredBackBufferHeight = 500;
		_graphics.PreferredBackBufferWidth = 500;
		_graphics.ApplyChanges();

		Pixel = new(GraphicsDevice, 1, 1);
		Pixel.SetData(new[] { Color.White });

		ParticleGrid.RegisterParticle(sandParticle);
		ParticleGrid.RegisterParticle(solidParticle);
		ParticleGrid.RegisterParticle(waterParticle);
		grid = new(new Vector2(0, 0), 5, 100, 100);
		DrawableParticleGrid.ParticleId = ParticleGrid.IdFromParticle(sandParticle);

		base.Initialize();
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);

		// TODO: use this.Content to load your game content here
	}

	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();


		if (Input.GetKeyDown(Keys.R))
		{
			grid.Clear();
			grid.Update();
		}

		if (Input.GetKeyDown(Keys.D1))
			DrawableParticleGrid.ParticleId = ParticleGrid.IdFromParticle(sandParticle);
		if (Input.GetKeyDown(Keys.D2))
			DrawableParticleGrid.ParticleId = ParticleGrid.IdFromParticle(waterParticle);
		if (Input.GetKeyDown(Keys.D3))
			DrawableParticleGrid.ParticleId = ParticleGrid.IdFromParticle(solidParticle);

		if (!Input.GetKey(Keys.Space) && IsDebug)
			return;

		grid.Update();
		
		Input._UpdatePrevInput();

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.White);

		_spriteBatch.Begin(
			blendState: BlendState.NonPremultiplied,
			samplerState: SamplerState.PointClamp);

		ParticleUtility.DrawParticleGrid(grid, _spriteBatch);

		_spriteBatch.End();

		base.Draw(gameTime);
	}
}
