using System.Drawing;
using System.IO;
using System;
using System.Windows.Forms;
using TheCrowAndTheFox.Models;
using TheCrowAndTheFox.Models.Common;
using SharpDX.Samples;
using TheCrowAndTheFox.Engine;
using TheCrowAndTheFox.Audio;

namespace TheCrowAndTheFox
{
	public partial class MainForm : Form
	{
		private Program _gameInstance;
		private GameScreen _currentScreen = GameScreen.MainMenu;
		private int _bestScore = 0;
		private const string BestScoreFileName = "bestscore.txt";

		private IAudioManager _audioManager;

		private Button _playButton;
		private Button _settingsButton;
		private Button _exitButton;
		private Label _bestScoreLabel;

		private Button _settingsBackButton;
		private Label _settingsTitleLabel;

		private Label _musicVolumeLabel;
		private TrackBar _musicVolumeTrackBar;
		private Label _musicVolumeValueLabel;

		private Label _sfxVolumeLabel;
		private TrackBar _sfxVolumeTrackBar;
		private Label _sfxVolumeValueLabel;

		private Label _graphicsQualityLabel;
		private ComboBox _graphicsQualityComboBox;

		private int _currentMusicVolume = 70;
		private int _currentSfxVolume = 80;
		private string _currentGraphicsQuality = "Medium";
		private const string SettingsFileName = "gamesettings.txt";

		public MainForm()
		{
			InitializeAudioManager();
			SetupFormProperties();
			LoadBestScore();
			LoadGameSettings();
			ApplyInitialAudioSettings();
			SetupMainMenu();
			PlayMenuMusic();
		}

		private void InitializeAudioManager()
		{
			_audioManager = new AudioManager();
		}

		private void ApplyInitialAudioSettings()
		{
			if (_audioManager != null)
			{
				_audioManager.SetMusicVolume(_currentMusicVolume / 100f);
				_audioManager.SetSfxVolume(_currentSfxVolume / 100f);
			}
		}

		private void PlayMenuMusic()
		{
			string menuMusicPath = Path.Combine(Application.StartupPath, "Assets", "Music", "menu_music.mp3");
			if (File.Exists(menuMusicPath))
			{
				_audioManager?.PlayMusic(menuMusicPath, true);
			}
			else
			{
				Console.WriteLine($"Menu music file not found: {menuMusicPath}");
			}
		}

		private void SetupFormProperties()
		{
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1080, 720);
			this.Name = "MainForm";
			this.Text = "The Crow and The Fox - Main Menu";
			this.FormBorderStyle = FormBorderStyle.None;
			this.MaximizeBox = false;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.WindowState = FormWindowState.Maximized;
		}

		private void LoadBestScore()
		{
			if (File.Exists(BestScoreFileName))
			{
				int.TryParse(File.ReadAllText(BestScoreFileName), out _bestScore);
			}
		}

		private void SaveBestScore(int score)
		{
			if (score > _bestScore)
			{
				_bestScore = score;
				File.WriteAllText(BestScoreFileName, _bestScore.ToString());
			}
		}

		private void ClearControls()
		{
			for (int i = this.Controls.Count - 1; i >= 0; i--)
			{
				Control c = this.Controls[i];
				this.Controls.Remove(c);
				c.Dispose();
			}
		}

