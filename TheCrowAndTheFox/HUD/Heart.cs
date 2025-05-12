using TheCrowAndTheFox.Models.Consts;

namespace TheCrowAndTheFox.Models
{
	public class Heart : Rectangle
	{
		public bool IsActive { get; private set; }

		public Heart(float x, float y) : base(x, y, 50)
		{
			Sprite = SpriteNames.HeartFull;
			IsActive = true;
		}

		public void SetEmpty() => IsActive = false;

		public override void Update(float dt)
		{
			base.Update(dt);

			if(IsActive)
			{
				Sprite = SpriteNames.HeartFull;
			}
			else
			{
				Sprite = SpriteNames.HeartEmpty;
			}
		}
	}
}
