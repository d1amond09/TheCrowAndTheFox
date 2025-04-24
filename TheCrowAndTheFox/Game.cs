using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using TheCrowAndTheFox.Engine;
using TheCrowAndTheFox.Models;
using Rectangle = TheCrowAndTheFox.Models.Rectangle;

namespace TheCrowAndTheFox
{
	public class Game
	{
		public double Score { get; set; }
		public double BestScore { get; set; }

		private Player _player;
		private Background _background;
		private GameObject _leftBorder;
		private GameObject _rightBorder;
		private List<GameObject> _gameObjects;
		private BitmapLoader _bitmapLoader;
		private Random _random;

		private bool _isPaused;

		public Game()
		{
			_player = new Player();
			_background = new Background();
			_leftBorder = new Rectangle(_background.X, _background.Y, .1f, _background.Height);
			_rightBorder = new Rectangle(_background.Width, _background.Y, .1f, _background.Height);
			_gameObjects = new List<GameObject> { _background, _player };
			_random = new Random();
			_bitmapLoader = new BitmapLoader();
			_isPaused = false;
		}

		public void Pause()
		{
			_isPaused = !_isPaused;
		}

		private void SpawnObject()
		{
			float x = _random.Next(-10, 1080);
			_gameObjects.Add(new Cheese(x));
		}


		public void PlayerControlKeyDown(KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.A:
					_player.MoveLeft();
					break;
				case Keys.D:
					_player.MoveRight();
					break;
				case Keys.Space:
					_player.Jump();
					break;
			}
		}

		public void PlayerControlKeyUp(KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.A:
					_player.StopMoveLeft();
					break;
				case Keys.D:
					_player.StopMoveRight();
					break;
			}
		}

		public void Update()
		{
			if (_isPaused) return;

			foreach(var obj in _gameObjects)
			{
				obj.Update();
			}

			var fallingObjects = _gameObjects.Where(g => g is Cheese).ToList();


			if (_leftBorder.IsCollide(_player))
			{
				_player.StopMoveLeft();
				_player.Stay();
			}
			if (_rightBorder.IsCollide(_player))
			{
				_player.StopMoveRight();
				_player.Stay();
			}

			if (fallingObjects.Any(o => o.IsCollide(_player)))
			{
				var objs = fallingObjects.FindAll(o => o.IsCollide(_player));
				foreach (var obj in objs)
				{
					_gameObjects.Remove(obj);
					Score++;
				}
			}

			fallingObjects.RemoveAll(obj => obj.Y > 720); 
			_gameObjects.RemoveAll(obj => obj.Y > 720); 
			

			if (fallingObjects.Count == 0 || fallingObjects[0].Y > 720) 
			{
				SpawnObject();
			}
			
		}

		public void Render(RenderTarget renderTarget)
		{
			if (_isPaused) return;

			foreach (var obj in _gameObjects)
			{
				obj.Draw(renderTarget);
				renderTarget.DrawBitmap(
					_bitmapLoader.GetBitmap(renderTarget, obj.Sprite), 
					new RectangleF(obj.X, obj.Y, obj.Width, obj.Height), 
					1.0f, 
					BitmapInterpolationMode.Linear);
				renderTarget.Transform = Matrix3x2.Identity;
			}
		}
	}
}