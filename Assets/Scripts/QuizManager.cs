using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    public Image questionImage;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public Color correctAnswerColor = Color.green;
    public Color wrongAnswerColor = Color.red;
    public float feedbackDelay = 1.8f;
    public GameObject endScreenPanel;
    public GameObject gamePanel;
    public TextMeshProUGUI resultsText;

    public string jsonFileName = "questions.json";
    private List<QuestionData> questionList;
    private int currentQuestionIndex;
    private int correctAnswerIndex;
    private int correctAnswersCount;
    private int wrongAnswersCount;
    private int questionsAskedCount;

    private const int TotalQuestions = 31;

    void Start()
    {
        LoadQuestionsFromJson();
        LoadRandomQuestion();
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
        if (questionsAskedCount >= TotalQuestions || questionList.Count == 0)
        {
            ShowEndScreen();
            return;
        }

        if (questionList == null || questionList.Count == 0)
        {
            Debug.LogError("No questions available!");
            return;
        }

        //random question
        currentQuestionIndex = Random.Range(0, questionList.Count);
        QuestionData questionData = questionList[currentQuestionIndex];
        correctAnswerIndex = questionData.correctAnswerIndex;

        questionText.text = questionData.question;

        // Load the image
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
                answerButtons[i].interactable = true;
            }
        }
        else
        {
            Debug.LogError("Not enough answer buttons for the number of answers.");
        }

        // Remove the question from the list to prevent it from being asked again
        questionList.RemoveAt(currentQuestionIndex);

        // Increment the question count correctly
        questionsAskedCount++;
        Debug.Log($"Questions asked: {questionsAskedCount}/{TotalQuestions}");
    }

    void OnAnswerSelected(int index)
    {
        // Disable all answer buttons to prevent multiple clicks
        SetButtonsInteractable(false);

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

        // Start the coroutine to load the next question after a delay
        StartCoroutine(NextQuestionAfterDelay());
    }

    IEnumerator NextQuestionAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDelay);
        LoadRandomQuestion();
    }

    void SetButtonsInteractable(bool interactable)
    {
        foreach (var button in answerButtons)
        {
            button.interactable = interactable;
        }
    }

    void ShowEndScreen()
    {
        // Hide the game panel
        gamePanel.SetActive(false);
        // Show the end screen panel
        endScreenPanel.SetActive(true);

        // Display the results
        resultsText.text = $"Σωστές απαντήσεις: {correctAnswersCount}\nΛάθος απαντήσεις: {wrongAnswersCount}";
    }

    public void RestartGame()
    {

        // Reset counts
        correctAnswersCount = 0;
        wrongAnswersCount = 0;
        questionsAskedCount = 0;

        endScreenPanel.SetActive(false);
        endScreenPanel.SetActive(false); 
        // Show the game panel
        gamePanel.SetActive(true);
        LoadRandomQuestion();
    }
}
