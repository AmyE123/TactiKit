namespace CT6GAMAI
{
    using UnityEngine;

    /// <summary>
    /// A scriptable object of all cursor audio clips
    /// </summary>
    [CreateAssetMenu(fileName = "CursorAudioClips", menuName = "ScriptableObjects/Audio/CursorSFX", order = 1)]
    public class CursorAudioClips : ScriptableObject
    {
        /// <summary>
        /// The audio clip for confirming a selection.
        /// </summary>
        public AudioClip Confirm;

        /// <summary>
        /// The audio clip for cancelling a selection.
        /// </summary>
        public AudioClip Cancel;

        /// <summary>
        /// The audio clip for an invalid selection.
        /// </summary>
        public AudioClip Invalid;

        /// <summary>
        /// The audio clip for moving the cursor without anything selected.
        /// </summary>
        public AudioClip Move1;

        /// <summary>
        /// The audio clip for moving the cursor with something selected.
        /// </summary>
        public AudioClip Move2;

        /// <summary>
        /// The audio clip for when a unit is selected.
        /// </summary>
        public AudioClip SelectUnit;
    }
}