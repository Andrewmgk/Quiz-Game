using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class QuizManager : MonoBehaviour
{
    public Image questionImage;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public Color correctAnswerColor = Color.green;
    public Color wrongAnswerColor = Color.red;
    public float feedbackDelay = 1.5f;

    public string jsonFileName = "questions.json";
    private List<QuestionData> questionList;
    private int currentQuestionIndex;
    private int correctAnswerIndex;

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
        Sprite questionSprite = Resources.Load<Sprite>(questionData.imageName);
        if (questionSprite != null)
        {
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
        }
        else
        {
            answerButtons[index].GetComponent<Image>().color = wrongAnswerColor;
            answerButtons[correctAnswerIndex].GetComponent<Image>().color = correctAnswerColor;
            Debug.Log("Wrong!");
        }

        StartCoroutine(NextQuestionAfterDelay());
    }

    IEnumerator NextQuestionAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDelay);
        LoadRandomQuestion();
    }
}
