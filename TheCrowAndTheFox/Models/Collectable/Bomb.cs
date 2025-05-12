using TheCrowAndTheFox.Models.Consts;
using TheCrowAndTheFox.Models.Common;
using TheCrowAndTheFox.Interfaces;

namespace TheCrowAndTheFox.Models
{
	public class Bomb : FallingObject, ICollectable
	{
		public Bomb(float x, float y, Direction direction) : base(x, y, direction)
		{
			Sprite = SpriteNames.Bomb;
		}
	}
}