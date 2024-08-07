using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject endScreen;
    public QuizManager quizManager;

    void Start()
    {
        // Ensure the start panel is active and the game panel is inactive at the beginning
        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        endScreen.SetActive(false);
    }

    public void StartGame()
    {
        // Switch from the start panel to the game panel
        startPanel.SetActive(false);
        gamePanel.SetActive(true);

        // Start the quiz
        quizManager.LoadRandomQuestion();
    }

    public void QuitGame()
    {
        // Quit the application
        Debug.Log("Quit Game");
        Application.Quit();

        // If running in the Unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
