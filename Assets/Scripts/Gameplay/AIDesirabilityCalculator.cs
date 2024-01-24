namespace CT6GAMAI
{
    using UnityEngine;

    /// <summary>
    /// A class for calculating desirability for AI actions.
    /// </summary>
    public class AIDesirabilityCalculator : MonoBehaviour
    {
        /// <summary>
        /// A normalizing calculator function.
        /// </summary>
        /// <returns>A normalized value</returns>
        private static float Normalize(float value, float minValue, float maxValue)
        {
            if (minValue == maxValue)
            {
                return 0;
            }

            return (value - minValue) / (maxValue - minValue);
        }

        /// <summary>
        /// Calculates the desirability for finding a fort based on the units current health and distance to the nearest fort.
        /// </summary>
        /// <returns>A desirability from 0 to 100. 100 is a very high desirability and 0 is a very low desirability.</returns>
        public static int CalculateFortDesirability(UnitAIManager unit)
        {
            float currentHealth = unit.UnitCurrentHealth;
            float distanceToFort = unit.GetDistanceToNearestFort();

            float maxHealth = unit.UnitStatsManager.UnitBaseData.HealthPointsBaseValue;
            float maxMovement = unit.UnitStatsManager.UnitBaseData.MovementBaseValue;

            // Normalizing the health between a 0 to 1 scale.
            // Reversing the health value with -1 as a lower health value should give us a higher desirability score.
            float normalizeHealth = 1 - Normalize(currentHealth, 0, maxHealth);

            // Normalizing the distance between a 0 to 1 scale.
            // 0 would be closer to the fort, and 1 would be at the maximum movement range from the fort.
            float normalizeDistance = Normalize(distanceToFort, 0, maxMovement);

            // These weighting values affect the overall desirability calculations.
            float healthWeighting = unit.Playstyle.HealthWeight;
            float distanceWeighting = unit.Playstyle.DistanceWeight;

            // Calculates the contribution of the units health to the overall desirability.
            // Lower health increases the desirability of finding a fort.
            // The weight (`healthWeighting`) adjusts how strongly health influences this desirability.
            float healthFactor = healthWeighting * normalizeHealth;

            // Calculates the contribution of the units distance to the overall desirability.
            // The `normalizeDistance` is inverted, so that being closer to the fort returns a higher score.
            // The weight (`distanceWeighting`) adjusts how strongly distance influences this desirability.
            float distanceFactor = distanceWeighting * (1 - normalizeDistance);

            // Adds the health and distance to get the total desirability score before scaling.
            float rawDesirability = healthFactor + distanceFactor;

            // Ensures that the desirability is within the range 0 to 100.           
            float scaledDesirability = rawDesirability * 100;
            int finalDesirability = Mathf.Clamp((int)scaledDesirability, 0, 100);

            return finalDesirability;
        }

        /// <summary>
        /// Calculates the desirability for retreating based on the unit's current health, distance to safe spots, and distance to players.
        /// </summary>
        /// <returns>A desirability from 0 to 100. 100 is a very high desirability and 0 is a very low desirability.</returns>
        public static int CalculateRetreatDesirability(UnitAIManager unit)
        {
            float currentHealth = unit.UnitCurrentHealth;
            float distanceToSafeLocation = unit.GetDistanceToBestSafeSpot();
            float distanceToPlayer = unit.GetDistanceToNearestPlayer();

            float maxHealth = unit.UnitStatsManager.UnitBaseData.HealthPointsBaseValue;
            float maxMovement = unit.UnitStatsManager.UnitBaseData.MovementBaseValue;

            // Normalizing the health between a 0 to 1 scale.
            // Reversing the health value with -1 as a lower health value should give us a higher desirability score.
            float normalizeHealth = 1 - Normalize(currentHealth, 0, maxHealth);

            // Normalizing the distance to a safe location. Closer safe locations increase the desirability to retreat.
            // 0 would be closer to safe locations, and 1 would be at the maximum movement range from safe locations.
            float normalizeSafeDistance = Normalize(distanceToSafeLocation, 0, maxMovement);

            // Normalizing the distance to the nearest player. Closer players increase the desirability to retreat.
            // 0 would be closer to nearer player, and 1 would be at the maximum movement range from the player.
            float normalizePlayerDistance = Normalize(distanceToPlayer, 0, maxMovement);

            // These weighting values affect the overall desirability calculations.
            float healthWeighting = unit.Playstyle.HealthWeight;
            float distanceWeighting = unit.Playstyle.DistanceWeight;
            float escapeWeighting = unit.Playstyle.EscapeWeight;

            // Calculates the contribution of the units health to the overall desirability.
            // Lower health increases the desirability of retreating.
            // The weight (`healthWeighting`) adjusts how strongly health influences this desirability.
            float healthFactor = healthWeighting * normalizeHealth;

            // Calculates the contribution of the units distance from a safe location to the overall desirability.
            // The `normalizeSafeDistance` is inverted, so that being closer to a safe location returns a higher score.
            // The weight (`distanceWeighting`) adjusts how strongly distance influences this desirability.
            float safeDistanceFactor = distanceWeighting * (1 - normalizeSafeDistance);

            // Calculates the contribution of the units distance from a player to the overall desirability.
            // The weight (`escapeWeighting`) adjusts how strongly escaping a player influences this desirability.
            float playerDistanceFactor = escapeWeighting * normalizePlayerDistance;

            // Adds the health and distance to get the total desirability score before scaling.
            float rawDesirability = healthFactor + safeDistanceFactor + playerDistanceFactor;

            // Ensures that the desirability is within the range 0 to 100.  
            float scaledDesirability = rawDesirability * 100;
            int finalDesirability = Mathf.Clamp((int)scaledDesirability, 0, 100);

            return finalDesirability;
        }

        /// <summary>
        /// Calculates the desirability for attacking based on the unit's distance to the nearest player, power advantage over the player, and current health.
        /// </summary>
        /// <returns>A desirability from 0 to 100. 100 is a very high desirability and 0 is a very low desirability.</returns>
        public static int CalculateAttackDesirability(UnitAIManager unit)
        {
            float distanceToPlayer = unit.GetDistanceToNearestPlayer();
            float powerAdvantage = unit.GetNearestPlayer() != null ? unit.CalculatePowerAdvantage(unit.GetNearestPlayer()) : 0;
            float currentHealth = unit.UnitCurrentHealth;

            float maxMovement = unit.UnitStatsManager.UnitBaseData.MovementBaseValue;
            float maxHealth = unit.UnitStatsManager.UnitBaseData.HealthPointsBaseValue;

            // Normalizing the health between a 0 to 1 scale.
            // Reversing the health value with -1 as a lower health value should give us a higher desirability score.
            float normalizeHealth = 1 - Normalize(currentHealth, 0, maxHealth);

            // Normalizing the distance between a 0 to 1 scale.
            // 0 would be closer to the player, and 1 would be at the maximum movement range from the player.
            float normalizeDistance = Normalize(distanceToPlayer, 0, maxMovement);

            //float normalizeDistance = distanceToPlayer / maxMovement;
            //float normalizeHealth = (maxHealth - currentHealth) / maxHealth;

            // These weighting values affect the overall desirability calculations.
            float distanceWeighting = unit.Playstyle.PlayerDistanceWeight;
            float powerWeighting = unit.Playstyle.PowerDifferenceWeight; // Add this to your Playstyle
            float healthWeighting = unit.Playstyle.HealthWeight;

            // Calculates the contribution of the units distance to the overall desirability.
            // The `normalizeDistance` is inverted, so that being closer to a player returns a higher score.
            // The weight (`distanceWeighting`) adjusts how strongly distance influences this desirability.
            float distanceFactor = distanceWeighting * (1 - normalizeDistance);

            // Calculates the contribution of the units power to the overall desirability.
            // The weight (`powerWeighting`) adjusts how strongly power influences this desirability.
            float powerFactor = powerWeighting * powerAdvantage;

            // Calculates the contribution of the units health to the overall desirability.
            // Lower health decreases the desirability of attacking.
            // The weight (`healthWeighting`) adjusts how strongly health influences this desirability.
            float healthFactor = healthWeighting * normalizeHealth;

            // Adds the health and distance to get the total desirability score before scaling.
            float rawDesirability = distanceFactor + powerFactor + healthFactor;

            // Ensures that the desirability is within the range 0 to 100.  
            float scaledDesirability = rawDesirability * 100;
            int finalDesirability = Mathf.Clamp((int)scaledDesirability, 0, 100);

            return finalDesirability;
        }

        /// <summary>
        /// Calculates the desirability for waiting
        /// </summary>
        /// <returns>A desirability of 0 or 100. 100 is a very high desirability and 0 is a very low desirability.</returns>
        public static int CalculateWaitDesirability(UnitAIManager unit)
        {
            bool hasTakenAnyDamage = unit.UnitCurrentHealth < unit.UnitStatsManager.UnitBaseData.HealthPointsBaseValue;

            if (hasTakenAnyDamage && CalculateFortDesirability(unit) > 0)
            {
                return 0;
            }
            else
            {
                return unit.ArePlayersVisible() ? 0 : 100;
            }
        }
    }
}