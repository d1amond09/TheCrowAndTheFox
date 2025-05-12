using TheCrowAndTheCrow.Models.Consts;
using TheCrowAndTheFox.Models.Common;
using TheCrowAndTheFox.Models.Consts;
using TheCrowAndTheFox.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheCrowAndTheFox.Models
{
	public class Crow : GameObject
	{
		private const int COUNT_SPRITES_FLY = 5;
		private const float ANIM_FRAME_DURATION_FLY = 0.15f;
		private const float BOMB_DROP_RANGE_X = 50f;
		private const float MIN_DROP_HEIGHT_ABOVE_PLAYER = 100f;

		private float _fallingObjectXOffset;
		private float _fallingObjectYOffset;
		private Animation _animation;
		private Player _targetPlayer;
		public FallingObject FallingObject { get; protected set; }

		public Crow(float x, float y, Direction direction, float speed, FallingObject carriedObject)
			: base(x, y, GameSizes.XL)
		{
			Direction = direction;
			Speed = speed;

			FallingObject = carriedObject;
			if (FallingObject != null)
			{
				_fallingObjectXOffset = (Direction == Direction.Left) ? -20f : 20f;
				_fallingObjectYOffset = Height * 0.6f;

				FallingObject.SetPosition(X + _fallingObjectXOffset, Y + _fallingObjectYOffset);
				FallingObject.SetInitialDirection(direction);
			}

			Sprite = (Direction == Direction.Right) ? CrowSprites.FlyRight + "0.png" : CrowSprites.FlyLeft + "0.png";
			_animation = new Animation(
				(Direction == Direction.Right) ? CrowSprites.FlyRight : CrowSprites.FlyLeft,
				COUNT_SPRITES_FLY,
				ANIM_FRAME_DURATION_FLY
			);
		}

		public void FindPlayerReference(List<GameObject> gameObjects)
		{
			if (_targetPlayer == null)
			{
				_targetPlayer = gameObjects.OfType<Player>().FirstOrDefault();
			}
		}

		public override void Update(float dt)
		{
			base.Update(dt);
			UpdateSpriteState(dt);

			if (FallingObject != null && !FallingObject.IsFalling)
			{
				FallingObject.UpdateCarriedPosition(X + _fallingObjectXOffset, Y + _fallingObjectYOffset + (float)Math.Sin(X / 15f) * 5f);

				if (FallingObject is Bomb && _targetPlayer != null && new Random().Next(-5, 1) > 0)
				{
					bool isPlayerHorizontallyUnder = Math.Abs(this.CenterX - _targetPlayer.CenterX) < BOMB_DROP_RANGE_X;
					bool isHighEnough = this.Y < _targetPlayer.Y - MIN_DROP_HEIGHT_ABOVE_PLAYER;

					if (isPlayerHorizontallyUnder && isHighEnough)
					{
						FallingObject.ForceFall();
					}
				}
			}
		}

		private void UpdateSpriteState(float dt)
		{
			string animPrefix = Direction == Direction.Right
				? CrowSprites.FlyRight
				: CrowSprites.FlyLeft;

			if (_animation.CurrentSprite == null || !_animation.CurrentSprite.StartsWith(animPrefix))
			{
				_animation = new Animation(animPrefix, COUNT_SPRITES_FLY, ANIM_FRAME_DURATION_FLY);
			}

			_animation.Update(dt);
			Sprite = _animation.CurrentSprite;
		}
	}
}