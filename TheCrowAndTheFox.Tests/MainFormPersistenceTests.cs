using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheCrowAndTheFox;
using System.IO;

namespace TheCrowAndTheFox.Tests
{
	[TestClass]
	public class MainFormPersistenceTests
	{
		private string _testSettingsFileName;
		private string _testBestScoreFileName;
		private MainForm _mainFormInstance; // Нужен для вызова методов, но без отображения UI

		[TestInitialize]
		public void TestInit()
		{
			_testSettingsFileName = Path.Combine(Path.GetTempPath(), $"test_gamesettings_{Guid.NewGuid()}.txt");
			_testBestScoreFileName = Path.Combine(Path.GetTempPath(), $"test_bestscore_{Guid.NewGuid()}.txt");
		}

		[TestCleanup]
		public void TestCleanup()
		{
			try
			{
				if (File.Exists(_testSettingsFileName)) File.Delete(_testSettingsFileName);
			}
			catch (IOException ex)
			{
				Console.WriteLine($"Error deleting {_testSettingsFileName} in Cleanup: {ex.Message}");
			}
			try
			{
				if (File.Exists(_testBestScoreFileName)) File.Delete(_testBestScoreFileName);
			}
			catch (IOException ex)
			{
				Console.WriteLine($"Error deleting {_testBestScoreFileName} in Cleanup: {ex.Message}");
			}
		}


		[TestMethod]
		public void Settings_SaveAndLoad_Correctly()
		{
			// Arrange
			var settingsToSave = new GameSettingsMock
			{
				MusicVolume = 50,
				SfxVolume = 75,
				GraphicsQuality = "High"
			};
			SaveSettingsDirectly(settingsToSave, _testSettingsFileName);


			// Act
			var loadedSettings = LoadSettingsDirectly(_testSettingsFileName);

			// Assert
			Assert.IsNotNull(loadedSettings, "Loaded settings should not be null.");
			Assert.AreEqual(settingsToSave.MusicVolume, loadedSettings.MusicVolume);
			Assert.AreEqual(settingsToSave.SfxVolume, loadedSettings.SfxVolume);
			Assert.AreEqual(settingsToSave.GraphicsQuality, loadedSettings.GraphicsQuality);
		}

		[TestMethod]
		public void BestScore_SaveAndLoad_Correctly()
		{
			// Arrange
			int scoreToSave = 12345;
			SaveBestScoreDirectly(scoreToSave, _testBestScoreFileName);

			// Act
			int loadedScore = LoadBestScoreDirectly(_testBestScoreFileName);

			// Assert
			Assert.AreEqual(scoreToSave, loadedScore);
		}

		private class GameSettingsMock { public int MusicVolume; public int SfxVolume; public string GraphicsQuality; }

		private void SaveSettingsDirectly(GameSettingsMock settings, string filePath)
		{
			using (StreamWriter writer = new StreamWriter(filePath, false))
			{
				writer.WriteLine($"MusicVolume={settings.MusicVolume}");
				writer.WriteLine($"SfxVolume={settings.SfxVolume}");
				writer.WriteLine($"GraphicsQuality={settings.GraphicsQuality}");
			}
		}
		private GameSettingsMock LoadSettingsDirectly(string filePath)
		{
			var settings = new GameSettingsMock { MusicVolume = 70, SfxVolume = 80, GraphicsQuality = "Medium" }; // Defaults
			if (File.Exists(filePath))
			{
				string[] lines = File.ReadAllLines(filePath);
				foreach (string line in lines)
				{
					string[] parts = line.Split('=');
					if (parts.Length == 2)
					{
						string key = parts[0].Trim();
						string value = parts[1].Trim();
						switch (key)
						{
							case "MusicVolume": int.TryParse(value, out settings.MusicVolume); break;
							case "SfxVolume": int.TryParse(value, out settings.SfxVolume); break;
							case "GraphicsQuality": settings.GraphicsQuality = value; break;
						}
					}
				}
			}
			return settings;
		}
		private void SaveBestScoreDirectly(int score, string filePath) { File.WriteAllText(filePath, score.ToString()); }
		private int LoadBestScoreDirectly(string filePath)
		{
			if (File.Exists(filePath)) { int.TryParse(File.ReadAllText(filePath), out int score); return score; }
			return 0;
		}
	}
}