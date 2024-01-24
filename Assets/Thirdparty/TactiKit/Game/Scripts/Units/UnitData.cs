namespace CT6GAMAI
{
    using UnityEngine;

    /// <summary>
    /// Fire Emblem: Fates is my main inspiration for this AI project, so all
    /// mechanics have similar/the same documentation as the in-game descriptions.
    /// </summary>
    [CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/Units/UnitData", order = 1)]
    public class UnitData : ScriptableObject
    {       
        /// <summary>
        /// Unit's name. Displayed in-game.
        /// </summary>
        public string UnitName = "Unassigned";

        /// <summary>
        /// Unit's portrait.
        /// </summary>
        public Sprite UnitPortrait;

        /// <summary>
        /// Unit's team.
        /// </summary>
        public Constants.Team UnitTeam = Constants.Team.Player;

        /// <summary>
        /// Unit's class. Determines stats.
        /// </summary>
        public Constants.Class UnitClass = Constants.Class.Knight;

        /// <summary>
        /// Unit's equipped weapon.
        /// </summary>
        public WeaponData EquippedWeapon;

        /// <summary>
        /// Shows how seasoned a unit is in his or her current class.
        /// </summary>
        public int ClassLevel = 0;

        #region Battle Stats

        /// <summary>
        /// Base value health points.
        /// </summary>
        public int HealthPointsBaseValue = 0;

        /// <summary>
        /// Movement. Determines how many spaces a unit can move.
        /// </summary>
        public int MovementBaseValue = 0;

        /// <summary>
        /// Attack. The power of physical or magical attacks.
        /// </summary>
        public int AttackBaseValue = 0;

        /// <summary>
        /// Critical Rate. Affects the odds of getting a critical hit.
        /// </summary>
        public int CriticalRateBaseValue = 0;

        /// <summary>
        /// Hit Rate. Affects the odds of hitting an enemy unit.
        /// </summary>
        public int HitRateBaseValue = 0;

        /// <summary>
        /// Avoid Rate. Affects the unit's odds of evading attacks.
        /// </summary>
        public int AvoidRateBaseValue = 0;

        /// <summary>
        /// Strength. Affects damage the unit deals with physical attacks.
        /// </summary>
        public int StrengthBaseValue = 0;

        /// <summary>
        /// Speed. Affects Avoid Rate. Unit strikes twice if 5 higher than opponent.
        /// </summary>
        public int SpeedBaseValue = 0;

        /// <summary>
        /// Magic. Affects damage the unit deals with magical attacks.
        /// </summary>
        public int MagicBaseValue = 0;

        /// <summary>
        /// Luck. Has various effects. Lowers risk of enemy criticals.
        /// </summary>
        public int LuckBaseValue = 0;

        /// <summary>
        /// Skill. Affects hit rate and the frequency of critical hits.
        /// </summary>
        public int SkillBaseValue = 0;

        /// <summary>
        /// Defense. Reduces damage from physical attacks.
        /// </summary>
        public int DefenseBaseValue = 0;

        /// <summary>
        /// Resistance. Reduces damage from magical attacks.
        /// </summary>
        public int ResistanceBaseValue = 0;

        /// <summary>
        /// The sum of the unit's key stats.
        /// </summary>
        public int RatingValue = 0;

        #endregion //Battle Stats
    }
}