using Rectangle = TheCrowAndTheFox.Models.Rectangle;
using TheCrowAndTheFox.Models.Common;
using System.Collections.Generic;
using TheCrowAndTheFox.Engine;
using TheCrowAndTheFox.Models;
using System.Windows.Forms;
using SharpDX.Direct2D1;
using System.Linq;
using SharpDX;
using System;
using TheCrowAndTheFox.Interfaces;
using TheCrowAndTheFox.Models.Consts;
using System.IO;
using TheCrowAndTheFox.Audio;

namespace TheCrowAndTheFox
{
	public class GameManager : IDisposable
	{
		public BitmapInterpolationMode CurrentBitmapInterpolationMode { get; private set; } = BitmapInterpolationMode.Linear;
		private Player _player;
		private List<GameObject> _gameObjects;
		private IAudioManager _audioManager; 
		private readonly string _cheeseSoundPath = Path.Combine(Application.StartupPath, "Assets", "Music", "collect.mp3");
		private readonly string _gameOverPath = Path.Combine(Application.StartupPath, "Assets", "Music", "game-over.mp3");

		private bool _isPausedInternal;

		private float _timeSinceLastCrowSpawn = 0f;
		private float _currentCrowSpawnInterval = 3.0f; 
		private const float MIN_CROW_SPAWN_INTERVAL = 1.0f;
		private const float SPAWN_INTERVAL_DECREASE_PER_SCORE_TIER = 0.25f;
		private const int SCORE_TIER_FOR_DIFFICULTY_INCREASE = 5;

		private float _currentCrowBaseSpeed = 200f;
		private const float MAX_CROW_BASE_SPEED = 350f;
		private const float CROW_SPEED_INCREASE_PER_SCORE_TIER = 15f;

		private double _bombProbability = 0.35; 
		private const double MAX_BOMB_PROBABILITY = 0.65; 
		private const double BOMB_PROBABILITY_INCREASE_PER_SCORE_TIER = 0.05;

		private int _maxConcurrentCrows = 2;
		private const int ABSOLUTE_MAX_CROWS = 6; 
		private const int MAX_CROWS_INCREASE_PER_SCORE_TIER = 1;

		private const int SCORE_TIER_FOR_MAX_CROWS_INCREASE = 10;
		private int _lastScoreTierForMaxCrowsChecked = 0;

		private int _lastScoreTierChecked = 0;

		private Random _random = new Random();

