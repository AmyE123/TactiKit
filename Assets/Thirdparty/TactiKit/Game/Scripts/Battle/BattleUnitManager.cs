namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manages the battle properties and behaviours of a unit in the game.
    /// </summary>
    public class BattleUnitManager : MonoBehaviour
    {
        [Header("Unit Identification")]
        [SerializeField] private Side _unitSide;
        [SerializeField] private UnitManager _unitManagerRef;
        [SerializeField] private UnitStatsManager _unitStatsManagerRef;

        [Header("Unit Animation")]
        [SerializeField] private Animator _animator;
        [SerializeField] private bool _ignoreAnimation;

        [Header("Unit Status")]
        [SerializeField] private bool _canUnitAttackAgain;
        [SerializeField] private bool _unitCompleteAttacks;

        /// <summary>
        /// Gets the side which the unit is on.
        /// </summary>
        public Side UnitSide => _unitSide;

        /// <summary>
        /// Gets the animator commonent attached to the battle unit.
        /// </summary>
        public Animator Animator => _animator;

        /// <summary>
        /// Gets the UnitManager reference for the unit.
        /// </summary>
        public UnitManager UnitManagerRef => _unitManagerRef;

        /// <summary>
        /// Gets the UnitStatsManager reference for the unit.
        /// </summary>
        public UnitStatsManager UnitStatsManager => _unitStatsManagerRef;

        /// <summary>
        /// Whether the unit can attack again.
        /// </summary>
        public bool CanUnitAttackAgain => _canUnitAttackAgain;

        /// <summary>
        /// Whether the uni has completed all of its attacks.
        /// </summary>
        public bool UnitCompleteAttacks => _unitCompleteAttacks;

        public bool IgnoreAnimation => _ignoreAnimation;

        private void ClearExistingChildren()
        {
            int children = transform.childCount;

            for (int i = children - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        private void SetUnitStatsAndReferences(UnitStatsManager unitStats, UnitManager unitManager)
        {
            _unitStatsManagerRef = unitStats;
            _unitManagerRef = unitManager;
            _canUnitAttackAgain = _unitStatsManagerRef.DblAtk;
        }

        private void InstantiateBattleUnit()
        {
            GameObject battleGO = Instantiate(_unitManagerRef.BattleUnit, transform);
            _animator = battleGO.GetComponentInChildren<Animator>();

            _ignoreAnimation = _animator == null;
        }

        /// <summary>
        /// Sets the status for whether the unit has completed all of its attacks.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void SetUnitCompleteAttacks(bool value)
        {
            _unitCompleteAttacks = value;
        }

        /// <summary>
        /// Sets the references for the unit's statistics and manager, and initializes the unit for battle.
        /// </summary>
        /// <param name="unitStats">The UnitStatsManager reference.</param>
        /// <param name="unitManager">The UnitManager reference.</param>
        public void SetUnitReferences(UnitStatsManager unitStats, UnitManager unitManager)
        {
            ClearExistingChildren();
            SetUnitStatsAndReferences(unitStats, unitManager);
            InstantiateBattleUnit();
        }
    }
}