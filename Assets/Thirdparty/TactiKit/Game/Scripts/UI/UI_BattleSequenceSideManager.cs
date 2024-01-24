namespace CT6GAMAI
{
    using DG.Tweening;
    using System.Collections;
    using System.Text.RegularExpressions;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Manages the UI components for one side of the battle sequence display.
    /// </summary>
    public class UI_BattleSequenceSideManager : MonoBehaviour
    {
        [Header("UI Configuration")]
        [SerializeField] private Constants.Side _side;

        [Header("UI Elements")]
        [SerializeField] private TMP_Text _equippedWeaponValueText;
        [SerializeField] private Image _equippedWeaponImage;
        [SerializeField] private TMP_Text _unitNameValueText;
        [SerializeField] private TMP_Text _currentHPValueText;
        [SerializeField] private TMP_Text _attackStatValueText;
        [SerializeField] private TMP_Text _hitStatValueText;
        [SerializeField] private TMP_Text _critStatValueText;
        [SerializeField] private Image _healthBarFill;
        [SerializeField] private RectTransform _criticalPopup;
        [SerializeField] private CanvasGroup _criticalCanvasGroup;

        [Header("Unit Reference")]
        [SerializeField] private UnitManager _activeUnitManager;

        private void Update()
        {
            if (_activeUnitManager != null)
            {
                UpdateHealthFill(_activeUnitManager);
            }
        }

        /// <summary>
        /// Updates the health bar based on the units current health.
        /// </summary>
        /// <param name="unit">The unit to update the health bar for.</param>
        private void UpdateHealthFill(UnitManager unit)
        {
            _currentHPValueText.text = unit.UnitStatsManager.HealthPoints.ToString();

            float healthFillAmount = CalculateHealthPercentage(unit.UnitStatsManager.HealthPoints, unit.UnitData.HealthPointsBaseValue);

            _healthBarFill.DOFillAmount(healthFillAmount, 0.1f).SetEase(Ease.Linear);
        }

        /// <summary>
        /// Calculates the health percentage for the health bar based on current and maximum health.
        /// </summary>
        /// <param name="currentHealth">The current health of the unit.</param>
        /// <param name="maxHealth">The maximum health of the unit.</param>
        /// <returns>The calculated health percentage.</returns>
        private float CalculateHealthPercentage(int currentHealth, int maxHealth)
        {
            return (float)currentHealth / maxHealth;
        }

        /// <summary>
        /// Shows a critical popup when a unit gets a critical hit in battle
        /// </summary>
        public IEnumerator ShowCritPopup()
        {
            _criticalCanvasGroup.alpha = 0;

            _criticalCanvasGroup.DOFade(1, 0.5f);

            _criticalPopup.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f, 10, 1);

            yield return new WaitForSeconds(2);

            _criticalCanvasGroup.DOFade(0, 0.5f);
        }

        /// <summary>
        /// Populates the UI with data from a unit for the battle sequence.
        /// </summary>
        public void PopulateBattleSequenceUIData(UnitManager unit, int attackValue, int hitValue, int critValue)
        {
            _activeUnitManager = unit;

            _unitNameValueText.text = unit.UnitData.UnitName;

            var formattedWeaponName = Regex.Replace(unit.UnitData.EquippedWeapon.WeaponName.ToString(), "(\\B[A-Z])", " $1");
            _equippedWeaponValueText.text = formattedWeaponName;
            _equippedWeaponImage.sprite = unit.UnitData.EquippedWeapon.WeaponSprite;

            _currentHPValueText.text = unit.UnitStatsManager.HealthPoints.ToString();

            _attackStatValueText.text = attackValue.ToString();

            _hitStatValueText.text = hitValue.ToString() + "%";

            _critStatValueText.text = critValue.ToString() + "%";

            float healthFillAmount = CalculateHealthPercentage(unit.UnitStatsManager.HealthPoints, unit.UnitData.HealthPointsBaseValue);

            _healthBarFill.fillAmount = healthFillAmount;
        }
    }
}