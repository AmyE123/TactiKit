namespace CT6GAMAI
{
    using UnityEngine;

    /// <summary>
    /// Represents the weighting for AI playstyles in the game.
    /// </summary>
    [CreateAssetMenu(fileName = "PlaystyleWeighting", menuName = "ScriptableObjects/Units/PlaystyleWeighting", order = 2)]
    public class AIPlaystyleWeighting : ScriptableObject
    {
        /// <summary>
        /// The AI's playstyle.
        /// </summary>
        public Constants.Playstyle Playstyle;

        /// <summary>
        /// The AI's concern regarding their health. 
        /// 0 = Less Concerned / 1 = Most Concerned.
        /// </summary>
        public float HealthWeight;

        /// <summary>
        /// The AI's concern regarding their distance to a goal/target. 
        /// A higher weight means the AI will prioritize actions that minimize their distance to the target.
        /// 0 = Less Concerned / 1 = Most Concerned.
        /// </summary>
        public float DistanceWeight;

        /// <summary>
        /// The AI's concern regarding how close they get to other player units.
        /// 0 = More Defensive / 1 = More Offensive.
        /// </summary>
        public float PlayerDistanceWeight;

        /// <summary>
        /// The AI's concern regarding the difference in power between themselves and another player unit.
        /// 0 = Less Concerned / 1 = Most Concerned.
        /// </summary>
        public float PowerDifferenceWeight;

        /// <summary>
        /// The AI's concern regarding how much they care about running away from a battle if things get tough.
        /// 0 = Not Concerned / 1 = Concerned.
        /// </summary>
        public float EscapeWeight;
    }
}