using SharpDX.Direct2D1;
using SharpDX;

namespace TheCrowAndTheFox.Models
{
	public class Player : GameObject
	{
		public int Score { get; set; }
		private const float speed = 10f;

		public Player(RenderTarget renderTarget2D) : base(renderTarget2D, 0, 500, 60f, 50f)
		{
			Sprite = "fox-sprite.png";
		}

		public void MoveLeft()
		{
			if (X > 0)
			{
				Move(-speed, 0);
			}
		}

		public void MoveRight()
		{
			if (X < 800 - Width)
			{
				Move(speed, 0);
			}
		}

		public override void Draw(RenderTarget renderTarget)
		{
			base.Draw(renderTarget);
		}
	}
}