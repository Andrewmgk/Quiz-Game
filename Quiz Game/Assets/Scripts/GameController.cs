using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject gamePanel;
    public QuizManager quizManager;

    void Start()
    {
        // Ensure the start panel is active and the game panel is inactive at the beginning
        startPanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    public void StartGame()
    {
        // Switch from the start panel to the game panel
        startPanel.SetActive(false);
        gamePanel.SetActive(true);

        // Start the quiz
        quizManager.LoadRandomQuestion();
    }
}
