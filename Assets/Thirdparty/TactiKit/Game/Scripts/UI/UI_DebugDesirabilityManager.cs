namespace CT6GAMAI
{
    using TMPro;
    using UnityEngine;

    public class UI_DebugDesirabilityManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playstyleValue;
        [SerializeField] private TMP_Text _fortValue;
        [SerializeField] private TMP_Text _retreatValue;
        [SerializeField] private TMP_Text _attackValue;
        [SerializeField] private TMP_Text _waitValue;

        public void PopulateDebugDesirability(UnitAIManager unit)
        {
            _playstyleValue.text = unit.Playstyle.Playstyle.ToString();
            _fortValue.text = AIDesirabilityCalculator.CalculateFortDesirability(unit).ToString();
            _retreatValue.text = AIDesirabilityCalculator.CalculateRetreatDesirability(unit).ToString();
            _attackValue.text = AIDesirabilityCalculator.CalculateAttackDesirability(unit).ToString();
            _waitValue.text = AIDesirabilityCalculator.CalculateWaitDesirability(unit).ToString();
        }
    }

}