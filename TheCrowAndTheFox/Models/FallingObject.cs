using SharpDX.Direct2D1;
using SharpDX;
using System;
using SharpDX.Mathematics.Interop;
using System.Drawing.Imaging;

namespace TheCrowAndTheFox.Models
{
	public class FallingObject : GameObject
	{
		private const float speed = 70f;
		protected float _rotation; 
		protected const float _angle = (float)Math.PI; 

		public FallingObject(RenderTarget renderTarget2D, float x) : base(renderTarget2D, x, 0, 50f) 
		{
			Sprite = "cheese.png";
		}

		public void Move()
		{
			Rotate(_angle * Timr.DeltaTime);
			base.Move(0, speed * Timr.DeltaTime);

		}
		public void Rotate(float angle)
		{
			_rotation -= angle; 
		}

		public override void Draw(RenderTarget renderTarget)
		{
			// Сохраняем текущую трансформацию
			var originalTransform = renderTarget.Transform;
			

			var centerX = X + Width / 2;
			var centerY = Y + Height / 2;

			// Создаем матрицу сдвига обратно
			var finalMatrix = Matrix3x2.Rotation(_rotation, new Vector2(centerX, centerY));
			
			// Устанавливаем трансформацию
			renderTarget.Transform = finalMatrix;
			

			//renderTarget.Transform = originalTransform;

		}
	}
}