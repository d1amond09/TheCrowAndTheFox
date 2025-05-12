using Factory = SharpDX.DirectWrite.Factory;
using TheCrowAndTheFox.Engine;
using SharpDX.Direct2D1;
using System;

namespace TheCrowAndTheFox.HUD
{
	public class HeadsUpDisplay : IDisposable
	{
		private readonly Factory _factoryDWrite;
		private readonly GameManager _game;
		private bool disposedValue;

		public Health Health { get; set; }
		public Score Score { get; set; }

		public HeadsUpDisplay(GameManager game, Factory factoryDWrite)
		{
			_factoryDWrite = factoryDWrite;
			_game = game;

			Health = new Health(_game.Health);
			Score = new Score(_factoryDWrite, _game.Width);
		}

		public void Update(float dt)
		{
			Health.Update(dt, _game.Health);
			Score.Update(_game.Score);
		}

		public void Draw(RenderTarget renderTarget, BitmapLoader bitmapLoader, SolidColorBrush sceneColorBrush)
		{
			Health.Draw(renderTarget, bitmapLoader);
			Score.Draw(renderTarget, bitmapLoader, sceneColorBrush);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					Score?.Dispose();
				}
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
