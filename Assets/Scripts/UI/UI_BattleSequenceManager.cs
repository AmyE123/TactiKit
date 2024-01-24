namespace CT6GAMAI
{
    using UnityEngine;

    /// <summary>
    /// Manages the UI elements for displaying battle sequence information.
    /// </summary>
    public class UI_BattleSequenceManager : MonoBehaviour
    {
        [SerializeField] private UI_BattleSequenceSideManager[] _battleSequenceSideManagers;
        [SerializeField] private BattleManager _battleManager;

        /// <summary>
        /// Gets and populates battle data for the battle sequence UI for both units involved in the battle.
        /// </summary>
        /// <param name="leftUnit">The left unit.</param>
        /// <param name="rightUnit">The right unit.</param>
        public void GetValuesForBattleSequenceUI(UnitManager leftUnit, UnitManager rightUnit)
        {           
            PopulateBattleSequenceUIValues(0, leftUnit, _battleManager.AttackerAtk, _battleManager.AttackerHit, _battleManager.AttackerCrit);
            PopulateBattleSequenceUIValues(1, rightUnit, _battleManager.DefenderAtk, _battleManager.DefenderHit, _battleManager.DefenderCrit);            
        }

        public void CriticalPopup(int sideIdx)
        {
            StartCoroutine(_battleSequenceSideManagers[sideIdx].ShowCritPopup());
        }

        /// <summary>
        /// Populates the battle sequence UI for a specific side with data from a unit.
        /// </summary>
        public void PopulateBattleSequenceUIValues(int sideIdx, UnitManager unit, int attack, int hitRate, int critRate)
        {
            _battleSequenceSideManagers[sideIdx].PopulateBattleSequenceUIData(unit, attack, hitRate, critRate);
        }
    }
}