﻿namespace TheCrowAndTheFox.Models
{
    public class Rectangle : GameObject
    {
		public Rectangle(float x, float y, float width, float height) 
			: base(x, y, width, height) { }

		public Rectangle(float x, float y, float size) 
			: base(x, y, size) { }
	}
}
