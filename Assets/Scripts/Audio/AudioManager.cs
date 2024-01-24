namespace CT6GAMAI
{
    using UnityEngine;

    /// <summary>
    /// This class manages functionality for audio/sfx within the game.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _cursorAudioSource;
        [SerializeField] private CursorAudioClips _cursorAudioClips;

        /// <summary>
        /// Plays a cursor sound effect based on the unit selection state.
        /// </summary>
        /// <param name="isUnitPressed">Indicates whether a unit is currently selected.</param>
        public void PlayCursorSound(bool isUnitPressed)
        {
            if (!_cursorAudioSource.isPlaying)
            {
                _cursorAudioSource.PlayOneShot(isUnitPressed ? _cursorAudioClips.Move2 : _cursorAudioClips.Move1);
            }
        }

        /// <summary>
        /// Plays a sound effect when toggling the selection of a unit.
        /// </summary>
        /// <param name="isUnitPressed">Indicates whether a unit is currently selected.</param>
        public void PlayToggleUnitSound(bool isUnitPressed)
        {
            if (!_cursorAudioSource.isPlaying)
            {
                _cursorAudioSource.PlayOneShot(isUnitPressed ? _cursorAudioClips.SelectUnit : _cursorAudioClips.Cancel);
            }
        }

        /// <summary>
        /// Plays an appropriate sound effect based on whether the path is valid.
        /// </summary>
        /// <param name="isValidPath">Indicates whether the selected path is valid.</param>
        public void PlaySelectPathSound(bool isValidPath)
        {
            if (!_cursorAudioSource.isPlaying)
            {
                _cursorAudioSource.PlayOneShot(isValidPath ? _cursorAudioClips.Confirm : _cursorAudioClips.Invalid);
            }
        }
    }
}