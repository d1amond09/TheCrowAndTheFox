using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using TheCrowAndTheFox.Engine;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace TheCrowAndTheFox.Models
{
	public abstract class GameObject
	{
		public string Sprite { get; protected set; }
		public float Width { get; protected set; }
		public float Height { get; protected set; }
		public float X { get; protected set; }
		public float Y { get; protected set; }

		public GameObject(float x, float y, float size) : this(x, y, size, size) { }	
		
		public GameObject(float x, float y, float width, float height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public virtual void Update()
		{

		}


		public virtual void Draw(RenderTarget renderTarget)
		{
			
		}


		public bool IsCollide(GameObject gameObject) =>
			X < gameObject.X + gameObject.Width &&
			X + Width > gameObject.X &&
			Y < gameObject.Y + gameObject.Height &&
			Y + Height > gameObject.Y;
	}
}
