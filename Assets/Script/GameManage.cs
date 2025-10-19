using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

/// <summary>
/// Central game manager controlling global gameplay states,
/// such as pause, victory, failure, and main menu transitions.
/// Also updates UI elements (health bar, coin text) and manages scene flow.
/// </summary>
public class GameManage : MonoBehaviour
{
    //Singleton instance (accessible from other scripts)
    public static GameManage Instance;

    // UI panels for pause, victory, and failure screens
    public GameObject PausePanel;
    public GameObject VictoryPanel;
    public GameObject FailurePanel;

    // UI buttons for pause/victory/failure menus
    public Button ResumeButton;
    public Button RestartButton;
    public Button MainMenuButton;
    public Button QuitButton;

    // Game state flags
    public bool isWin = false;
    public bool isFail = false;

    // UI elements showing player's health and coins
    public Slider _healthslider;
    public TMP_Text _cointext;

    private void Awake()
    {
        // Set this instance as the global singleton
        Instance = this;
    }

    void Start()
    {
        // 🔹 Set resolution and initialize UI after one frame
        Screen.SetResolution(2560, 1440, FullScreenMode.FullScreenWindow);

        // Use coroutine to wait until all UI objects are active in the scene
        StartCoroutine(InitUI());
        Debug.Log("Health slider = " + _healthslider);
    }

    /// <summary>
    /// Waits one frame, finds all UI objects by name,
    /// and assigns event listeners to buttons.
    /// </summary>
    IEnumerator InitUI()
    {
        // Wait a frame to ensure UI objects have been instantiated
        yield return null;

        // Find panels and buttons by name in the scene hierarchy
        PausePanel = GameObject.Find("PausePanel");
        VictoryPanel = GameObject.Find("VictoryPanel");
        FailurePanel = GameObject.Find("FailurePanel");

        ResumeButton = GameObject.Find("ResumeButton")?.GetComponent<Button>();
        RestartButton = GameObject.Find("RestartButton")?.GetComponent<Button>();
        MainMenuButton = GameObject.Find("MainMenuButton")?.GetComponent<Button>();
        QuitButton = GameObject.Find("QuitButton")?.GetComponent<Button>();

        // Optional: find text and health bar if not linked manually in the inspector
        //_cointext = GameObject.Find("CoinText")?.GetComponent<TMP_Text>();
        //_healthslider = GameObject.Find("HealthSlider")?.GetComponent<Slider>();

        // Log warning if UI references are missing
        if (_cointext == null || _healthslider == null)
            Debug.LogWarning("⚠️ UI elements not found. Check object names in the scene.");

        // Bind button click events to their respective methods
        ResumeButton?.onClick.AddListener(Resume);
        RestartButton?.onClick.AddListener(Restart);
        QuitButton?.onClick.AddListener(Exit);
        MainMenuButton?.onClick.AddListener(GoMainMenu);

        // Initialize player-related UI
        if (Player.Instance != null)
        {
            UpdateCoin();
            UpdateHealth();
        }
    }

    /// <summary>
    /// Updates the displayed coin amount based on player's current coins.
    /// </summary>
    public void UpdateCoin()
    {
        if (_cointext != null)
            _cointext.text = Player.Instance.coin.ToString();
    }

    /// <summary>
    /// Updates the health bar to reflect the player's current HP ratio.
    /// </summary>
    public void UpdateHealth()
    {
        if (_healthslider != null)
            _healthslider.value = Player.Instance.hp / Player.Instance.maxHp;
    }

    void Update()
    {
        // Toggle pause/resume when the ESC key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0)
                Resume();
            else
                Pause();
        }
    }

    /// <summary>
    /// Loads the main menu scene (assumed to be scene index 0).
    /// </summary>
    public void GoMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Pauses the game by showing the pause panel and freezing time.
    /// </summary>
    public void Pause()
    {
        var cg = PausePanel?.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }

        Time.timeScale = 0; // Freeze all in-game movement
    }

    /// <summary>
    /// Resumes the game by hiding the pause panel and restoring time.
    /// </summary>
    public void Resume()
    {
        var cg = PausePanel?.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0;
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }

        Time.timeScale = 1; // Resume normal time flow
    }

    /// <summary>
    /// Displays the victory screen and returns to the main menu after a short delay.
    /// </summary>
    public void Win()
    {
        var cg = VictoryPanel?.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }

        isWin = true;
        StartCoroutine(WaitGoMainMenu());
    }

    /// <summary>
    /// Displays the failure screen and returns to the main menu after a short delay.
    /// </summary>
    public void GameOver()
    {
        var cg = FailurePanel?.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }

        isFail = true;
        StartCoroutine(WaitGoMainMenu());
    }

    /// <summary>
    /// Reloads the current scene to restart the game.
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Exits the application (only works in a built version, not in editor).
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Coroutine that waits for several seconds before automatically
    /// returning to the main menu after victory or failure.
    /// </summary>
    IEnumerator WaitGoMainMenu()
    {
        yield return new WaitForSeconds(4f);
        GoMainMenu();
    }
}
