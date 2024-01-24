namespace CT6GAMAI
{
    using System.Collections;
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manages the music for different phases of the game.
    /// </summary>
    public class TurnMusicManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _enemyPhaseMusic;
        [SerializeField] private AudioClip _playerPhaseMusic;
        [SerializeField] private AudioClip _deathMusic;
        [SerializeField] private AudioClip _victoryMusic;
        [SerializeField] private bool _isPlayerMusic;
        [SerializeField] private bool _playVictoryMusic = false;
        [SerializeField] private bool _isPlayingVictoryMusic = false;
        [SerializeField] private bool _forcePlayDeathMusic = false;

        /// <summary>
        /// A boolean for playing victory music
        /// </summary>
        public bool PlayVictoryMusic { get { return _playVictoryMusic; } set { _playVictoryMusic = value; } }

        public void Update()
        {
            CheckForVictoryMusic();
        }

        /// <summary>
        /// Plays the player phase music
        /// </summary>
        public void PlayPlayerPhaseMusic()
        {
            if (!_forcePlayDeathMusic)
            {
                _isPlayerMusic = true;
                if (!_playVictoryMusic)
                {
                    StartCoroutine(CrossfadeMusic(_playerPhaseMusic));
                }
            }    
        }

        /// <summary>
        /// Plays the enemy phase music
        /// </summary>
        public void PlayEnemyPhaseMusic()
        {
            if (!_forcePlayDeathMusic)
            {
                if (!_playVictoryMusic)
                {
                    _isPlayerMusic = false;
                    StartCoroutine(CrossfadeMusic(_enemyPhaseMusic));
                }
            }
        }

        /// <summary>
        /// Plays the death music
        /// </summary>
        public void PlayDeathMusic(bool isEndGame = false)
        {
            if (!_playVictoryMusic)
            {
                _forcePlayDeathMusic = isEndGame;
                StartCoroutine(CrossfadeMusic(_deathMusic));
            }

            if (isEndGame)
            {
                _forcePlayDeathMusic = isEndGame;
                StartCoroutine(CrossfadeMusic(_deathMusic));
            }
        }

        /// <summary>
        /// Check for the victory music
        /// </summary>
        public void CheckForVictoryMusic()
        {
            if (!_forcePlayDeathMusic)
            {
                if (PlayVictoryMusic && !_isPlayingVictoryMusic)
                {
                    _isPlayingVictoryMusic = true;
                    StartCoroutine(CrossfadeMusic(_victoryMusic));
                }
            }     
        }

        /// <summary>
        /// Resumes the last playing phase music. 
        /// Used for after playing other music events.
        /// </summary>
        public void ResumeLastPhaseMusic(Team deathTeam)
        {
            if (!_forcePlayDeathMusic)
            {
                if (deathTeam != Team.Enemy)
                {
                    if (_isPlayerMusic)
                    {
                        PlayPlayerPhaseMusic();
                    }
                    else
                    {
                        PlayEnemyPhaseMusic();
                    }
                }
            }
        }

        private IEnumerator CrossfadeMusic(AudioClip newClip)
        {
            if (_audioSource.clip != null)
            {
                // Fade out current clip
                float startVolume = _audioSource.volume;
                for (float t = 0; t < CROSSFADE_DURATION; t += Time.deltaTime)
                {
                    _audioSource.volume = Mathf.Lerp(startVolume, 0, t / CROSSFADE_DURATION);
                    yield return null;
                }
            }

            // Change the clip and fade in
            _audioSource.clip = newClip;
            _audioSource.Play();
            for (float t = 0; t < CROSSFADE_DURATION; t += Time.deltaTime)
            {
                _audioSource.volume = Mathf.Lerp(0, 1, t / CROSSFADE_DURATION);
                yield return null;
            }
        }
    }

}