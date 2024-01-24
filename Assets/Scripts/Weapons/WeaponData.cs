namespace CT6GAMAI
{
    using UnityEngine;

    /// <summary>
    /// Fire Emblem: Fates is my main inspiration for this AI project, so all
    /// mechanics have similar/the same documentation as the in-game descriptions.
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/Weapons/WeaponData", order = 1)]
    public class WeaponData : ScriptableObject
    {
        /// <summary>
        /// Unit's equipped weapon.
        /// </summary>
        public Constants.WeaponType WeaponType;

        /// <summary>
        /// Weapon's name. Displayed in-game.
        /// </summary>
        public string WeaponName = "Unassigned";

        /// <summary>
        /// Weapon's sprite. Displayed in-game.
        /// </summary>
        public Sprite WeaponSprite = null;

        /// <summary>
        /// Weapon's minimum range.
        /// </summary>
        public int WeaponMinRange;

        /// <summary>
        /// Weapon's max range.
        /// </summary>
        public int WeaponMaxRange;

        /// <summary>
        /// Weapon's might.
        /// </summary>
        public int WeaponMight;
    }
}
