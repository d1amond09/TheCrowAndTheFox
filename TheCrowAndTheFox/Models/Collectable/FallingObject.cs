using TheCrowAndTheFox.Models.Common;
using TheCrowAndTheFox.Engine;
using SharpDX.Direct2D1;
using SharpDX;
using System;
using TheCrowAndTheFox.Models.Consts;

namespace TheCrowAndTheFox.Models
{
	public abstract class FallingObject : GameObject
	{
		private const float GRAVITY = 9f;
		private const float TERMINAL_VELOCITY = float.MaxValue;
		private const float START_VELOCITY = 0f;
		private const float ANGLE = (float)Math.PI * 2;
		private const float BOBBING_AMPLITUDE = 5f;
		private const float BOBBING_FREQUENCY_DIVISOR = 20f;

		private readonly float _directionRotate;

		public bool IsFalling { get; private set; }
		private float _velocityY;
		public float PositionFalling { get; set; }
		public float Rotation { get; private set; }
		public bool ShouldBeRemoved { get; private set; } = false;
		public void MarkForRemoval() { ShouldBeRemoved = true; }

		public FallingObject(float x, float y, Direction direction) : base(x, y, GameSizes.Default)
		{
			IsFalling = false;
			_velocityY = START_VELOCITY;
			Direction = direction;
			Random rnd = new Random();
			_directionRotate = rnd.Next(-1, 2);
			PositionFalling = rnd.NextFloat(20f, 900f);
		}

		public void SetPosition(float x, float y)
		{
			X = x;
			Y = y;
		}

		public void SetInitialDirection(Direction dir)
		{
			if (!IsFalling)
			{
				Direction = dir;
			}
		}

		public void Fall(float dt)
		{
			IsFalling = true;
			_velocityY += GRAVITY * dt;

			_velocityY = _velocityY > TERMINAL_VELOCITY 
				? TERMINAL_VELOCITY 
				: _velocityY;

			Y += _velocityY;
		}

		public void UpdateCarriedPosition(float newX, float newY)
		{
			if (!IsFalling)
			{
				X = newX;
				Y = newY;
			}
		}

		public void Rotate(float dt)
		{
			Rotation += ANGLE * _directionRotate * dt;
		}

		public void Rotate(float dt, float direction)
		{
			Rotation += ANGLE * direction * dt;
		}

		public override void Update(float dt)
		{
			if (!IsFalling)
			{
				bool shouldStartFalling = false;
				if (Direction == Direction.Left && X <= PositionFalling)
				{
					shouldStartFalling = true;
				}
				else if (Direction == Direction.Right && X >= PositionFalling)
				{
					shouldStartFalling = true;
				}

				if (shouldStartFalling)
				{
					IsFalling = true;
					Direction = Direction.None;
					CurrentSpeed = 0;          
				}
			}

			if (IsFalling)
			{
				Fall(dt);   
				Rotate(dt); 
			}
		}

		public void ForceFall()
		{
			if (!IsFalling)
			{
				IsFalling = true;
				Direction = Direction.None; 
				CurrentSpeed = 0;
				Console.WriteLine("Bomb forced to fall!");
			}
		}

		public override void Draw(RenderTarget renderTarget, BitmapLoader bitmapLoader, BitmapInterpolationMode interpolationMode)
		{
			base.Draw(renderTarget, bitmapLoader, interpolationMode);
		}
	}
}