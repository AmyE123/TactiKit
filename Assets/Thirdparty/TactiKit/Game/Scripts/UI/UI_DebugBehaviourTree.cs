namespace CT6GAMAI
{
    using TMPro;
    using UnityEngine;

    public class UI_DebugBehaviourTree : MonoBehaviour
    {
        [SerializeField] private TMP_Text _activeAIUnitValue;
        [SerializeField] private TMP_Text _activeSequenceValue;
        [SerializeField] private TMP_Text _activeActionValue;
        [SerializeField] private TMP_Text _activeActionStateValue;

        public void UpdateActiveAIUnit(string activeAIName)
        {
            _activeAIUnitValue.text = activeAIName;
        }

        public void UpdateActiveSequence(string sequenceName)
        {
            _activeSequenceValue.text = sequenceName;
        }

        public void UpdateActiveAction(string actionName)
        {
            _activeActionValue.text = actionName;
        }

        public void UpdateActiveActionState(Constants.BTNodeState activeAction)
        {
            _activeActionStateValue.text = activeAction.ToString();
        }
    }
}