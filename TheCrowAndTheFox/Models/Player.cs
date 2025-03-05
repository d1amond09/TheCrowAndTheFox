using SharpDX;

namespace TheCrowAndTheFox.Models
{
	public class Player : GameObject
	{
		private const float MAX_SPEED = 500f;
		private const float ACCELERATION = 100f;
		private const float DECELERATION = 10f;

		private float currentSpeed = 0f; 

		public Player() : base(0, 500, 60f, 50f)
		{
			Sprite = "fox-sprite.png";
		}

		public void MoveLeft() 
		{
			if (X > 0)
			{
				Accelerate(-ACCELERATION);
			}
		}

		public void MoveRight()
		{
			if (X < 1080 - Width)
			{
				Accelerate(ACCELERATION);
			}
		}

		private void Accelerate(float change)
		{
			currentSpeed += change;
			currentSpeed = MathUtil.Clamp(currentSpeed, -MAX_SPEED, MAX_SPEED);
			X += currentSpeed * Timer.DeltaTime;
		}

		public void Stop()
		{
			if (currentSpeed > 0)
			{
				currentSpeed -= DECELERATION;
				if (currentSpeed < 0) currentSpeed = 0;
			}
			else if (currentSpeed < 0)
			{
				currentSpeed += DECELERATION;
				if (currentSpeed > 0) currentSpeed = 0;
			}
			
			X += currentSpeed * Timer.DeltaTime;
		}

		public override void Update()
		{
			Stop();
		}

	}
}