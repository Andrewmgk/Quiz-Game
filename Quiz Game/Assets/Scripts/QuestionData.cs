using System.Collections.Generic;

[System.Serializable]
public class QuestionData
{
    public string question;
    public string imageName;
    public string[] answers;
    public int correctAnswerIndex;
}

[System.Serializable]
public class QuestionList
{
    public List<QuestionData> questions;
}
