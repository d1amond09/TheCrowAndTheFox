using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Samples;
using TheCrowAndTheFox.Models;

namespace TheCrowAndTheFox
{
	public class Game
	{
		public int Score { get; set; }
		public int BestScore { get; set; }

		private Player _player;
		private Background _background;
		private List<GameObject> _gameObjects;
		private Random _random;

		private Dictionary<string, Bitmap> _bitmaps = new Dictionary<string, Bitmap>();

		public static Bitmap LoadFromFile(RenderTarget renderTarget, string file)
		{
			using (var bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromFile($"Assets/Textures/{file}"))
			{
				var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
				var bitmapProperties = new BitmapProperties(new SharpDX.Direct2D1.PixelFormat(Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied));
				var size = new Size2(bitmap.Width, bitmap.Height);

				int stride = bitmap.Width * sizeof(int);
				using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
				{
					var bitmapData = bitmap.LockBits(sourceArea, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

					for (int y = 0; y < bitmap.Height; y++)
					{
						int offset = bitmapData.Stride * y;
						for (int x = 0; x < bitmap.Width; x++)
						{
							byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
							byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
							byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
							byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
							int rgba = R | (G << 8) | (B << 16) | (A << 24);
							tempStream.Write(rgba);
						}

					}
					bitmap.UnlockBits(bitmapData);
					tempStream.Position = 0;

					return new Bitmap(renderTarget, size, tempStream, stride, bitmapProperties);
				}
			}
		}

		public Bitmap GetBitmap(RenderTarget renderTarget, string file)
		{
			if (_bitmaps.TryGetValue(file, out Bitmap bitmap))
			{
				return bitmap;
			}
			else
			{
				bitmap = LoadFromFile(renderTarget, file);
				_bitmaps.Add(file, bitmap);
				return bitmap;
			}
		}


		public Game(RenderTarget renderTarget2D)
		{
			_player = new Player();
			_background = new Background();
			_gameObjects = new List<GameObject> { _background, _player };
			_random = new Random();
			SpawnObject();
		}

		private void SpawnObject()
		{
			float x = _random.Next(-10, 1080);
			_gameObjects.Add(new Cheese(x));
		}


		public void PlayerControl(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.A)
			{
				_player.MoveLeft();
			}
			else if (e.KeyCode == Keys.D)
			{
				_player.MoveRight();
			}
		}

		public void Update()
		{
			foreach(var obj in _gameObjects)
			{
				obj.Update();
			}

			var fallingObjects = _gameObjects.Where(g => g is Cheese).ToList();

			
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
			foreach (var obj in _gameObjects)
			{
				obj.Draw(renderTarget);
				renderTarget.DrawBitmap(
					GetBitmap(renderTarget, obj.Sprite), 
					new RectangleF(obj.X, obj.Y, obj.Width, obj.Height), 
					1.0f, 
					BitmapInterpolationMode.Linear);
				renderTarget.Transform = Matrix3x2.Identity;
			}
		}
	}
}