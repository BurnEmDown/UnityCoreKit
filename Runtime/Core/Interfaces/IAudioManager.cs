namespace Core.Interfaces
{
    /// <summary>
    /// Provides methods for managing audio playback, including sound effects and music.
    /// </summary>
    public interface IAudioManager
    {
        /// <summary>
        /// Plays a sound effect identified by the specified key.
        /// </summary>
        /// <param name="key">The key identifying the sound effect to play.</param>
        void PlaySfx(string key);

        /// <summary>
        /// Plays music identified by the specified key, with an option to loop.
        /// </summary>
        /// <param name="key">The key identifying the music to play.</param>
        /// <param name="loop">Indicates whether the music should loop. Default is true.</param>
        void PlayMusic(string key, bool loop = true);

        /// <summary>
        /// Stops the currently playing music.
        /// </summary>
        void StopMusic();

        /// <summary>
        /// Sets the volume for sound effects.
        /// </summary>
        /// <param name="volume">The volume level for sound effects, ranging from 0 to 1.</param>
        void SetSfxVolume(float volume);

        /// <summary>
        /// Sets the volume for music.
        /// </summary>
        /// <param name="volume">The volume level for music, ranging from 0 to 1.</param>
        void SetMusicVolume(float volume);

        /// <summary>
        /// Pauses the currently playing music.
        /// </summary>
        void PauseMusic();

        /// <summary>
        /// Resumes the paused music.
        /// </summary>
        void ResumeMusic();

        /// <summary>
        /// Stops a specific sound effect identified by the specified key.
        /// </summary>
        /// <param name="key">The key identifying the sound effect to stop.</param>
        void StopSfx(string key);

        /// <summary>
        /// Pauses all currently playing sounds, including music and sound effects.
        /// </summary>
        void PauseAllSounds();

        /// <summary>
        /// Resumes all paused sounds, including music and sound effects.
        /// </summary>
        void ResumeAllSounds();

        /// <summary>
        /// Mutes or unmutes all audio.
        /// </summary>
        /// <param name="mute">True to mute all audio, false to unmute.</param>
        void MuteAll(bool mute);

        /// <summary>
        /// Checks if music is currently playing.
        /// </summary>
        /// <returns>True if music is playing, otherwise false.</returns>
        bool IsMusicPlaying();

        /// <summary>
        /// Checks if a specific sound effect identified by the key is currently playing.
        /// </summary>
        /// <param name="key">The key identifying the sound effect.</param>
        /// <returns>True if the sound effect is playing, otherwise false.</returns>
        bool IsSfxPlaying(string key);
    }
}