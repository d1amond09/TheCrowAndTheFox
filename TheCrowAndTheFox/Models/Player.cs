using System;
using SharpDX;
using SharpDX.Direct2D1;
using TheCrowAndTheFox.Engine;

namespace TheCrowAndTheFox.Models
{
	public class Player : GameObject
	{
		private const float MAX_SPEED = 250f;
		private const float ACCELERATION = 1500f;
		private const float DECELERATION_FORCE = 1000f;
		private const float JUMP_VELOCITY = 750f;
		private const float GRAVITY = 2000f;
		private const float SIZE = 50f;
		private const float GROUND_LEVEL = 650f;
		private const float SCREEN_WIDTH = 1080f;

		private const int COUNT_SPRITES_JUMP = 7;
		private const int COUNT_SPRITES_IDLE = 5;
		private const int COUNT_SPRITES_RUN = 8;

		private const float ANIM_FRAME_DURATION_RUN = 0.08f;
		private const float ANIM_FRAME_DURATION_IDLE = 0.15f;
		private const float ANIM_FRAME_DURATION_JUMP = 0.1f;

		private float _currentHorizontalSpeed = 0f;
		private float _verticalSpeed = 0f;

		private bool _isMovingLeft = false;
		private bool _isMovingRight = false;
		private bool _isGround = false;

		private int _currentSpriteIndex = 0;
		private float _animationTimer = 0f;
		private bool _facingRight = true;

		public Player() : base(100, GROUND_LEVEL - SIZE, SIZE, SIZE)
		{
			_isGround = true;
			Sprite = GetSpritePath("Fox/fox-idle-", 0); 
		}

		public void MoveLeft() => _isMovingLeft = true;
		public void StopMoveLeft() => _isMovingLeft = false;

		public void MoveRight() => _isMovingRight = true;
		public void StopMoveRight() => _isMovingRight = false;

		public void Jump()
		{
			if (_isGround)
			{
				_verticalSpeed = -JUMP_VELOCITY;
				_isGround = false;
			}
		}

		public override void Update()
		{
			float dt = Timer.DeltaTime;

			HandleHorizontalMovement(dt);
			ApplyGravity(dt);
			UpdatePosition(dt);
			HandleCollisions();
			UpdateSpriteState(dt);
		}

		private void HandleHorizontalMovement(float dt)
		{
			float targetSpeed = 0f;

			if (_isMovingLeft)
			{
				targetSpeed = -MAX_SPEED;
				_facingRight = false;
			}
			if (_isMovingRight)
			{
				targetSpeed = MAX_SPEED;
				_facingRight = true;
			}

			if (targetSpeed != 0)
			{
				float acceleration = ACCELERATION * dt;
				if (targetSpeed > 0)
				{
					_currentHorizontalSpeed = Math.Min(_currentHorizontalSpeed + acceleration, MAX_SPEED);
				}
				else
				{
					_currentHorizontalSpeed = Math.Max(_currentHorizontalSpeed - acceleration, -MAX_SPEED);
				}
			}
			else
			{
				float deceleration = DECELERATION_FORCE * dt;
				if (_currentHorizontalSpeed > 0)
				{
					_currentHorizontalSpeed = Math.Max(0, _currentHorizontalSpeed - deceleration);
				}
				else if (_currentHorizontalSpeed < 0)
				{
					_currentHorizontalSpeed = Math.Min(0, _currentHorizontalSpeed + deceleration);
				}
			}
		}

		public void Stay()
		{
			_currentHorizontalSpeed = 0f;
			_isMovingLeft = false;
			_isMovingRight = false;
		}

		private void ApplyGravity(float dt)
		{
			if (!_isGround)
			{
				_verticalSpeed += GRAVITY * dt;
			}
		}

		private void UpdatePosition(float dt)
		{
			X += _currentHorizontalSpeed * dt;
			Y += _verticalSpeed * dt;
			X = MathUtil.Clamp(X, 0, SCREEN_WIDTH - Width);
		}

		private void HandleCollisions()
		{
			if (Y + Height >= GROUND_LEVEL && _verticalSpeed >= 0)
			{
				Y = GROUND_LEVEL - Height;
				_verticalSpeed = 0;
				_isGround = true;
			}
			else if (Y + Height < GROUND_LEVEL) 
			{	
				_isGround = false;
			}
		}

		private void UpdateSpriteState(float dt)
		{
			string animPrefix;
			int frameCount;
			float frameDuration;
			bool loop = true;
			string currentStatePrefix; 

			if (!_isGround)
			{
				currentStatePrefix = _facingRight ? "Fox/fox-jump-" : "Fox/fox-jump-left-";
				frameCount = COUNT_SPRITES_JUMP;
				frameDuration = ANIM_FRAME_DURATION_JUMP;
				loop = true;
			}
			else if (Math.Abs(_currentHorizontalSpeed) > 0.1f)
			{
				currentStatePrefix = _facingRight ? "Fox/fox-run-" : "Fox/fox-run-left-";
				frameCount = COUNT_SPRITES_RUN;
				frameDuration = ANIM_FRAME_DURATION_RUN;
			}
			else
			{
				currentStatePrefix = _facingRight ? "Fox/fox-idle-" : "Fox/fox-idle-left-";
				frameCount = COUNT_SPRITES_IDLE;
				frameDuration = ANIM_FRAME_DURATION_IDLE;
			}

			animPrefix = currentStatePrefix; 

			if (Sprite == null || !Sprite.StartsWith(animPrefix))
			{
				_currentSpriteIndex = 0;
				_animationTimer = 0f;
				Sprite = GetSpritePath(animPrefix, _currentSpriteIndex);
			}


			_animationTimer += dt;

			if (_animationTimer >= frameDuration)
			{
				_animationTimer -= frameDuration; 
				_currentSpriteIndex++;

				if (_currentSpriteIndex >= frameCount)
				{
					if (loop)
					{
						_currentSpriteIndex = 0;
					}
					else
					{
						_currentSpriteIndex = frameCount - 1; 
					}
				}
				Sprite = GetSpritePath(animPrefix, _currentSpriteIndex);
			}
		}

		private string GetSpritePath(string prefix, int index)
		{
			return $"{prefix}{index}.png";
		}
	}
}
