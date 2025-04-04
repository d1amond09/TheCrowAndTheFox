using SharpDX;
using SharpDX.Direct2D1;
using TheCrowAndTheFox.Engine;

namespace TheCrowAndTheFox.Models
{
	public class Player : GameObject
	{
		private const float MAX_SPEED = 350f;
		private const float ACCELERATION = 30f;
		private const float DECELERATION = 5f;
		private const float JUMP_FORCE = 480f;
		private const float GRAVITY = 980f; 

		private float _currentSpeed = 0f;
		private float _verticalSpeed = 0f;

		private bool _isJumping = false; 
		private bool _isMovingLeft = false; 
		private bool _isMovingRight = false;

		private int _numSpriteLeftRun;
		private int _numSpriteLeftJump;
		private int _numSpriteRightRun;
		private int _numSpriteRightJump;
		private int _timeLeftRun;
		private int _timeLeftJump;
		private int _timeRightRun;
		private int _timeRightJump;

		public Player() : base(0, 500, 60f, 50f)
		{
			Sprite = "Fox/fox-run-0.png";
		}

		public void MoveLeft() => _isMovingLeft = true;
		public void StopMoveLeft() => _isMovingLeft = false;

		public void MoveRight() => _isMovingRight = true;
		public void StopMoveRight() => _isMovingRight = false;

		public void Jump()
		{
			if (!_isJumping)
			{
				_verticalSpeed = -JUMP_FORCE; 
				_isJumping = true;
				_numSpriteRightJump = 0;
			}
		}

		private void ApplyGravity()
		{
			if (_isJumping)
			{
				_verticalSpeed += GRAVITY * Timer.DeltaTime; 
				Y += _verticalSpeed * Timer.DeltaTime; 

				if (Y >= 500) 
				{
					Y = 500; 
					_isJumping = false; 
					_verticalSpeed = 0; 
				}
			}
		}

		private void Accelerate(float change)
		{
			_currentSpeed += change;
			_currentSpeed = MathUtil.Clamp(_currentSpeed, -MAX_SPEED, MAX_SPEED);
			X += _currentSpeed * Timer.DeltaTime;
		}

		public void Stop()
		{
			if (_currentSpeed > 0)
			{
				_currentSpeed -= DECELERATION;
				if (_currentSpeed < 0) _currentSpeed = 0;
			}
			else if (_currentSpeed < 0)
			{
				_currentSpeed += DECELERATION;
				if (_currentSpeed > 0) _currentSpeed = 0;
			}

			X += _currentSpeed * Timer.DeltaTime;
		}

		public override void Update()
		{
			Stop();
			ApplyGravity();

			if (_isMovingLeft && X > 0)
			{
				Accelerate(-ACCELERATION);
			}

			if (_isMovingRight && X < 1080 - Width)
			{
				Accelerate(ACCELERATION);
			}
		}

		public override void Draw(RenderTarget renderTarget)
		{
			if (_currentSpeed > 0 && _verticalSpeed == 0)
			{
				UpdateSprite(ref _numSpriteRightRun, "Fox/fox-run-", ref _timeRightRun, 8, 8);
			}

			if (_currentSpeed < 0 && _verticalSpeed == 0)
			{
				UpdateSprite(ref _numSpriteLeftRun, "Fox/fox-run-left-", ref _timeLeftRun, 8, 8);
			}

			if (_currentSpeed == 0 && _verticalSpeed == 0)
			{
				UpdateSprite(ref _numSpriteRightRun, "Fox/fox-idle-", ref _timeRightRun, 8, 5);
			}

			if (_verticalSpeed != 0)
			{
				UpdateSprite(ref _numSpriteRightJump, "Fox/fox-jump-", ref _timeRightJump, 20, 7);
			}

			base.Draw(renderTarget);
		}

		private void UpdateSprite(ref int spriteNum, string prefix, ref int timeCounter, int time, int countOfSprites)
		{
			if (timeCounter >= time)
			{
				Sprite = $"{prefix}{spriteNum % countOfSprites}.png";
				spriteNum++;
				timeCounter = 0;
			}

			if(spriteNum >= countOfSprites)
			{
				spriteNum = 0;
			}

			timeCounter++;
		}

	}
}