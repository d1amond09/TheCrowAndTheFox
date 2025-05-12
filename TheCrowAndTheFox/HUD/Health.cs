using System.Collections.Generic;
using TheCrowAndTheFox.Engine;
using TheCrowAndTheFox.Models;
using SharpDX.Direct2D1;
using System.Linq;

namespace TheCrowAndTheFox.HUD
{
	public class Health
	{
		public List<Heart> Hearts { get; set; }

		public Health(int health)
		{
			Hearts = new List<Heart>();
			for (int i = 0; i < health; i++)
			{
				Hearts.Add(new Heart(i * 60, 10));
			}
		}

		public void Update(float dt, int health)
		{
			if(health < Hearts.Count(h => h.IsActive))
			{
				Heart heart = Hearts.FindLast(h => h.IsActive);
				heart.SetEmpty();
			}

			foreach (var heart in Hearts)
			{
				heart.Update(dt);
			}
		}

		public void Draw(RenderTarget renderTarget, BitmapLoader bitmapLoader)
		{
			foreach (var heart in Hearts)
			{
				heart.Draw(renderTarget, bitmapLoader);
			}
		}
	}
}
