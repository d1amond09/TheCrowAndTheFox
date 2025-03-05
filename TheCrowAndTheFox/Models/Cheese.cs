using SharpDX.Direct2D1;
using SharpDX;
using System;

namespace TheCrowAndTheFox.Models
{
	public class Cheese : GameObject
	{
		private const float GRAVITY = 9.8f / 2;
		private const float TERMINAL_VELOCITY = float.MaxValue; 
		private const float START_VELOCITY = 0f; 
		private const float ANGLE = (float) Math.PI * 2;
		
		private float _velocityY;

		private float _rotation;
		private readonly float _direction;


		public float CenterX => X + Width / 2;
		public float CenterY => Y + Height / 2;

		public Cheese(float x) : base(x, 0, 50f)
		{
			Sprite = "cheese.png";
			_velocityY = START_VELOCITY;

			Random rnd = new Random();
			_direction = rnd.Next(-1, 2) > 0 ? 1 : -1;
		}

		public void Fall()
		{
			_velocityY += GRAVITY * Timer.DeltaTime;

			_velocityY = _velocityY > TERMINAL_VELOCITY ?
				TERMINAL_VELOCITY : _velocityY;

			Y += _velocityY;
		}

		public void Rotate()
		{
			_rotation += ANGLE * _direction * Timer.DeltaTime;
		}

		public override void Update()
		{
			Fall();
			Rotate();
		}

		public override void Draw(RenderTarget renderTarget)
		{
			var finalMatrix = Matrix3x2.Rotation(_rotation, new Vector2(CenterX, CenterY));
			renderTarget.Transform = finalMatrix;
		}
	}
}