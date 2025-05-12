using NAudio.Wave;
using System;
using TheCrowAndTheFox.Audio;

namespace TheCrowAndTheFox.Engine
{
	public class AudioManager : IAudioManager
	{
		private IWavePlayer _musicDevice;
		private AudioFileReader _musicFileReader;
		private float _musicVolume = 0.5f;
		private bool _isMusicPlaying = false;

		private IWavePlayer _sfxDevice;
		private float _sfxVolume = 0.7f;

		public AudioManager() { }

		public void PlayMusic(string filePath, bool loop = true)
		{
			StopMusic();
			try
			{
				_musicFileReader = new AudioFileReader(filePath) { Volume = _musicVolume };
				_musicDevice = new WaveOutEvent();
				_musicDevice.Init(_musicFileReader);

				if (loop)
				{
					_musicDevice.PlaybackStopped += (sender, args) =>
					{
						if (loop && _musicFileReader != null && _isMusicPlaying)
						{
							_musicFileReader.Position = 0;
							_musicDevice?.Play();
						}
					};
				}
				_musicDevice.Play();
				_isMusicPlaying = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error playing music '{filePath}': {ex.Message}");
				StopMusic();
			}
		}

		public void StopMusic()
		{
			_isMusicPlaying = false;
			_musicDevice?.Stop();
			_musicFileReader?.Dispose();
			_musicFileReader = null;
			_musicDevice?.Dispose();
			_musicDevice = null;
		}

		public void SetMusicVolume(float volume)
		{
			_musicVolume = Math.Max(0f, Math.Min(1f, volume));
			if (_musicFileReader != null)
			{
				_musicFileReader.Volume = _musicVolume;
			}
		}

		public float GetMusicVolume() => _musicVolume;

		public void PlaySoundEffect(string filePath)
		{
			if (string.IsNullOrEmpty(filePath)) return;

			try
			{
				_sfxDevice?.Stop();
				(_sfxDevice as IDisposable)?.Dispose();
				_sfxDevice = null;

				var sfxFileReader = new AudioFileReader(filePath) { Volume = _sfxVolume };
				_sfxDevice = new WaveOutEvent();
				_sfxDevice.Init(sfxFileReader);

				_sfxDevice.PlaybackStopped += (sender, args) =>
				{
					if (sender == _sfxDevice)
					{
						sfxFileReader.Dispose();
						(sender as IWavePlayer)?.Dispose();
						if (sender == _sfxDevice)
						{
							_sfxDevice = null;
						}
					}
				};
				_sfxDevice.Play();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error playing SFX '{filePath}': {ex.Message}");
				(_sfxDevice as IDisposable)?.Dispose();
				_sfxDevice = null;
			}
		}

		public void SetSfxVolume(float volume)
		{
			_sfxVolume = Math.Max(0f, Math.Min(1f, volume));
		}

		public float GetSfxVolume() => _sfxVolume;

		public void Dispose()
		{
			StopMusic();

			_sfxDevice?.Stop();
			(_sfxDevice as IDisposable)?.Dispose();
			_sfxDevice = null;
		}
	}
}