using TheCrowAndTheFox.Models.Consts;

namespace TheCrowAndTheFox.Models
{
	class Background : Rectangle
	{
		public Background(int width, int height) : base(0, 0, width, height)
		{
			Sprite = SpriteNames.Background;
		}
	}
}
