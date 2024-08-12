using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

[System.Serializable]

public class QuizManager : MonoBehaviour
{
    public Image questionImage;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public Color correctAnswerColor = Color.green;
    public Color wrongAnswerColor = Color.red;
    public float feedbackDelay = 1.5f;
    public GameObject endScreen;
    public GameObject gamePanel;
    public TextMeshProUGUI resultsText;

    public string jsonFileName = "questions.json";
    private List<QuestionData> questionList;
    private int currentQuestionIndex;
    private int correctAnswerIndex;
    private int correctAnswersCount;
    private int wrongAnswersCount;
    private int questionsAskedCount;

    private const int TotalQuestions = 20;

    void Start()
    {
        LoadQuestionsFromJson();
    }

    void LoadQuestionsFromJson()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            QuestionList loadedData = JsonUtility.FromJson<QuestionList>(jsonData);
            questionList = new List<QuestionData>(loadedData.questions);
        }
        else
        {
            Debug.LogError("Cannot find file!");
        }
    }

    public void LoadRandomQuestion()
    {
        if (questionsAskedCount >= TotalQuestions)
        {

            ShowEndScreen();
            return;
        }

        if (questionList == null || questionList.Count == 0)
        {
            Debug.LogError("No questions available!");
            return;
        }

        // Get a random question
        currentQuestionIndex = Random.Range(0, questionList.Count);
        QuestionData questionData = questionList[currentQuestionIndex];
        correctAnswerIndex = questionData.correctAnswerIndex;

        // Update the UI with the selected question data
        questionText.text = questionData.question;

        // Load the image from Resources
        Debug.Log("Loading image: " + questionData.imageName);
        Sprite questionSprite = Resources.Load<Sprite>(questionData.imageName);
        if (questionSprite != null)
        {
            Debug.Log("Image loaded successfully: " + questionData.imageName);
            questionImage.sprite = questionSprite;
        }
        else
        {
            Debug.LogError("Image not found: " + questionData.imageName);
        }

        // Ensure there are enough answer buttons
        if (questionData.answers.Length <= answerButtons.Length)
        {
            for (int i = 0; i < questionData.answers.Length; i++)
            {
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = questionData.answers[i];
                answerButtons[i].onClick.RemoveAllListeners();
                int index = i; // Capture index for the lambda
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
                answerButtons[i].GetComponent<Image>().color = Color.white; // Reset button color
            }
        }
        else
        {
            Debug.LogError("Not enough answer buttons for the number of answers.");
        }

        
    }

    void OnAnswerSelected(int index)
    {
        if (index == correctAnswerIndex)
        {
            answerButtons[index].GetComponent<Image>().color = correctAnswerColor;
            Debug.Log("Correct!");
            correctAnswersCount++;
        }
        else
        {
            answerButtons[index].GetComponent<Image>().color = wrongAnswerColor;
            answerButtons[correctAnswerIndex].GetComponent<Image>().color = correctAnswerColor;
            Debug.Log("Wrong!");
            wrongAnswersCount++;
        }

        questionsAskedCount++;
        StartCoroutine(NextQuestionAfterDelay());
    }

    IEnumerator NextQuestionAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDelay);
        LoadRandomQuestion();
    }

    void ShowEndScreen()
    {
        // Hide the game panel
        gamePanel.SetActive(false);
        // Show the end screen panel
        endScreen.SetActive(true);

        // Display the results
        resultsText.text = $"Correct Answers: {correctAnswersCount}\nWrong Answers: {wrongAnswersCount}";
    }

    public void RestartGame()
    {
        // Reset counts
        correctAnswersCount = 0;
        wrongAnswersCount = 0;
        questionsAskedCount = 0;

        // Hide the end screen panel
        endScreen.SetActive(false);

        // Show the game panel and load the first question
        gamePanel.SetActive(true);
        LoadRandomQuestion();
    }
}