using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;
using TheCrowAndTheFox.Engine;
using System.Windows.Forms;
using TheCrowAndTheFox.HUD;
using SharpDX.Samples;
using System;
using TheCrowAndTheFox.Models.Common;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX;
using TheCrowAndTheFox.Audio;

namespace TheCrowAndTheFox
{
	internal class Program : Direct2D1WinFormDemoApp
	{
		private GameManager _game;
		private HeadsUpDisplay _hud;

		public int Width { get; set; }
		public int Height { get; set; }
		public BitmapLoader BitmapLoader { get; private set; }

		private MainForm _mainFormHost;
		public GameScreen CurrentScreen { get; private set; } = GameScreen.Playing;
		private bool _exitLoopSignal = false;

		private TextFormat _menuTextFormat;
		private TextFormat _titleTextFormat;
		private SolidColorBrush _menuBrush;
		private SolidColorBrush _overlayBrushPaused;
		private SolidColorBrush _overlayBrushGameOver;
		private IAudioManager _audioManager;

		private string _activeGraphicsQuality = "Medium";

		public Program(MainForm mainFormHost, IAudioManager audioManager)
		{
			_mainFormHost = mainFormHost;
			_audioManager = audioManager;
		}

		protected override void Initialize(DemoConfiguration demoConfiguration)
		{
			base.Initialize(demoConfiguration);
			BitmapLoader = new BitmapLoader();

			Width = demoConfiguration.Width;
			Height = demoConfiguration.Height;

			_game = new GameManager(demoConfiguration.Width, demoConfiguration.Height, _audioManager);
			_game.BestScore = _mainFormHost.GetBestScore();

			_hud = new HeadsUpDisplay(_game, FactoryDWrite);
			RenderTarget2D.TextAntialiasMode = TextAntialiasMode.Cleartype;

			_menuTextFormat = new TextFormat(FactoryDWrite, "Maiandra GD", FontWeight.Normal, FontStyle.Normal, FontStretch.Normal, 24)
			{
				TextAlignment = TextAlignment.Center,
				ParagraphAlignment = ParagraphAlignment.Center
			};
			_titleTextFormat = new TextFormat(FactoryDWrite, "Maiandra GD", FontWeight.Bold, FontStyle.Normal, FontStretch.Normal, 36)
			{
				TextAlignment = TextAlignment.Center,
				ParagraphAlignment = ParagraphAlignment.Center
			};
			_menuBrush = new SolidColorBrush(RenderTarget2D, Color.White);
			_overlayBrushPaused = new SolidColorBrush(RenderTarget2D, new Color4(0, 0, 0, 0.7f));
			_overlayBrushGameOver = new SolidColorBrush(RenderTarget2D, new Color4(0.1f, 0, 0, 0.85f));

			CurrentScreen = GameScreen.Playing;

			if (_mainFormHost != null)
			{
				UpdateGraphicsSettings(_mainFormHost.GetCurrentGraphicsQuality());
			}
			else
			{
				UpdateGraphicsSettings("Medium");
			}
		}

		public void UpdateGraphicsSettings(string qualityLevel)
		{
			_activeGraphicsQuality = qualityLevel;

			if (RenderTarget2D == null) return;

			switch (_activeGraphicsQuality)
			{
				case "Low":
					RenderTarget2D.TextAntialiasMode = TextAntialiasMode.Grayscale;
					break;
				case "Ultra":
				case "High":
				case "Medium":
				default:
					RenderTarget2D.TextAntialiasMode = TextAntialiasMode.Cleartype;
					break;
			}

			if (_game != null)
			{
				_game.SetGraphicsQuality(_activeGraphicsQuality);
			}
			Console.WriteLine($"Graphics settings applied: {_activeGraphicsQuality}, Text AA: {RenderTarget2D.TextAntialiasMode}");
		}

		protected override void KeyDown(KeyEventArgs e)
		{
			if (_exitLoopSignal) return;

			if (CurrentScreen == GameScreen.Playing)
			{
				_game.PlayerControlKeyDown(e);
				if (e.KeyCode == Keys.Escape)
				{
					TogglePause();
				}
			}
			else if (CurrentScreen == GameScreen.Paused)
			{
				if (e.KeyCode == Keys.Escape) { TogglePause(); }
				else if (e.KeyCode == Keys.R) { RequestRestart(); }
				else if (e.KeyCode == Keys.M) { RequestReturnToMainMenu(); }
			}
			else if (CurrentScreen == GameScreen.GameOver)
			{
				if (e.KeyCode == Keys.R) { RequestRestart(); }
				else if (e.KeyCode == Keys.M) { RequestReturnToMainMenu(); }
				else if (e.KeyCode == Keys.Escape) { RequestReturnToMainMenu(); }
			}
		}

