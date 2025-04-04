using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;
using SharpDX;
using SharpDX.DXGI;

namespace TheCrowAndTheFox.Engine
{
    class BitmapLoader
    {

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
	}
}
