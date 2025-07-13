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
        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        endScreen.SetActive(false);
    }

    public void StartGame()
    {
        startPanel.SetActive(false);
        gamePanel.SetActive(true);

        // Start the quiz
        quizManager.LoadRandomQuestion();
    }

    public void QuitGame()
    {
        Application.Quit();

        // For the Unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
