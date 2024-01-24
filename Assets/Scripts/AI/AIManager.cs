namespace CT6GAMAI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Manages the AI behavior for units in the game.
    /// </summary>
    public class AIManager : MonoBehaviour
    {
        private GameManager _gameManager;

        [SerializeField] private List<UnitAIManager> _aiUnits;
        [SerializeField] private AIPlaystyleWeighting[] _playstyles;

        /// <summary>
        /// All avaliable AI playstyles
        /// </summary>
        public AIPlaystyleWeighting[] Playstyles => _playstyles;

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        private IEnumerator HandleAIUnitTurns()
        {
            foreach (UnitAIManager ai in _aiUnits)
            {
                yield return ai.BeginEnemyAI();
            }
        }

        /// <summary>
        /// Starts all the enemy AI turns & applies relevant terrain effects.
        /// </summary>
        public void StartEnemyAI()
        {
            List<UnitManager> activeEnemyAI = _gameManager.UnitsManager.ActiveEnemyUnits;

            foreach (UnitManager unit in activeEnemyAI)
            {
                unit.ApplyTerrainEffects();
            }

            StartCoroutine(HandleAIUnitTurns());
        }

        /// <summary>
        /// Updates the list of AI units based on active enemy units.
        /// </summary>
        public void UpdateAIUnits()
        {
            _aiUnits.Clear();
            List<UnitManager> activeEnemyAI = _gameManager.UnitsManager.ActiveEnemyUnits;

            foreach (UnitManager unit in activeEnemyAI)
            {
                UnitAIManager UnitAI = unit.GetComponent<UnitAIManager>();
                if (UnitAI != null)
                {
                    _aiUnits.Add(UnitAI);
                }
                else
                {
                    Debug.LogWarning("[AI]: Unit hasn't got AI component added");
                }
            }
        }
    }
}