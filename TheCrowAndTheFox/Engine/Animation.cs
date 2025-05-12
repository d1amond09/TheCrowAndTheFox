namespace TheCrowAndTheFox.Engine
{
	public class Animation
	{
		private int _currentSpriteIndex = 0;
		private float _animationTimer = 0f;
		private readonly string _spritePrefix;
		private readonly int _frameCount;
		private readonly float _frameDuration;
		private readonly bool _loop;

		public string CurrentSprite { get; private set; }

		public Animation(string spritePrefix, int frameCount, float frameDuration, bool loop = true)
		{
			_spritePrefix = spritePrefix;
			_frameCount = frameCount;
			_frameDuration = frameDuration;
			_loop = loop;
			CurrentSprite = GetSpritePath(_currentSpriteIndex);
		}

		public void Update(float dt)
		{
			_animationTimer += dt;

			if (_animationTimer >= _frameDuration)
			{
				_animationTimer -= _frameDuration;
				_currentSpriteIndex++;

				if (_currentSpriteIndex >= _frameCount)
				{
					if (_loop)
					{
						_currentSpriteIndex = 0;
					}
					else
					{
						_currentSpriteIndex = _frameCount - 1;
					}
				}
				CurrentSprite = GetSpritePath(_currentSpriteIndex);
			}
		}

		private string GetSpritePath(int index)
		{
			return $"{_spritePrefix}{index}.png";
		}
	}
}