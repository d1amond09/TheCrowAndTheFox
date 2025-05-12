using TheCrowAndTheFox.Engine;
using SharpDX.Direct2D1;
using SharpDX;
using TheCrowAndTheFox.Models.Common;

namespace TheCrowAndTheFox.Models
{
	public abstract class GameObject
	{
		public string Sprite { get; set; }
		public float Speed { get; protected set; }
		public float CurrentSpeed { get; protected set; }
		public float Width { get; protected set; }
		public float Height { get; protected set; }
		public float X { get; protected set; }
		public float Y { get; protected set; }
		public float SpawnX { get; private set; }
		public float SpawnY { get; private set; }
		public float CenterX => X + Width / 2;
		public float CenterY => Y + Height / 2;

		public Direction Direction { get; protected set; }

		public void MoveLeft() => Direction = Direction.Left;
		public void MoveRight() => Direction = Direction.Right;

		public GameObject(float x, float y, float size) : this(x, y, size, size) { }

		public GameObject(float x, float y, float width, float height)
		{
			X = x;
			Y = y;
			SpawnX = x;
			SpawnY = y;
			Width = width;
			Height = height;
			Speed = 200;
		}

		public virtual void Update(float dt)
		{
			HandleHorizontalMovement(dt);
			UpdatePosition(dt);
		}

		protected virtual void HandleHorizontalMovement(float dt)
		{
			float targetSpeed = 0f;

			if (Direction == Direction.Left)
			{
				targetSpeed = -Speed;
			}
			if (Direction == Direction.Right)
			{
				targetSpeed = Speed;
			}

			CurrentSpeed = targetSpeed;
		}

		public virtual void Stay()
		{
			CurrentSpeed = 0f;
			Direction = Direction.None;
		}

		protected virtual void UpdatePosition(float dt)
		{
			X += CurrentSpeed * dt;
		}

		public virtual void Draw(RenderTarget renderTarget, BitmapLoader bitmapLoader)
		{
			renderTarget.DrawBitmap(
				bitmapLoader.GetBitmap(renderTarget, Sprite),
				new RectangleF(X, Y, Width, Height),
				1.0f,
				BitmapInterpolationMode.Linear);
			renderTarget.Transform = Matrix3x2.Identity;
		}

		public virtual void Draw(RenderTarget renderTarget, BitmapLoader bitmapLoader, BitmapInterpolationMode interpolationMode)
		{
			if (string.IsNullOrEmpty(Sprite)) return;

			Bitmap bitmapToDraw = bitmapLoader.GetBitmap(renderTarget, Sprite);
			if (bitmapToDraw == null) return;

			Matrix3x2 originalTransform = renderTarget.Transform;
			if (this is FallingObject fallingObject && fallingObject.Rotation != 0)
			{
				var finalMatrix = Matrix3x2.Rotation(fallingObject.Rotation, new Vector2(CenterX, CenterY));
				renderTarget.Transform = finalMatrix * originalTransform;
			}

			renderTarget.DrawBitmap(
				bitmapToDraw,
				new RectangleF(X, Y, Width, Height),
				1.0f,
				interpolationMode
			);

			renderTarget.Transform = originalTransform;
		}

		public bool IsCollide(GameObject gameObject) =>
			X < gameObject.X + gameObject.Width &&
			X + Width > gameObject.X &&
			Y < gameObject.Y + gameObject.Height &&
			Y + Height > gameObject.Y;
	}
}