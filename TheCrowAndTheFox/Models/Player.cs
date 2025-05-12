using TheCrowAndTheFox.Engine;
using SharpDX;
using System;
using TheCrowAndTheFox.Models.Consts;
using TheCrowAndTheFox.Models.Common;
using System.Windows.Forms;
using System.IO;
using TheCrowAndTheFox.Audio;

namespace TheCrowAndTheFox.Models
{
	public class Player : GameObject
	{
		private const float MAX_SPEED = 400f;
		private const float ACCELERATION = 1500f;
		private const float DECELERATION = 2000f;
		private const float JUMP_VELOCITY = 750f;
		private const float GRAVITY = 2000f;
		private const float GROUND_LEVEL = 900f;

		private const int COUNT_SPRITES_JUMP = 7;
		private const int COUNT_SPRITES_IDLE = 5;
		private const int COUNT_SPRITES_RUN = 8;

		private const float ANIM_FRAME_DURATION_RUN = 0.08f;
		private const float ANIM_FRAME_DURATION_IDLE = 0.15f;
		private const float ANIM_FRAME_DURATION_JUMP = 0.1f;

		private IAudioManager _audioManager;
		private readonly string _jumpSoundPath = Path.Combine(Application.StartupPath, "Assets", "Music", "player_jump.mp3");
		private readonly string _hurtSoundPath = Path.Combine(Application.StartupPath, "Assets", "Music", "player_hurt.mp3");

		private float _verticalSpeed = 0f;

		private bool _isGround = false;

		private Animation _animation;
		public bool IsBorder => X == 0 || X == _screenWidth - Width;
		private bool _isTryingToMoveLeft = false;
		private bool _isTryingToMoveRight = false;
		private Direction _facingDirection = Direction.Right;

		public int Health { get; private set; }
		private readonly int _screenWidth;

		public Player(int screenWidth, int health, IAudioManager audioManager) 
		: base(100, GROUND_LEVEL - GameSizes.XL, GameSizes.XL, GameSizes.XL)
		{
			_audioManager = audioManager;
			Speed = MAX_SPEED;
			_screenWidth = screenWidth;
			Direction = Direction.None;
			_facingDirection = Direction.Right;
			Health = health;
			_isGround = true;
			Sprite = PlayerSprites.Default;
			_animation = new Animation(PlayerSprites.IdleLeft, COUNT_SPRITES_IDLE, ANIM_FRAME_DURATION_IDLE);
		}

		public void PlayerControlKeyDown(KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.A: StartMoveLeft(); break;
				case Keys.D: StartMoveRight(); break;
				case Keys.Space: Jump(); break;
			}
		}

		public void PlayerControlKeyUp(KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.A: StopMoveLeft(); break;
				case Keys.D: StopMoveRight(); break;
			}
		}

		public void TakeDamage()
		{
			if (Health > 0) 
			{
				Health--;
				_audioManager?.PlaySoundEffect(_hurtSoundPath);
			}
		}

		public void Jump()
		{
			if (_isGround)
			{
				_verticalSpeed = -JUMP_VELOCITY;
				_isGround = false;
				_audioManager?.PlaySoundEffect(_jumpSoundPath);
			}
		}

		public void StartMoveLeft()
		{
			_isTryingToMoveLeft = true;
			_isTryingToMoveRight = false; 
			_facingDirection = Direction.Left;
		}

		public void StartMoveRight()
		{
			_isTryingToMoveRight = true;
			_isTryingToMoveLeft = false; 
			_facingDirection = Direction.Right; 
		}

		public void StopMoveLeft()
		{
			_isTryingToMoveLeft = false;
		}

		public void StopMoveRight()
		{
			_isTryingToMoveRight = false;
		}

		public override void Update(float dt)
		{
			ApplyGravity(dt);
			base.Update(dt);
			HandleCollisions();
			UpdateSpriteState(dt);	
		}

		public override void Stay()
		{
			_isTryingToMoveLeft = false;
			_isTryingToMoveRight = false;
		}

		protected override void HandleHorizontalMovement(float dt)
		{
			float targetSpeed = 0;

			if (_isTryingToMoveRight)
			{
				targetSpeed = Speed;
				Direction = Direction.Right; 
				_facingDirection = Direction.Right;
			}
			else if (_isTryingToMoveLeft)
			{
				targetSpeed = -Speed;
				Direction = Direction.Left; 
				_facingDirection = Direction.Left;
			}
			else 
			{
				Direction = Direction.None;
			}


			if (targetSpeed != 0)
			{
				if (Math.Sign(targetSpeed) != Math.Sign(CurrentSpeed) && CurrentSpeed != 0) 
				{
					CurrentSpeed += Math.Sign(targetSpeed) * ACCELERATION * dt;
				}
				else
				{
					CurrentSpeed += Math.Sign(targetSpeed) * ACCELERATION * dt;
				}

				if (targetSpeed > 0)
					CurrentSpeed = Math.Min(CurrentSpeed, targetSpeed);
				else
					CurrentSpeed = Math.Max(CurrentSpeed, targetSpeed);
			}
			else 
			{
				if (Math.Abs(CurrentSpeed) > 0.01f) 
				{
					float decelerationAmount = DECELERATION * dt;
					if (CurrentSpeed > 0)
					{
						CurrentSpeed -= decelerationAmount;
						if (CurrentSpeed < 0) CurrentSpeed = 0;
					}
					else if (CurrentSpeed < 0)
					{
						CurrentSpeed += decelerationAmount;
						if (CurrentSpeed > 0) CurrentSpeed = 0;
					}
				}
				else
				{
					CurrentSpeed = 0;
				}
			}

		}

		private void ApplyGravity(float dt)
		{
			if (!_isGround) 
			{
				_verticalSpeed += GRAVITY * dt;
			}

		}

		protected override void UpdatePosition(float dt)
		{
			base.UpdatePosition(dt);
			X = MathUtil.Clamp(X, 0, _screenWidth - Width);
			
			Y += _verticalSpeed * dt;
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

			if (!_isGround)
			{
				animPrefix = (_facingDirection == Direction.Right) ? PlayerSprites.JumpRight : PlayerSprites.JumpLeft;
				frameCount = COUNT_SPRITES_JUMP;
				frameDuration = ANIM_FRAME_DURATION_JUMP;
			}
			else if (Math.Abs(CurrentSpeed) > 0.1f)
			{
				animPrefix = (_facingDirection == Direction.Right) ? PlayerSprites.RunRight : PlayerSprites.RunLeft;
				frameCount = COUNT_SPRITES_RUN;
				frameDuration = ANIM_FRAME_DURATION_RUN;
			}
			else
			{
				animPrefix = (_facingDirection == Direction.Right) ? PlayerSprites.IdleRight : PlayerSprites.IdleLeft;
				frameCount = COUNT_SPRITES_IDLE;
				frameDuration = ANIM_FRAME_DURATION_IDLE;
			}

			if (_animation.CurrentSprite == null || !_animation.CurrentSprite.StartsWith(animPrefix))
			{
				_animation = new Animation(animPrefix, frameCount, frameDuration);
			}

			_animation.Update(dt);
			Sprite = _animation.CurrentSprite;
		}
	}
}
