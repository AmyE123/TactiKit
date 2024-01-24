namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manages battle-related functionalities and interactions between units.
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private BattleSequenceManager _battleSequenceManager;

        [Header("Current Battle Stats - Attacker (Initiator)")]
        [SerializeField] private UnitManager _attackingUnit;
        [SerializeField] private int _attackerAtk;
        [SerializeField] private bool _attackerDblAtk;
        [SerializeField] private int _attackerHit;
        [SerializeField] private int _attackerCrit;
        [SerializeField] private int _attackerRemainingHP;

        [Header("Current Battle Stats - Defender (Opponent)")]
        [SerializeField] private UnitManager _defendingUnit;
        [SerializeField] private int _defenderAtk;
        [SerializeField] private bool _defenderDblAtk;
        [SerializeField] private int _defenderHit;
        [SerializeField] private int _defenderCrit;
        [SerializeField] private int _defenderRemainingHP;

        private GameManager _gameManager;
        private CameraManager _cameraManager;
        private bool _isBattleActive;
        private Team _initiatingTeam;

        /// <summary>
        /// The attack amount from the attacker for the current battle stats.
        /// </summary>
        public int AttackerAtk => _attackerAtk;

        /// <summary>
        /// Whether the attacker can do a double attack for the current battle stats.
        /// </summary>
        public bool AttackerDblAtk => _attackerDblAtk;

        /// <summary>
        /// The hit amount from the attacker for the current battle stats.
        /// </summary>
        public int AttackerHit => _attackerHit;

        /// <summary>
        /// The crit amount from the attacker for the current battle stats.
        /// </summary>
        public int AttackerCrit => _attackerCrit;

        /// <summary>
        /// The remaining HP of the attacker for the current battle stats.
        /// </summary>
        public int AttackerRemainingHP => _attackerRemainingHP;

        /// <summary>
        /// The attack amount from the defender for the current battle stats.
        /// </summary>
        public int DefenderAtk => _defenderAtk;

        /// <summary>
        /// Whether the defender can do a double attack for the current battle stats.
        /// </summary>
        public bool DefenderDblAtk => _defenderDblAtk;

        /// <summary>
        /// The hit amount from the defender for the current battle stats.
        /// </summary>
        public int DefenderHit => _defenderHit;

        /// <summary>
        /// The crit amount from the defender for the current battle stats.
        /// </summary>
        public int DefenderCrit => _defenderCrit;

        /// <summary>
        /// The remaining HP of the defender for the current battle stats.
        /// </summary>
        public int DefenderRemainingHP => _defenderRemainingHP;

        /// <summary>
        /// Whether a battle is currently active.
        /// </summary>
        public bool IsBattleActive => _isBattleActive;

        private void Start()
        {
            InitializeValues();
        }

        private void InitializeValues()
        {
            _gameManager = GameManager.Instance;
            _cameraManager = _gameManager.CameraManager;
        }

        private void SetUnitStats(UnitStatsManager unitStats, int atk, bool dblAtk, int hit, int crit)
        {
            unitStats.SetUnitStats(atk, dblAtk, hit, crit);
        }

        /// <summary>
        /// Switches to battle mode when battle is initiated.
        /// </summary>
        public void SwitchToBattle(Team initiatingTeam)
        {
            _cameraManager.SwitchCamera(_cameraManager.Cameras[(int)CameraStates.Battle]);
            _isBattleActive = true;
            _battleSequenceManager.StartBattle(_attackingUnit, _defendingUnit, initiatingTeam);

            _attackingUnit.IsInBattle = true;
            _defendingUnit.IsInBattle = true;

            _initiatingTeam = initiatingTeam;
        }

        /// <summary>
        /// Handles setting values at the end of a battle.
        /// </summary>
        public void HandleEndOfBattleUnit()
        {
            if (_initiatingTeam == Team.Player)
            {
                _attackingUnit.FinalizeMovementValues();
                _attackingUnit.IsAwaitingMoveConfirmation = false;
            }
            else
            {
                _defendingUnit.FinalizeMovementValues();
            }
        }

        /// <summary>
        /// Switches to map mode when battle is over.
        /// </summary>
        public void SwitchToMap()
        {
            _cameraManager.SwitchCamera(_cameraManager.Cameras[(int)CameraStates.Map]);
            _isBattleActive = false;

            _attackingUnit.IsInBattle = false;
            _defendingUnit.IsInBattle = false;

            HandleEndOfBattleUnit();

            _gameManager.UIManager.ActionItemsManager.HideActionItems();
            _gameManager.UIManager.BattleForecastManager.CancelBattleForecast();
        }

        /// <summary>
        /// Calculates and sets the battle forecast values for the attacker and defender.
        /// </summary>
        /// <param name="leftUnit">The left unit. (Typically player unit)</param>
        /// <param name="rightUnit">The right unit. (Typically enemy unit)</param>
        public void CalculateValuesForBattleForecast(UnitManager leftUnit, UnitManager rightUnit)
        {
            _attackerAtk = BattleCalculator.CalculateAttackPower(leftUnit, rightUnit);
            _attackerDblAtk = BattleCalculator.CanDoubleAttack(leftUnit, rightUnit);
            _attackerHit = BattleCalculator.CalculateHitRatePercentage(leftUnit, rightUnit);
            _attackerCrit = BattleCalculator.CalculateCriticalRatePercentage(leftUnit, rightUnit);

            _defenderAtk = BattleCalculator.CalculateAttackPower(rightUnit, leftUnit);
            _defenderDblAtk = BattleCalculator.CanDoubleAttack(rightUnit, leftUnit);
            _defenderHit = BattleCalculator.CalculateHitRatePercentage(rightUnit, leftUnit);
            _defenderCrit = BattleCalculator.CalculateCriticalRatePercentage(rightUnit, leftUnit);

            _attackerRemainingHP = BattleCalculator.CalculateRemainingHPForecast(leftUnit, DefenderAtk, DefenderDblAtk);
            _defenderRemainingHP = BattleCalculator.CalculateRemainingHPForecast(rightUnit, AttackerAtk, AttackerDblAtk);

            _gameManager.BattleManager.SetBattleStats(leftUnit, AttackerAtk, AttackerDblAtk, AttackerHit, AttackerCrit, AttackerRemainingHP, rightUnit, DefenderAtk, DefenderDblAtk, DefenderHit, DefenderCrit, DefenderRemainingHP);
        }

        /// <summary>
        /// Sets the battle stats for both attacking and defending units.
        /// </summary>
        /// <param name="unitA">The attacking unit.</param>
        /// <param name="atkA">Attack value for the attacking unit.</param>
        /// <param name="dblAtkA">Whether the attacking unit can double attack.</param>
        /// <param name="hitA">Hit rate for the attacking unit.</param>
        /// <param name="critA">Critical rate for the attacking unit.</param>
        /// <param name="remainingHPA">Remaining HP for the attacking unit.</param>
        /// <param name="unitB">The defending unit.</param>
        /// <param name="atkB">Attack value for the defending unit.</param>
        /// <param name="dblAtkB">Whether the defending unit can double attack.</param>
        /// <param name="hitB">Hit rate for the defending unit.</param>
        /// <param name="critB">Critical rate for the defending unit.</param>
        /// <param name="remainingHPB">Remaining HP for the defending unit.</param>
        public void SetBattleStats(UnitManager unitA, int atkA, bool dblAtkA, int hitA, int critA, int remainingHPA, UnitManager unitB, int atkB, bool dblAtkB, int hitB, int critB, int remainingHPB)
        {
            _attackingUnit = unitA;
            _attackerAtk = atkA;
            _attackerDblAtk = dblAtkA;
            _attackerHit = hitA;
            _attackerCrit = critA;
            _attackerRemainingHP = remainingHPA;
            SetUnitStats(unitA.UnitStatsManager, atkA, dblAtkA, hitA, critA);

            _defendingUnit = unitB;
            _defenderAtk = atkB;
            _defenderDblAtk = dblAtkB;
            _defenderHit = hitB;
            _defenderCrit = critB;
            _defenderRemainingHP = remainingHPB;
            SetUnitStats(unitB.UnitStatsManager, atkB, dblAtkB, hitB, critB);
        }
    }
}