using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu1 : MonoBehaviour
{
    [SerializeField]
    private Dropdown difficulty;
    public void PlayGame()
    {
        // Replace "GameScene" with the name of your game scene
        
        SceneManager.LoadScene("Peyton Scene");
    }
    public void PlayGameNorm()
    {
        // Replace "GameScene" with the name of your game scene

        SceneManager.LoadScene("Difficulty 2");
    }
    public void PlayGameHard()
    {
        // Replace "GameScene" with the name of your game scene

        SceneManager.LoadScene("Difficulty 3");
    }
    // This function is called when the Settings button is clicked
    public void OpenSettings()
    {
        // Load the Settings menu scene or show the settings UI panel
        Debug.Log("Settings button clicked. Implement your settings menu here.");
    }

    // This function is called when the Quit button is clicked
    public void QuitGame()
    {
        Debug.Log("Quit button clicked. Exiting the game.");
#if UNITY_EDITOR
        // If running in the Unity Editor, stop playing
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running in a built application, quit the game
        Application.Quit();
#endif
    }
}
