using System;
using System.Windows.Forms;
using SharpDX.Samples;
using TheCrowAndTheFox.Models;

namespace TheCrowAndTheFox
{
	internal class Program : Direct2D1WinFormDemoApp
	{
		private Game _game;

		public Program()
		{
		}

		protected override void Initialize(DemoConfiguration demoConfiguration)
		{
			base.Initialize(demoConfiguration);
			_game = new Game(RenderTarget2D);
		}

		protected override void KeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Exit();

			_game.PlayerControl(e);
		}

		protected override void Update(DemoTime time)
		{
			_game.Update();
			Timr.DeltaTime = FrameDelta;
			
		}

		protected override void Draw(DemoTime time)
		{
			_game.Render(RenderTarget2D);
		}



		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new MainForm());
			Program program = new Program();
			program.Run(new DemoConfiguration("SharpDX DirectWrite Simple HelloWorld Demo"));
		}
	}
}
