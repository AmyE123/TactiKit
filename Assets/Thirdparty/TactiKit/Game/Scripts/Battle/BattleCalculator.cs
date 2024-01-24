namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// A class for calculating and determining battle outcomes
    /// </summary>
    public class BattleCalculator : MonoBehaviour
    {
        private static int CalculatePhysicalAttackPower(UnitManager attacker, UnitManager defender)
        {
            int attackStrength = attacker.UnitData.StrengthBaseValue + attacker.UnitData.EquippedWeapon.WeaponMight;
            int defenseStrength = defender.UnitData.DefenseBaseValue + CalculateTerrainDefenseBonus(defender);

            int attackPower = attackStrength - defenseStrength;

            if (attackPower <= 0)
            {
                attackPower = 0;
            }

            return attackPower;
        }

        private static int CalculateMagicAttackPower(UnitManager attacker, UnitManager defender)
        {
            int attackStrength = attacker.UnitData.MagicBaseValue + attacker.UnitData.EquippedWeapon.WeaponMight;
            int defenseStrength = defender.UnitData.ResistanceBaseValue + CalculateTerrainDefenseBonus(defender);

            int attackPower = attackStrength - defenseStrength;

            if (attackPower <= 0)
            {
                attackPower = 0;
            }

            return attackPower;
        }

        private static int CalculateTerrainDefenseBonus(UnitManager unit)
        {
            return unit.StoodNode.NodeData.TerrainType.DefenseBoost;
        }

        private static int CalculateWeaponTriangeFactor(UnitData attacker, UnitData defender)
        {
            var unitA_WeaponType = attacker.EquippedWeapon.WeaponType;
            var unitB_WeaponType = defender.EquippedWeapon.WeaponType;

            return (unitA_WeaponType, unitB_WeaponType) switch
            {
                // Axe bonuses
                (WeaponType.Axe, WeaponType.Lance) => +WEAPON_TRIANGLE_BONUS,
                (WeaponType.Axe, WeaponType.Sword) => -WEAPON_TRIANGLE_BONUS,
                (WeaponType.Axe, WeaponType.Tome) => -WEAPON_TRIANGLE_BONUS,

                // Lance bonuses
                (WeaponType.Lance, WeaponType.Axe) => -WEAPON_TRIANGLE_BONUS,
                (WeaponType.Lance, WeaponType.Sword) => +WEAPON_TRIANGLE_BONUS,
                (WeaponType.Lance, WeaponType.Tome) => +WEAPON_TRIANGLE_BONUS,

                // Sword bonuses
                (WeaponType.Sword, WeaponType.Axe) => +WEAPON_TRIANGLE_BONUS,
                (WeaponType.Sword, WeaponType.Lance) => -WEAPON_TRIANGLE_BONUS,

                // Tome bonuses
                (WeaponType.Tome, WeaponType.Axe) => +WEAPON_TRIANGLE_BONUS,
                (WeaponType.Tome, WeaponType.Lance) => -WEAPON_TRIANGLE_BONUS,

                // Default for same weapon types
                _ => 0
            };
        }

        /// <summary>
        /// Determines whether the attacker can do a double attack on the defender.
        /// </summary>
        /// <param name="attacker">The attacking unit.</param>
        /// <param name="defender">The defending unit.</param>
        /// <returns>True if the attacker can double attack.</returns>
        public static bool CanDoubleAttack(UnitManager attacker, UnitManager defender)
        {
            return (defender.UnitData.SpeedBaseValue + DOUBLE_ATK_SPEED_THRESHOLD) <= attacker.UnitData.SpeedBaseValue;
        }

        /// <summary>
        /// Returns the units power amount. Used for Utility Theory.
        /// </summary>
        public static int CalculatePower(UnitManager unit)
        {
            return unit.UnitData.AttackBaseValue + unit.UnitData.EquippedWeapon.WeaponMight;
        }

        /// <summary>
        /// Returns the units power amount with weapon triange factored in. Used for Utility Theory.
        /// </summary>
        public static int CalculatePower(UnitManager attacker, UnitManager defender)
        {
            int weaponFactor = CalculateWeaponTriangeFactor(attacker.UnitData, defender.UnitData);
            return attacker.UnitData.AttackBaseValue + attacker.UnitData.EquippedWeapon.WeaponMight + weaponFactor;
        }

        /// <summary>
        /// Calculates the attack power of a unit against another unit.
        /// </summary>
        /// <param name="attacker">The attacking unit.</param>
        /// <param name="defender">The defending unit.</param>
        /// <returns>The calculated attack power.</returns>
        public static int CalculateAttackPower(UnitManager attacker, UnitManager defender)
        {
            var weaponType = attacker.UnitData.EquippedWeapon.WeaponType;

            if (weaponType == WeaponType.Tome)
            {
                return CalculateMagicAttackPower(attacker, defender);
            }

            return CalculatePhysicalAttackPower(attacker, defender);
        }

        /// <summary>
        /// Calculates the critical rate of an attacker against a defender.
        /// </summary>
        /// <param name="attacker">The attacking unit.</param>
        /// <param name="defender">The defending unit.</param>
        /// <returns>The critical rate percentage.</returns>
        public static int CalculateCriticalRatePercentage(UnitManager attacker, UnitManager defender)
        {
            int criticalStrength = attacker.UnitData.CriticalRateBaseValue + (attacker.UnitData.SkillBaseValue / CRIT_SKILL_DIVIDER);
            int defenseStrength = defender.UnitData.LuckBaseValue;

            int critRate = criticalStrength - defenseStrength;
            critRate = Mathf.Clamp(critRate, 0, 100);

            return critRate;
        }

        /// <summary>
        /// Calculates the hit rate percentage of an attacker against a defender, taking the weapon triangle into account.
        /// </summary>
        /// <param name="attacker">The attacking unit.</param>
        /// <param name="defender">The defending unit.</param>
        /// <returns>The hit rate percentage.</returns>
        public static int CalculateHitRatePercentage(UnitManager attacker, UnitManager defender)
        {
            int weaponTriangleFactor = CalculateWeaponTriangeFactor(attacker.UnitData, defender.UnitData);
            int hitStrength = attacker.UnitData.HitRateBaseValue + weaponTriangleFactor;
            int defenseStrength = defender.UnitData.AvoidRateBaseValue;

            int hitRate = hitStrength - defenseStrength;
            hitRate = Mathf.Clamp(hitRate, 0, 100);

            return hitRate;
        }

        /// <summary>
        /// Calculates the remaining health points of a unit after an attack for the forecast UI.
        /// </summary>
        /// <param name="unit">The unit to calculate the remaining health for.</param>
        /// <param name="attackAmount">The amount of the attack.</param>
        /// <param name="canDoubleAttack">Indicates whether a double attack can occur.</param>
        /// <returns>The forecasted remaining HP.</returns>
        public static int CalculateRemainingHPForecast(UnitManager unit, int attackAmount, bool canDoubleAttack)
        {
            int attackValue = canDoubleAttack ? attackAmount * DOUBLE_ATK_MULTIPLIER : attackAmount;

            return unit.UnitStatsManager.HealthPoints - attackValue;
        }

        /// <summary>
        /// Does a critical hit roll.
        /// </summary>
        /// <param name="criticalPercentage">The critical hit percentage chance.</param>
        /// <returns>True if a critical hit happens.</returns>
        public static bool CriticalRoll(int criticalPercentage)
        {
            bool critHit = Roll(criticalPercentage);

            return critHit;
        }

        /// <summary>
        /// Does a hit roll. Determines whether a unit can land the attack.
        /// </summary>
        /// <param name="hitPercentage">The hit percentage chance.</param>
        /// <returns>True if a hit happens.</returns>
        public static bool HitRoll(int hitPercentage)
        {
            bool hit = Roll(hitPercentage);

            return hit;
        }

        /// <summary>
        /// Does a percentage-based roll based off values 0 to 100.
        /// </summary>
        /// <param name="percentage">The percentage chance for the roll to succeed.</param>
        /// <returns>True if the roll is successful.</returns>
        public static bool Roll(int percentage)
        {
            int roll = Random.Range(0, 100);

            return roll <= percentage;
        }
    }
}