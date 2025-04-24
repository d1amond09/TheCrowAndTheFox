using System;
using System.Diagnostics;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Samples;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;
using Timer = TheCrowAndTheFox.Engine.Timer;

namespace TheCrowAndTheFox
{
	internal class Program : Direct2D1WinFormDemoApp
	{
		private Game _game;
		public int Width { get; set; }
		public int Height { get; set; }
		public TextFormat TextFormat { get; private set; }
        public TextLayout TextLayout { get; private set; }

		public Program()
		{
		}

		protected override void Initialize(DemoConfiguration demoConfiguration)
		{
			base.Initialize(demoConfiguration);
			_game = new Game();

			Width = demoConfiguration.Width;
			Height = demoConfiguration.Height;

			TextFormat = new TextFormat(FactoryDWrite, "Calibri", 54) { TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Near };
			RenderTarget2D.TextAntialiasMode = TextAntialiasMode.Cleartype;
		}

		protected override void KeyDown(KeyEventArgs e)
		{
			_game.PlayerControlKeyDown(e);

			if(e.KeyCode == Keys.Escape)
			{
				_game.Pause();
			}
		}

		protected override void KeyUp(KeyEventArgs e)
		{
			_game.PlayerControlKeyUp(e);
		}

		protected override void Update(DemoTime time)
		{
			_game.Update();
			Timer.DeltaTime = FrameDelta;
		}

		protected override void Draw(DemoTime time)
		{
			_game.Render(RenderTarget2D);
			TextLayout = new TextLayout(FactoryDWrite, $" Score: {Timer.Debug}", TextFormat, Width, 100);
			
			RenderTarget2D.DrawTextLayout(new Vector2(0, 0), TextLayout, SceneColorBrush, DrawTextOptions.None);
			TextLayout.Dispose();
		}

		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new MainForm());
			Program program = new Program();
			program.Run(new DemoConfiguration("The Crow and The Fox"));
		}
	}
}