		public bool IsGameOver { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public double Score { get; set; }
		public int Health { get; set; }
		public double BestScore { get; set; }


		public GameManager(int width, int height, IAudioManager audioManager)
		{
			Width = width;
			Height = height;
			_audioManager = audioManager;
			InitializeNewGame();
		}

		private void InitializeNewGame()
		{
			_player = new Player(Width, 3, _audioManager);
			Background background = new Background(Width, Height);
			_gameObjects = new List<GameObject> { background, _player };

			IsGameOver = false;
			Score = 0;
			Health = _player.Health;

			_timeSinceLastCrowSpawn = 0f; 
			_currentCrowSpawnInterval = 3.0f;
			_currentCrowBaseSpeed = 200f;
			_bombProbability = 0.35;
			_lastScoreTierChecked = 0;

			SpawnCrow();
		}


		public void SetGraphicsQuality(string qualityLevel)
		{
			switch (qualityLevel)
			{
				case "Low":
					CurrentBitmapInterpolationMode = BitmapInterpolationMode.NearestNeighbor;
					break;
				case "Ultra":
				case "High":
				case "Medium":
				default:
					CurrentBitmapInterpolationMode = BitmapInterpolationMode.Linear;
					break;
			}
		}


		private void UpdateDifficulty()
		{
			int currentScoreTierGeneral = (int)(Score / SCORE_TIER_FOR_DIFFICULTY_INCREASE);
			if (currentScoreTierGeneral > _lastScoreTierChecked)
			{
				_currentCrowSpawnInterval = Math.Max(MIN_CROW_SPAWN_INTERVAL, _currentCrowSpawnInterval - SPAWN_INTERVAL_DECREASE_PER_SCORE_TIER);
				_currentCrowBaseSpeed = Math.Min(MAX_CROW_BASE_SPEED, _currentCrowBaseSpeed + CROW_SPEED_INCREASE_PER_SCORE_TIER);
				_bombProbability = Math.Min(MAX_BOMB_PROBABILITY, _bombProbability + BOMB_PROBABILITY_INCREASE_PER_SCORE_TIER);
				_lastScoreTierChecked = currentScoreTierGeneral;
				Console.WriteLine($"Difficulty Increased (General): SpawnInt: {_currentCrowSpawnInterval:F2}s, CrowSpeed: {_currentCrowBaseSpeed:F0}, BombProb: {_bombProbability:P0}");
			}

			int currentScoreTierForMaxCrows = (int)(Score / SCORE_TIER_FOR_MAX_CROWS_INCREASE);
			if (currentScoreTierForMaxCrows > _lastScoreTierForMaxCrowsChecked)
			{
				if (_maxConcurrentCrows < ABSOLUTE_MAX_CROWS)
				{
					_maxConcurrentCrows += MAX_CROWS_INCREASE_PER_SCORE_TIER; 
					_maxConcurrentCrows = Math.Min(_maxConcurrentCrows, ABSOLUTE_MAX_CROWS); 
					Console.WriteLine($"Difficulty Increased (Max Crows): Now {_maxConcurrentCrows}");
				}
				_lastScoreTierForMaxCrowsChecked = currentScoreTierForMaxCrows;
			}
		}


		public void SetPaused(bool isPaused)
		{
			_isPausedInternal = isPaused;
		}

		private void SpawnCrow()
		{
			bool isLeft = _random.Next(0, 2) == 0; 
			float offset = (float)_random.NextDouble() * (GameSizes.XXXL - GameSizes.XL) + GameSizes.XL; // Random offset

			float crowY = (float)_random.NextDouble() * (Height * 0.4f - GameSizes.Large) + GameSizes.Medium; // Spawn in upper 40% of screen

			Direction dir = isLeft ? Direction.Left : Direction.Right;
			float x = isLeft ? Width + offset : -offset; 

			FallingObject carriedObject = null;
			double itemRoll = _random.NextDouble();
			if (itemRoll < _bombProbability) 
			{
				carriedObject = new Bomb(0, 0, dir); 
			}
			else if (itemRoll < _bombProbability + 0.55)
			{
				carriedObject = new Cheese(0, 0, dir);
			}


			Crow crow = new Crow(x, crowY, dir, _currentCrowBaseSpeed, carriedObject);
			_gameObjects.Add(crow);
			if (crow.FallingObject != null) 
			{
				_gameObjects.Add(crow.FallingObject);
			}
		}


		public void PlayerControlKeyDown(KeyEventArgs e)
		{
			if (IsGameOver || _isPausedInternal) return; 
			_player.PlayerControlKeyDown(e); 
		}

		public void PlayerControlKeyUp(KeyEventArgs e)
		{
			if (IsGameOver || _isPausedInternal) return;
			_player.PlayerControlKeyUp(e); 
		}

		public void Update(float dt)
		{
			if (IsGameOver || _isPausedInternal) return;

			UpdateDifficulty();


			_timeSinceLastCrowSpawn += dt;
			int currentCrowCount = _gameObjects.Count(obj => obj is Crow);
			if (currentCrowCount < _maxConcurrentCrows && _timeSinceLastCrowSpawn >= _currentCrowSpawnInterval)
			{
				SpawnCrow();
				_timeSinceLastCrowSpawn = 0f; 
			}

			foreach (var obj in _gameObjects.ToList()) 
			{
				obj.Update(dt);
				
				if (obj is Crow crow)
				{
					crow.FindPlayerReference(_gameObjects); 
				}

				if (obj is ICollectable)
				{
					if (obj is Cheese cheese && cheese.IsCollide(_player))
					{
						Score++;
						_audioManager?.PlaySoundEffect(_cheeseSoundPath);
						cheese.MarkForRemoval();
					}
					if (obj is Bomb bomb && bomb.IsCollide(_player))
					{
						_player.TakeDamage();
						bomb.MarkForRemoval();
					}
				}
			}

			Health = _player.Health;
			if (Health <= 0 && !IsGameOver) 
			{
				IsGameOver = true;
				_audioManager?.PlaySoundEffect(_gameOverPath);
				return;
			}

			_gameObjects.RemoveAll(obj =>
				obj.Y > Height + 200
				|| obj.X > Width + 300f
				|| obj.X < -300f
				|| (obj is FallingObject fallingObject && fallingObject.ShouldBeRemoved) 
			);

			if (_gameObjects.Count(g => g is Crow) < 2) 
			{
				SpawnCrow();
			}
		}

		public void Render(RenderTarget renderTarget, BitmapLoader bitmapLoader)
		{
			foreach (var obj in _gameObjects)
			{
				obj.Draw(renderTarget, bitmapLoader, CurrentBitmapInterpolationMode);
			}
		}

		public void Dispose()
		{
			
		}
	}
}