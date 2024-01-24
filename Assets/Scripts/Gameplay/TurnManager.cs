namespace CT6GAMAI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manages the turn based mechanics, controlling the transition between different phases.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        [SerializeField] private Phases _activePhase;
        [SerializeField] private UI_PhaseManager _uiPhaseManager;
        [SerializeField] private TurnMusicManager _turnMusicManager;
        [SerializeField] private int _turnsTaken;

        private GameManager _gameManager;
        private bool _isPhaseStarted = false;

        /// <summary>
        /// An event that can be used to update other parts of the game when the phase changes
        /// </summary>
        public event Action<Phases> OnPhaseChanged;

        /// <summary>
        /// The active phase
        /// </summary>
        public Phases ActivePhase => _activePhase;

        /// <summary>
        /// The music manager for turns
        /// </summary>
        public TurnMusicManager TurnMusicManager => _turnMusicManager;

        /// <summary>
        /// The amount of turns which have currently been taken. Used for endgame screen.
        /// </summary>
        public int TurnsTaken => _turnsTaken;

        /// <summary>
        /// A boolean for checking whether the game has finished.
        /// </summary>
        public bool HasGameEnded;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            SetPhase(Phases.PlayerPhase);
        }

        private void Update()
        {
            if (!HasGameEnded)
            {
                if (!_isPhaseStarted)
                {
                    StartPhase();
                }
                else if (CheckAllTurnsHaveBeenTaken(ActivePhase))
                {
                    SwitchPhase();
                    _isPhaseStarted = false;
                }

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    if (ActivePhase == Phases.PlayerPhase)
                    {
                        SwitchPhase();
                        _isPhaseStarted = false;
                    }
                }

                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    SwitchPhase();
                    _isPhaseStarted = false;
                }
            }
        }

        private IEnumerator TransitionToNextPhase(Phases nextPhase, float delay)
        {
            UpdatePlayerInput(nextPhase);

            yield return new WaitForSeconds(delay);

            SetPhase(nextPhase);
        }

        private void StartPhase()
        {
            if (!_isPhaseStarted)
            {
                if (ActivePhase == Phases.PlayerPhase)
                {
                    foreach (var unit in _gameManager.UnitsManager.ActivePlayerUnits)
                    {
                        unit.ResetTurn();
                    }
                }
                else
                {
                    foreach (var unit in _gameManager.UnitsManager.ActiveEnemyUnits)
                    {
                        unit.ResetTurn();
                    }
                }

                _isPhaseStarted = true;
            }
        }

        private bool CheckAllTurnsHaveBeenTaken(Phases turn)
        {
            List<UnitManager> units = turn == Phases.PlayerPhase ? _gameManager.UnitsManager.ActivePlayerUnits : _gameManager.UnitsManager.ActiveEnemyUnits;

            foreach (UnitManager unit in units)
            {
                if (!unit.HasActedThisTurn)
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdatePlayerInput(Phases nextPhase)
        {
            if (nextPhase == Phases.EnemyPhase)
            {
                _gameManager.GridManager.GridCursor.SetPlayerInput(false);
            }
        }

        private void StartPlayerPhase()
        {
            _turnsTaken += 1;

            // Enable player input
            _gameManager.GridManager.GridCursor.SetPlayerInput(true);
            _turnMusicManager.PlayPlayerPhaseMusic();

            RefreshMap();
            StartPlayerTurn();
        }

        private void StartEnemyPhase()
        {
            _gameManager.AIManager.UpdateAIUnits();

            _turnMusicManager.PlayEnemyPhaseMusic();

            RefreshMap();
            _gameManager.AIManager.StartEnemyAI();
        }

        /// <summary>
        /// Refreshes the map when a new phase starts to update all stood nodes.
        /// </summary>
        public void RefreshMap()
        {
            foreach (NodeManager node in _gameManager.GridManager.AllNodes)
            {
                node.UpdateStoodUnit();
            }
        }

        /// <summary>
        /// Begins the player turn and applies terrain effects.
        /// </summary>
        public void StartPlayerTurn()
        {
            foreach (UnitManager unit in _gameManager.UnitsManager.ActivePlayerUnits)
            {
                unit.ApplyTerrainEffects();
            }

            if (_gameManager.UnitsManager.ActivePlayerUnits.Count > 0)
            {
                var firstPlayer = _gameManager.UnitsManager.ActivePlayerUnits[0];
                _gameManager.GridManager.GridCursor.MoveCursorTo(firstPlayer.StoodNode.Node);
            }
        }

        /// <summary>
        /// Sets the phase to a new phase.
        /// </summary>
        /// <param name="newPhase">The new phase you want to transition to.</param>
        public void SetPhase(Phases newPhase)
        {
            _activePhase = newPhase;
            OnPhaseChanged?.Invoke(_activePhase);

            switch (_activePhase)
            {
                case Phases.PlayerPhase:
                    StartPlayerPhase();
                    break;
                case Phases.EnemyPhase:
                    StartEnemyPhase();
                    break;
            }

            _isPhaseStarted = false;
        }

        /// <summary>
        /// Switches the game to the next phase.
        /// </summary>
        public void SwitchPhase()
        {
            Phases nextPhase = ActivePhase == Phases.PlayerPhase ? Phases.EnemyPhase : Phases.PlayerPhase;

            _uiPhaseManager.DisplayPhaseUI(nextPhase);
            StartCoroutine(TransitionToNextPhase(nextPhase, PHASE_SWITCH_DELAY));
        }
    }
}