using TheCrowAndTheFox.Models.Consts;
using TheCrowAndTheFox.Models.Common;
using TheCrowAndTheFox.Interfaces;

namespace TheCrowAndTheFox.Models
{
	public class Cheese : FallingObject, ICollectable
	{
		public Cheese(float x, float y, Direction direction) : base(x, y, direction)
		{
			Sprite = SpriteNames.Cheese;
		}
	}
}