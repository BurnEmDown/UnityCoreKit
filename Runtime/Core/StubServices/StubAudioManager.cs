using Core.Interfaces;
using UnityEngine;
using Logger = Core.Utils.Logs.Logger;

namespace Core.StubServices
{
    /// <summary>
    /// A placeholder implementation of <see cref="IAudioManager"/> used for development,
    /// prototyping, and as a reference example for creating a real audio manager.
    /// </summary>
    /// <remarks>
    /// This stub does **not** play real audio.  
    /// Instead, it prints log messages using <see cref="Logger"/> to simulate audio behavior.
    ///
    /// The purpose of this class is to:
    /// <list type="bullet">
    ///   <item><description>Allow the project to run without a real audio backend</description></item>
    ///   <item><description>Provide an example of how a proper audio manager might be structured</description></item>
    ///   <item><description>Prevent null-reference errors during early development</description></item>
    /// </list>
    ///
    /// **Do not** ship this in production.  
    /// Replace it with an actual implementation using Unity AudioSources, a mixer system,
    /// FMOD, Wwise, or any other audio framework.
    /// </remarks>
    public class StubAudioManager : IAudioManager
    {
        private float _sfxVolume = 1f;
        private float _musicVolume = 1f;

        /// <summary>
        /// Logs a simulated sound effect play request.
        /// </summary>
        public void PlaySfx(string key)
        {
            Logger.Log($"[Audio Stub] PlaySFX: {key} (vol={_sfxVolume})");
        }

        /// <summary>
        /// Logs a simulated music playback request.
        /// </summary>
        public void PlayMusic(string key, bool loop = true)
        {
            Logger.Log($"[Audio Stub] PlayMusic: {key} (loop={loop}, vol={_musicVolume})");
        }

        /// <summary>
        /// Logs a simulated music stop request.
        /// </summary>
        public void StopMusic()
        {
            Logger.Log("[Audio Stub] StopMusic()");
        }

        /// <summary>
        /// Sets SFX volume locally and logs the change.
        /// </summary>
        public void SetSfxVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            Logger.Log($"[Audio Stub] SfxVolume = {_sfxVolume}");
        }

        /// <summary>
        /// Sets music volume locally and logs the change.
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            Logger.Log($"[Audio Stub] MusicVolume = {_musicVolume}");
        }

        /// <summary>
        /// Not implemented in the stub. Throws <see cref="System.NotImplementedException"/>.
        /// </summary>
        public void PauseMusic()
        {
            throw new System.NotImplementedException("[Audio Stub] PauseMusic is not implemented.");
        }

        /// <summary>
        /// Not implemented in the stub. Throws <see cref="System.NotImplementedException"/>.
        /// </summary>
        public void ResumeMusic()
        {
            throw new System.NotImplementedException("[Audio Stub] ResumeMusic is not implemented.");
        }

        /// <summary>
        /// Not implemented in the stub. Throws <see cref="System.NotImplementedException"/>.
        /// </summary>
        public void StopSfx(string key)
        {
            throw new System.NotImplementedException("[Audio Stub] StopSfx is not implemented.");
        }

        /// <summary>
        /// Not implemented in the stub. Throws <see cref="System.NotImplementedException"/>.
        /// </summary>
        public void PauseAllSounds()
        {
            throw new System.NotImplementedException("[Audio Stub] PauseAllSounds is not implemented.");
        }

        /// <summary>
        /// Not implemented in the stub. Throws <see cref="System.NotImplementedException"/>.
        /// </summary>
        public void ResumeAllSounds()
        {
            throw new System.NotImplementedException("[Audio Stub] ResumeAllSounds is not implemented.");
        }

        /// <summary>
        /// Not implemented in the stub. Throws <see cref="System.NotImplementedException"/>.
        /// </summary>
        public void MuteAll(bool mute)
        {
            throw new System.NotImplementedException("[Audio Stub] MuteAll is not implemented.");
        }

        /// <summary>
        /// Not implemented in the stub. Throws <see cref="System.NotImplementedException"/>.
        /// </summary>
        public bool IsMusicPlaying()
        {
            throw new System.NotImplementedException("[Audio Stub] IsMusicPlaying is not implemented.");
        }

        /// <summary>
        /// Not implemented in the stub. Throws <see cref="System.NotImplementedException"/>.
        /// </summary>
        public bool IsSfxPlaying(string key)
        {
            throw new System.NotImplementedException("[Audio Stub] IsSfxPlaying is not implemented.");
        }
    }
}