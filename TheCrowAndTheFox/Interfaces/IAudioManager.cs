using System;

namespace TheCrowAndTheFox.Audio
{
	public interface IAudioManager : IDisposable
	{
		void PlayMusic(string filePath, bool loop = true);
		void StopMusic();
		void SetMusicVolume(float volume);
		float GetMusicVolume();

		void PlaySoundEffect(string filePath);
		void SetSfxVolume(float volume);
		float GetSfxVolume();
	}
}
