using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls the main menu behavior — handles button clicks for
/// starting the game and quitting the application.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    // Reference to the "Start" button in the main menu
    public Button Button_Start;

    // Reference to the "Quit" button in the main menu
    public Button Button_Quit;

    private void Awake()
    {
        // 🔹 Find button objects in the scene by name and get their Button components
        Button_Start = GameObject.Find("Button_Start").GetComponent<Button>();
        Button_Quit = GameObject.Find("Button_Quit").GetComponent<Button>();
    }

    void Start()
    {
        // Set screen resolution and fullscreen mode
        Screen.SetResolution(2560, 1440, FullScreenMode.FullScreenWindow);

        // 🔹 Bind button click events
        Button_Start.onClick.AddListener(() =>
        {
            StartGame();
        });

        Button_Quit.onClick.AddListener(() =>
        {
            ExitGame();
        });
    }

    void Update()
    {
        // (Currently unused)
        // Could be used for menu animations or input shortcuts in the future
    }

    /// <summary>
    /// Loads the first gameplay scene (index 1 in Build Settings).
    /// Called when the player clicks the "Start" button.
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Quits the application when the "Quit" button is clicked.
    /// (Note: This has no effect inside the Unity Editor.)
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
