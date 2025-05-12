using Rectangle = TheCrowAndTheFox.Models.Rectangle;
using Factory = SharpDX.DirectWrite.Factory;
using TheCrowAndTheFox.Models.Consts;
using TheCrowAndTheFox.Engine;
using SharpDX.DirectWrite;
using SharpDX.Direct2D1;
using SharpDX;
using System;

namespace TheCrowAndTheFox.HUD
{
	public class Score : IDisposable
	{
		private readonly int _width;
		private readonly int _iconSize;
		private readonly Factory _factory;
		private readonly TextFormat _textFormat;

		private bool disposedValue;
		private double _score;
		private TextLayout _textLayout;

		public Rectangle Icon { get; private set; }

		public Score(Factory factory, int width)
		{
			_factory = factory;
			_width = width;
			_iconSize = (int) GameSizes.Large;

			Icon = new Rectangle(_width - _iconSize, GameSizes.Offset, _iconSize)
			{
				Sprite = SpriteNames.Cheese
			};

			_textFormat = new TextFormat(factory, "Calibri", 54)
			{
				TextAlignment = TextAlignment.Trailing,
				ParagraphAlignment = ParagraphAlignment.Center
			};
		}

		public void Update(double score)
		{
			_score = score;
		}

		public void Draw(RenderTarget renderTarget2D, BitmapLoader bitmapLoader, SolidColorBrush sceneColorBrush)
		{
			_textLayout = new TextLayout(_factory, $"{_score}", _textFormat, _width, 100);
			
			Vector2 origin = new Vector2(-_iconSize, 0);
			renderTarget2D.DrawTextLayout(origin, _textLayout, sceneColorBrush, DrawTextOptions.None);

			Icon.Draw(renderTarget2D, bitmapLoader);

			_textLayout.Dispose();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_textFormat?.Dispose();
					_textLayout?.Dispose(); 
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
