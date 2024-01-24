namespace CT6GAMAI
{
    using DG.Tweening;
    using System.Collections;
    using System.Text.RegularExpressions;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manages the UI for different phases.
    /// </summary>
    public class UI_PhaseManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _phaseText;
        [SerializeField] private Image _vignette;

        private void PopulatePhaseUI(Phases phase)
        {
            var formattedPhaseName = Regex.Replace(phase.ToString(), "(\\B[A-Z])", " $1");

            _phaseText.text = formattedPhaseName;

            if (phase == Phases.PlayerPhase)
            {
                _phaseText.color = UI_PlayerColour;
                _vignette.color = UI_PlayerColour;
            }
            else
            {
                _phaseText.color = UI_EnemyColour;
                _vignette.color = UI_EnemyColour;
            }
        }

        private IEnumerator AnimatePhaseUI(float delay)
        {
            ShowPhaseUI();

            yield return new WaitForSeconds(delay);

            HidePhaseUI();
        }

        private void ShowPhaseUI()
        {
            _canvasGroup.DOFade(1, 1f);
        }

        private void HidePhaseUI()
        {
            _canvasGroup.DOFade(0, 1f);
        }

        /// <summary>
        /// Displays the UI for a phase.
        /// </summary>
        /// <param name="phase">The phase to display in the UI.</param>
        public void DisplayPhaseUI(Phases phase)
        {
            PopulatePhaseUI(phase);
            StartCoroutine(AnimatePhaseUI(PHASE_UI_DELAY));
        }
    }
}