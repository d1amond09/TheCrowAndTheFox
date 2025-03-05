using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;

namespace TheCrowAndTheFox.Models
{
	class Background : GameObject
	{
		public Background(RenderTarget renderTarget2D) : base(renderTarget2D, 0, 0, 1080, 720)
		{
			Sprite = "background.png";
		}

		public override void Draw(RenderTarget renderTarget)
		{
			base.Draw(renderTarget);
		}
	}
}