		protected override void KeyUp(KeyEventArgs e)
		{
			if (CurrentScreen == GameScreen.Playing)
			{
				_game.PlayerControlKeyUp(e);
			}
		}

		public void TogglePause()
		{
			if (CurrentScreen == GameScreen.Playing)
			{
				CurrentScreen = GameScreen.Paused;
				_game.SetPaused(true);
			}
			else if (CurrentScreen == GameScreen.Paused)
			{
				CurrentScreen = GameScreen.Playing;
				_game.SetPaused(false);
			}
		}

		public void RequestRestart()
		{
			_exitLoopSignal = true;
			_mainFormHost.RestartGame();
			this.Exit();
		}

		public void RequestReturnToMainMenu()
		{
			_exitLoopSignal = true;
			_mainFormHost.ReturnToMainMenu();
			this.Exit();
		}

		protected override void Update(DemoTime time)
		{
			if (_exitLoopSignal) return;

			if (CurrentScreen == GameScreen.Playing)
			{
				_game.Update(FrameDelta);
				_hud.Update(FrameDelta);

				if (_game.IsGameOver)
				{
					CurrentScreen = GameScreen.GameOver;
					_mainFormHost.UpdateAndSavePlayerScore((int)_game.Score);
					_game.BestScore = _mainFormHost.GetBestScore();
				}
			}
		}

		protected override void Draw(DemoTime time)
		{
			if (_exitLoopSignal) return;

			RenderTarget2D.Clear(Color.CornflowerBlue);

			if (CurrentScreen == GameScreen.Playing || CurrentScreen == GameScreen.Paused || CurrentScreen == GameScreen.GameOver)
			{
				_game.Render(RenderTarget2D, BitmapLoader);
				_hud.Draw(RenderTarget2D, BitmapLoader, SceneColorBrush);
			}

			if (CurrentScreen == GameScreen.Paused)
			{
				DrawPauseMenu();
			}
			else if (CurrentScreen == GameScreen.GameOver)
			{
				DrawGameOverScreen();
			}
		}

		private void DrawPauseMenu()
		{
			RenderTarget2D.FillRectangle(new RectangleF(0, 0, Config.Width, Config.Height), _overlayBrushPaused);

			float RctHeight = 60;
			RenderTarget2D.DrawText("Paused", _titleTextFormat, new RectangleF(0, Config.Height * 0.2f, Config.Width, RctHeight), _menuBrush);
			RenderTarget2D.DrawText("Press ESC to Resume", _menuTextFormat, new RectangleF(0, Config.Height * 0.4f, Config.Width, RctHeight), _menuBrush);
			RenderTarget2D.DrawText("Press R to Restart", _menuTextFormat, new RectangleF(0, Config.Height * 0.5f + 10, Config.Width, RctHeight), _menuBrush);
			RenderTarget2D.DrawText("Press M for Main Menu", _menuTextFormat, new RectangleF(0, Config.Height * 0.6f + 20, Config.Width, RctHeight), _menuBrush);
		}

		private void DrawGameOverScreen()
		{
			RenderTarget2D.FillRectangle(new RectangleF(0, 0, Config.Width, Config.Height), _overlayBrushGameOver);

			float RctHeight = 60;
			RenderTarget2D.DrawText("Game Over", _titleTextFormat, new RectangleF(0, Config.Height * 0.2f, Config.Width, RctHeight), _menuBrush);
			RenderTarget2D.DrawText($"Your Score: {(int)_game.Score}", _menuTextFormat, new RectangleF(0, Config.Height * 0.35f, Config.Width, RctHeight), _menuBrush);
			RenderTarget2D.DrawText($"Best Score: {(int)_game.BestScore}", _menuTextFormat, new RectangleF(0, Config.Height * 0.45f + 10, Config.Width, RctHeight), _menuBrush);
			RenderTarget2D.DrawText("Press R to Restart", _menuTextFormat, new RectangleF(0, Config.Height * 0.6f + 20, Config.Width, RctHeight), _menuBrush);
			RenderTarget2D.DrawText("Press M for Main Menu", _menuTextFormat, new RectangleF(0, Config.Height * 0.7f + 30, Config.Width, RctHeight), _menuBrush);
		}

		protected override void Dispose(bool disposeManagedResources)
		{
			if (disposeManagedResources)
			{
				_menuTextFormat?.Dispose();
				_titleTextFormat?.Dispose();
				_menuBrush?.Dispose();
				_overlayBrushPaused?.Dispose();
				_overlayBrushGameOver?.Dispose();
				_game?.Dispose();
				_hud?.Dispose();
				BitmapLoader?.Dispose();
			}
			base.Dispose(disposeManagedResources);
		}
	}
}