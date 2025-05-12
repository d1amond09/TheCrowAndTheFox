using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheCrowAndTheFox; 
using TheCrowAndTheFox.Models;
using TheCrowAndTheFox.Engine;
using Moq;
using TheCrowAndTheFox.Audio;

namespace TheCrowAndTheFox.Tests
{
	[TestClass]
	public class GameManagerTests
	{
		private Mock<IAudioManager> _mockAudioManager;
		private GameManager _gameManager;
		private Player _player; // Для доступа к игроку внутри GameManager

		[TestInitialize]
		public void Setup()
		{
			_mockAudioManager = new Mock<IAudioManager>();
			// Для инициализации GameManager нужны width, height. Можем использовать фиктивные.
			_gameManager = new GameManager(800, 600, _mockAudioManager.Object);

			// Получаем доступ к игроку, созданному внутри GameManager
			// Это может потребовать сделать _player в GameManager доступным для тестов (internal или через свойство)
			// Или мы можем создать мок игрока и передать его, если архитектура позволяет.
			// Предположим, у GameManager есть способ получить игрока или он создается предсказуемо.
			// Для простоты, предположим, мы можем как-то получить ссылку на игрока.
			// В реальном GameManager _player приватный. Нужно будет продумать, как его тестировать.
			// Один из вариантов - сделать внутренний метод для получения _player или тестировать через публичные методы.
		}

		// Вспомогательный метод для симуляции доступа к игроку в GameManager
		// В реальном коде это может быть сложнее из-за инкапсуляции
		private Player GetPlayerFromGameManager(GameManager gm)
		{
			// Это грубый способ, использующий рефлексию, если поле приватное.
			// Лучше, если бы GameManager предоставлял способ тестирования этого.
			var playerField = typeof(GameManager).GetField("_player",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			return playerField?.GetValue(gm) as Player;
		}


		[TestMethod]
		public void Score_Increases_When_Cheese_Collected()
		{
			// Arrange
			_player = GetPlayerFromGameManager(_gameManager);
			Assert.IsNotNull(_player, "Player object not found in GameManager.");

			_gameManager.Score = 0;
			Cheese cheese = new Cheese(_player.X, _player.Y, Models.Common.Direction.None); // Помещаем сыр на игрока

			// Добавляем сыр в список объектов GameManager (требует доступа или метода)
			var gameObjectsList = typeof(GameManager).GetField("_gameObjects",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
				?.GetValue(_gameManager) as System.Collections.Generic.List<GameObject>;
			Assert.IsNotNull(gameObjectsList, "_gameObjects list not found.");
			gameObjectsList.Add(cheese);

			// Act
			_gameManager.Update(0.1f); // Вызываем Update для обработки столкновений

			// Assert
			Assert.AreEqual(1, _gameManager.Score, "Score should increase by 1.");
			_mockAudioManager.Verify(am => am.PlaySoundEffect(It.IsAny<string>()), Times.AtLeastOnce(), "Cheese collection sound should play.");
			Assert.IsTrue(cheese.ShouldBeRemoved, "Cheese should be marked for removal.");
		}

		[TestMethod]
		public void Health_Decreases_When_Bomb_Hit()
		{
			// Arrange
			_player = GetPlayerFromGameManager(_gameManager);
			Assert.IsNotNull(_player, "Player object not found in GameManager.");

			int initialHealth = _player.Health;
			Bomb bomb = new Bomb(_player.X, _player.Y, Models.Common.Direction.None); // Бомба на игроке

			var gameObjectsList = typeof(GameManager).GetField("_gameObjects",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
				?.GetValue(_gameManager) as System.Collections.Generic.List<GameObject>;
			Assert.IsNotNull(gameObjectsList, "_gameObjects list not found.");
			gameObjectsList.Add(bomb);

			// Act
			_gameManager.Update(0.1f);

			// Assert
			// Player.TakeDamage() вызывается, который уменьшает здоровье.
			// GameManager.Update() затем считывает это здоровье.
			Assert.AreEqual(initialHealth - 1, _player.Health, "Player health should decrease by 1.");
			Assert.AreEqual(initialHealth - 1, _gameManager.Health, "GameManager health should reflect player's health.");
			_mockAudioManager.Verify(am => am.PlaySoundEffect(It.IsAny<string>()), Times.AtLeastOnce(), "Bomb hit sound should play.");
			Assert.IsTrue(bomb.ShouldBeRemoved, "Bomb should be marked for removal.");
		}

		[TestMethod]
		public void GameOver_State_Set_When_Health_Is_Zero()
		{
			// Arrange
			_player = GetPlayerFromGameManager(_gameManager);
			Assert.IsNotNull(_player, "Player object not found in GameManager.");

			// Устанавливаем здоровье игрока в 1, чтобы следующее попадание было фатальным
			// Это требует возможности изменять здоровье игрока для теста,
			// или многократного вызова TakeDamage.
			while (_player.Health > 1) { _player.TakeDamage(); } // Уменьшаем здоровье до 1
			_gameManager.Health = _player.Health; // Синхронизируем

			Bomb bomb = new Bomb(_player.X, _player.Y, Models.Common.Direction.None);
			var gameObjectsList = typeof(GameManager).GetField("_gameObjects",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
				?.GetValue(_gameManager) as System.Collections.Generic.List<GameObject>;
			Assert.IsNotNull(gameObjectsList, "_gameObjects list not found.");
			gameObjectsList.Add(bomb);

			// Act
			_gameManager.Update(0.1f); // Первое попадание, здоровье станет 0
			_gameManager.Update(0.1f); // Еще один апдейт, чтобы состояние GameOver было установлено

			// Assert
			Assert.IsTrue(_gameManager.IsGameOver, "IsGameOver should be true when health is zero.");
		}


		[TestMethod]
		public void Difficulty_CrowSpawnInterval_Decreases_With_Score()
		{
			// Arrange
			// Получаем доступ к приватным полям сложности через рефлексию или делаем их internal
			var initialSpawnIntervalField = typeof(GameManager).GetField("_currentCrowSpawnInterval",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			float initialSpawnInterval = (float)initialSpawnIntervalField.GetValue(_gameManager);

			int scoreTier = (int)typeof(GameManager).GetField("SCORE_TIER_FOR_DIFFICULTY_INCREASE",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);


			// Act
			_gameManager.Score = scoreTier; // Достигаем порога для увеличения сложности
			_gameManager.Update(0.1f);      // Вызываем Update для срабатывания UpdateDifficulty

			float newSpawnInterval = (float)initialSpawnIntervalField.GetValue(_gameManager);

			// Assert
			Assert.IsTrue(newSpawnInterval < initialSpawnInterval, "Crow spawn interval should decrease after reaching score tier.");
		}
	}
}