		private void LoadGameSettings()
		{
			if (File.Exists(SettingsFileName))
			{
				try
				{
					string[] lines = File.ReadAllLines(SettingsFileName);
					foreach (string line in lines)
					{
						string[] parts = line.Split('=');
						if (parts.Length == 2)
						{
							string key = parts[0].Trim();
							string value = parts[1].Trim();
							switch (key)
							{
								case "MusicVolume":
									int.TryParse(value, out _currentMusicVolume);
									break;
								case "SfxVolume":
									int.TryParse(value, out _currentSfxVolume);
									break;
								case "GraphicsQuality":
									_currentGraphicsQuality = value;
									break;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error loading game settings: {ex.Message}");
				}
			}
		}

		private void SaveGameSettings()
		{
			try
			{
				using (StreamWriter writer = new StreamWriter(SettingsFileName, false))
				{
					writer.WriteLine($"MusicVolume={_currentMusicVolume}");
					writer.WriteLine($"SfxVolume={_currentSfxVolume}");
					writer.WriteLine($"GraphicsQuality={_currentGraphicsQuality}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error saving game settings: {ex.Message}");
			}
		}

		private void SetupMainMenu()
		{
			ClearControls();
			_currentScreen = GameScreen.MainMenu;
			this.Text = "The Crow and The Fox - Main Menu";

			try
			{
				string imagePath = Path.Combine(Application.StartupPath, "Assets", "Textures\\UI\\main-menu2.png");
				if (File.Exists(imagePath))
				{
					this.BackgroundImage = System.Drawing.Image.FromFile(imagePath);
					this.BackgroundImageLayout = ImageLayout.Stretch;
				}
				else
				{
					this.BackColor = Color.DarkSlateGray;
					MessageBox.Show($"Background image not found at {imagePath}. Please ensure 'main-menu2.png' is in the Assets folder.", "Asset Missing");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error loading background image: {ex.Message}");
				this.BackColor = Color.DarkSlateGray;
			}

			_bestScoreLabel = new Label
			{
				Text = $"Best Score: {_bestScore}",
				Size = new Size(600, 300),
				Location = new Point(15, 15),
				Font = new Font("Maiandra GD", 36F, FontStyle.Bold),
				ForeColor = Color.White,
				TextAlign = ContentAlignment.TopLeft,
				BackColor = Color.Transparent
			};
			this.Controls.Add(_bestScoreLabel);

			_playButton = new Button
			{
				Text = "Play",
				Size = new Size(270, 65),
				Location = new Point(270, 600),
				Font = new Font("Maiandra GD", 22F, FontStyle.Bold),
				BackColor = Color.FromArgb(255, 128, 0),
				ForeColor = Color.White
			};
			_playButton.Click += PlayButton_Click;
			this.Controls.Add(_playButton);

			_settingsButton = new Button
			{
				Text = "Settings",
				Size = new Size(270, 65),
				Location = new Point(270, _playButton.Bottom + 20),
				Font = new Font("Maiandra GD", 22F, FontStyle.Bold),
				BackColor = Color.Gray,
				ForeColor = Color.White
			};
			_settingsButton.Click += SettingsButton_Click;
			this.Controls.Add(_settingsButton);

			_exitButton = new Button
			{
				Text = "Exit",
				Size = new Size(270, 65),
				Location = new Point(270, _settingsButton.Bottom + 20),
				Font = new Font("Maiandra GD", 22F, FontStyle.Bold),
				BackColor = Color.DarkRed,
				ForeColor = Color.White
			};
			_exitButton.Click += ExitButton_Click;
			this.Controls.Add(_exitButton);
		}

		private void SetupSettingsScreen()
		{
			ClearControls();
			_currentScreen = GameScreen.Settings;
			this.Text = "The Crow and The Fox - Settings";
			this.BackgroundImage = null;
			this.BackColor = Color.FromArgb(40, 40, 60);

			int controlXOffset = 150;
			int controlWidth = this.ClientSize.Width - (2 * controlXOffset);
			int labelWidth = 200;
			int trackBarWidth = controlWidth - labelWidth - 100;
			int currentY = 50;

			_settingsTitleLabel = new Label
			{
				Text = "Settings",
				Font = new Font("Maiandra GD", 28F, FontStyle.Bold),
				ForeColor = Color.White,
				BackColor = Color.Transparent,
				AutoSize = false,
				Size = new Size(this.ClientSize.Width - 100, 60),
				TextAlign = ContentAlignment.MiddleCenter,
				Location = new Point(50, currentY)
			};
			this.Controls.Add(_settingsTitleLabel);
			currentY += _settingsTitleLabel.Height + 40;

			_musicVolumeLabel = new Label
			{
				Text = "Music Volume:",
				Font = new Font("Arial", 12F, FontStyle.Bold),
				ForeColor = Color.White,
				BackColor = Color.Transparent,
				Size = new Size(labelWidth, 30),
				TextAlign = ContentAlignment.MiddleLeft,
				Location = new Point(controlXOffset, currentY)
			};
			this.Controls.Add(_musicVolumeLabel);

			_musicVolumeTrackBar = new TrackBar
			{
				Minimum = 0,
				Maximum = 100,
				Value = _currentMusicVolume,
				TickFrequency = 10,
				Size = new Size(trackBarWidth, 45),
				Location = new Point(_musicVolumeLabel.Right + 10, currentY - 5)
			};
			_musicVolumeTrackBar.Scroll += MusicVolumeTrackBar_Scroll;
			this.Controls.Add(_musicVolumeTrackBar);

			_musicVolumeValueLabel = new Label
			{
				Text = $"{_musicVolumeTrackBar.Value}%",
				Font = new Font("Arial", 12F),
				ForeColor = Color.White,
				BackColor = Color.Transparent,
				Size = new Size(70, 30),
				TextAlign = ContentAlignment.MiddleRight,
				Location = new Point(_musicVolumeTrackBar.Right + 10, currentY)
			};
			this.Controls.Add(_musicVolumeValueLabel);
			currentY += _musicVolumeTrackBar.Height + 20;

			_sfxVolumeLabel = new Label
			{
				Text = "Sound FX Volume:",
				Font = new Font("Arial", 12F, FontStyle.Bold),
				ForeColor = Color.White,
				BackColor = Color.Transparent,
				Size = new Size(labelWidth, 30),
				TextAlign = ContentAlignment.MiddleLeft,
				Location = new Point(controlXOffset, currentY)
			};
			this.Controls.Add(_sfxVolumeLabel);

			_sfxVolumeTrackBar = new TrackBar
			{
				Minimum = 0,
				Maximum = 100,
				Value = _currentSfxVolume,
				TickFrequency = 10,
				Size = new Size(trackBarWidth, 45),
				Location = new Point(_sfxVolumeLabel.Right + 10, currentY - 5)
			};
			_sfxVolumeTrackBar.Scroll += SfxVolumeTrackBar_Scroll;
			this.Controls.Add(_sfxVolumeTrackBar);

			_sfxVolumeValueLabel = new Label
			{
				Text = $"{_sfxVolumeTrackBar.Value}%",
				Font = new Font("Arial", 12F),
				ForeColor = Color.White,
				BackColor = Color.Transparent,
				Size = new Size(70, 30),
				TextAlign = ContentAlignment.MiddleRight,
				Location = new Point(_sfxVolumeTrackBar.Right + 10, currentY)
			};
			this.Controls.Add(_sfxVolumeValueLabel);
			currentY += _sfxVolumeTrackBar.Height + 20;

			_graphicsQualityLabel = new Label
			{
				Text = "Graphics Quality:",
				Font = new Font("Arial", 12F, FontStyle.Bold),
				ForeColor = Color.White,
				BackColor = Color.Transparent,
				Size = new Size(labelWidth, 30),
				TextAlign = ContentAlignment.MiddleLeft,
				Location = new Point(controlXOffset, currentY)
			};
			this.Controls.Add(_graphicsQualityLabel);

			_graphicsQualityComboBox = new ComboBox
			{
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Arial", 12F),
				Size = new Size(trackBarWidth, 30),
				Location = new Point(_graphicsQualityLabel.Right + 10, currentY)
			};
			_graphicsQualityComboBox.Items.AddRange(new string[] { "Low", "Medium", "High", "Ultra" });
			_graphicsQualityComboBox.SelectedItem = _currentGraphicsQuality;
			_graphicsQualityComboBox.SelectedIndexChanged += GraphicsQualityComboBox_SelectedIndexChanged;
			this.Controls.Add(_graphicsQualityComboBox);
			currentY += _graphicsQualityComboBox.Height + 50;

			_settingsBackButton = new Button
			{
				Text = "Back to Main Menu",
				Size = new Size(250, 50),
				Font = new Font("Maiandra GD", 14F, FontStyle.Bold),
				ForeColor = Color.White,
				BackColor = Color.FromArgb(80, 80, 100),
				FlatStyle = FlatStyle.Flat,
				Location = new Point(this.ClientSize.Width / 2 - 125, this.ClientSize.Height - 100)
			};
			_settingsBackButton.FlatAppearance.BorderColor = Color.LightGray;
			_settingsBackButton.Click += (s, e) => {
				SaveGameSettings();
				SetupMainMenu();
			};
			this.Controls.Add(_settingsBackButton);
		}

		private void PlayButton_Click(object sender, EventArgs e)
		{
			_audioManager?.StopMusic();
			string menuMusicPath = Path.Combine(Application.StartupPath, "Assets", "Music", "game_music.mp3");
			_audioManager?.PlayMusic(menuMusicPath, true);
			StartGame();
		}

		private void SettingsButton_Click(object sender, EventArgs e)
		{
			SetupSettingsScreen();
		}

		private void ExitButton_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		public string GetCurrentGraphicsQuality()
		{
			return _currentGraphicsQuality;
		}

		private void StartGame()
		{
			this.Hide();
			_gameInstance?.Dispose();
			_gameInstance = new Program(this, _audioManager);
			_gameInstance.Run(new DemoConfiguration("The Crow and The Fox", this.ClientSize.Width, this.ClientSize.Height));
			this.Show();
			ReturnToMainMenu(false);
		}

		public void ReturnToMainMenu(bool disposeGame = true)
		{
			if (disposeGame && _gameInstance != null)
			{
				// _gameInstance.ExitGameLoop();
			}
			_gameInstance = null;

			this.Show();
			LoadBestScore();
			LoadGameSettings();
			ApplyInitialAudioSettings();

			PlayMenuMusic();
			SetupMainMenu();
		}

		public void RestartGame()
		{
			StartGame();
		}

		public int GetBestScore()
		{
			return _bestScore;
		}

		public void UpdateAndSavePlayerScore(int currentScore)
		{
			SaveBestScore(currentScore);
			if (_bestScoreLabel != null)
			{
				_bestScoreLabel.Text = $"Best Score: {_bestScore}";
			}
		}

		private void MusicVolumeTrackBar_Scroll(object sender, EventArgs e)
		{
			_currentMusicVolume = _musicVolumeTrackBar.Value;
			_musicVolumeValueLabel.Text = $"{_currentMusicVolume}%";
			_audioManager?.SetMusicVolume(_currentMusicVolume / 100f);
		}

		private void SfxVolumeTrackBar_Scroll(object sender, EventArgs e)
		{
			_currentSfxVolume = _sfxVolumeTrackBar.Value;
			_sfxVolumeValueLabel.Text = $"{_currentSfxVolume}%";
			_audioManager?.SetSfxVolume(_currentSfxVolume / 100f);
		}

		private void GraphicsQualityComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			_currentGraphicsQuality = _graphicsQualityComboBox.SelectedItem.ToString();
			ApplyGraphicsSettingsToGame();
		}

		public void ApplyGraphicsSettingsToGame()
		{
			_gameInstance?.UpdateGraphicsSettings(_currentGraphicsQuality);
		}

		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Application.Exit();
		}
	}
}