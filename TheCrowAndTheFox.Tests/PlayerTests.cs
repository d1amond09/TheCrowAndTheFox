using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheCrowAndTheFox.Models;
using TheCrowAndTheFox.Engine;
using Moq;
using TheCrowAndTheFox.Audio;

namespace TheCrowAndTheFox.Tests
{
	[TestClass]
	public class PlayerTests
	{
		private Mock<IAudioManager> _mockAudioManager;
		private Player _player;

		[TestInitialize]
		public void Setup()
		{
			_mockAudioManager = new Mock<IAudioManager>();
			// Игроку нужны screenWidth, initialHealth, audioManager
			_player = new Player(800, 3, _mockAudioManager.Object);
		}

		[TestMethod]
		public void TakeDamage_ReducesHealth_And_PlaysSound()
		{
			// Arrange
			int initialHealth = _player.Health;

			// Act
			_player.TakeDamage();

			// Assert
			Assert.AreEqual(initialHealth - 1, _player.Health, "Health should be reduced by 1.");
			_mockAudioManager.Verify(am => am.PlaySoundEffect(It.IsAny<string>()), Times.Once, "Hurt sound should be played once.");
		}

		[TestMethod]
		public void TakeDamage_DoesNotReduceHealth_BelowZero()
		{
			// Arrange
			// Уменьшаем здоровье до 0
			for (int i = 0; i < 10; i++) // Больше, чем начальное здоровье
			{
				_player.TakeDamage();
			}
			_mockAudioManager.Invocations.Clear(); // Очищаем вызовы мока после установки здоровья в 0

			// Act
			_player.TakeDamage(); // Попытка нанести урон при нулевом здоровье

			// Assert
			Assert.AreEqual(0, _player.Health, "Health should not go below zero.");
			_mockAudioManager.Verify(am => am.PlaySoundEffect(It.IsAny<string>()), Times.Never, "Hurt sound should not be played if health is already zero.");
		}

		[TestMethod]
		public void Jump_WhenGrounded_PlaysSound() // Тестирование изменения состояния (IsFalling, _verticalSpeed) сложнее без игрового цикла
		{
			// Arrange
			// Предполагаем, что игрок изначально на земле (IsGround = true по умолчанию в конструкторе)

			// Act
			_player.Jump();

			// Assert
			_mockAudioManager.Verify(am => am.PlaySoundEffect(It.IsAny<string>()), Times.Once, "Jump sound should be played.");
			// Дальнейшая проверка _isGround или _verticalSpeed требует симуляции Update и гравитации,
			// что выходит за рамки простого юнит-теста этого метода.
		}
	}
}