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

    public string jsonFileName = "questions.json";
    private List<QuestionData> questionList;

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
        int randomIndex = Random.Range(0, questionList.Count);
        QuestionData questionData = questionList[randomIndex];

        // Update the UI with the selected question data
        questionText.text = questionData.question;
        questionImage.sprite = Resources.Load<Sprite>(questionData.imageName);

        // Ensure there are enough answer buttons
        if (questionData.answers.Length <= answerButtons.Length)
        {
            for (int i = 0; i < questionData.answers.Length; i++)
            {
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = questionData.answers[i];
            }
        }
        else
        {
            Debug.LogError("Not enough answer buttons for the number of answers.");
        }
    }
}
