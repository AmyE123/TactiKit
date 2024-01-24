namespace CT6GAMAI
{
    using UnityEngine;
    using UnityEngine.UI;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manages the live stats of a unit.
    /// </summary>
    public class UnitStatsManager : MonoBehaviour
    {        
        [SerializeField] private UnitManager _unitManager;

        [SerializeField] private int _healthPoints;
        [SerializeField] private Image _healthBarFill;

        [SerializeField] private int _atk;
        [SerializeField] private bool _dblAtk;
        [SerializeField] private int _hit;
        [SerializeField] private int _crit;

        private UnitData _unitBaseData;

        /// <summary>
        /// Gets the base data of the unit.
        /// </summary>
        public UnitData UnitBaseData => _unitBaseData;

        /// <summary>
        /// Gets the current health points of the unit.
        /// </summary>
        public int HealthPoints => _healthPoints;

        /// <summary>
        /// Gets the attack stat of the unit.
        /// </summary>
        public int Atk => _atk;

        /// <summary>
        /// Gets whether the unit can perform a double attack.
        /// </summary>
        public bool DblAtk => _dblAtk;

        /// <summary>
        /// Gets the hit stat of the unit.
        /// </summary>
        public int Hit => _hit;

        /// <summary>
        /// Gets the critical hit stat of the unit.
        /// </summary>
        public int Crit => _crit;

        private void Start()
        {
            _unitBaseData = _unitManager.UnitData;
            _healthPoints = _unitBaseData.HealthPointsBaseValue;
        }

        private void Update() 
        {
            _healthBarFill.fillAmount = CalculateHealthPercentage(_healthPoints, _unitBaseData.HealthPointsBaseValue);
        }

        private float CalculateHealthPercentage(int currentHealth, int maxHealth)
        {
            return (float)currentHealth / maxHealth;
        }

        /// <summary>
        /// Adjusts the health points of the unit and checks the unit's health state.
        /// </summary>
        /// <param name="value">The value to adjust the health points by.</param>
        /// <returns>The updated health points after adjustment.</returns>
        public int AdjustHealthPoints(int value)
        {
            // Add the value to current health points and clamp it within the valid range
            _healthPoints = Mathf.Clamp(_healthPoints + value, 0, _unitBaseData.HealthPointsBaseValue);
            CheckHealthState();

            return _healthPoints;
        }

        /// <summary>
        /// Sets the health points of the unit to a specific value and checks the unit's health state.
        /// </summary>
        /// <param name="value">The value the HP should be set to.</param>
        /// <returns>The updated health points after adjustment.</returns>
        public int SetHealthPoints(int value)
        {
            // Add the value to current health points and clamp it within the valid range
            _healthPoints = Mathf.Clamp(value, 0, _unitBaseData.HealthPointsBaseValue);
            CheckHealthState();

            return _healthPoints;
        }

        /// <summary>
        /// Checks and returns the current health state of the unit.
        /// </summary>
        /// <returns>The current health state of the unit.</returns>
        public UnitHealthState CheckHealthState()
        {
            if (_healthPoints <= 0)
            {
                _unitManager.Death();
                return UnitHealthState.Dead;
            }
            else
            {
                return UnitHealthState.Alive;
            }
        }

        /// <summary>
        /// Sets the stats for the unit.
        /// </summary>
        public void SetUnitStats(int atk, bool dblAtk, int hit, int crit)
        {
            _atk = atk;
            _dblAtk = dblAtk;
            _hit = hit;
            _crit = crit;
        }
    }
}