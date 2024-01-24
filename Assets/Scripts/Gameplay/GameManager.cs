namespace CT6GAMAI
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Manages the overall game state and interactions between game components.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GlobalUnitsManager _unitsManager;
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private AudioManager _audioManager;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private CameraManager _cameraManager;
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private TurnManager _turnManager;
        [SerializeField] private AIManager _aiManager;

        #region Public Manager Getter References

        /// <summary>
        /// Public getter for GlobalUnitsManager.
        /// </summary>
        public GlobalUnitsManager UnitsManager => _unitsManager;

        /// <summary>
        /// Public getter for GridManager.
        /// </summary>
        public GridManager GridManager => _gridManager;

        /// <summary>
        /// Public getter for AudioManager
        /// </summary>
        public AudioManager AudioManager => _audioManager;

        /// <summary>
        /// Public getter for UIManager
        /// </summary>
        public UIManager UIManager => _uiManager;

        /// <summary>
        /// Public getter for CameraManager
        /// </summary>
        public CameraManager CameraManager => _cameraManager;

        /// <summary>
        /// Public getter for BattleManager
        /// </summary>
        public BattleManager BattleManager => _battleManager;

        /// <summary>
        /// Public getter for TurnManager
        /// </summary>
        public TurnManager TurnManager => _turnManager;

        /// <summary>
        /// Public getter for AI Manager
        /// </summary>
        public AIManager AIManager => _aiManager;

        #endregion // Public Manager Getter References

        /// <summary>
        /// Singleton instance of GameManager.
        /// </summary>
        public static GameManager Instance;

        /// <summary>
        /// The number of player deaths in the game. Used for the end screen.
        /// </summary>
        public int PlayerDeaths;

        void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Reloading of the scene for when the game reloads.
        /// </summary>
        public void ReloadScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}