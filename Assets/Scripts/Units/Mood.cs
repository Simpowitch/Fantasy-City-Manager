using System.Collections.Generic;

[System.Serializable]
public class Mood
{
    public const int MINVALUE = -100;
    public const int MAXVALUE = 100;

    List<MoodBuff> MoodBuffs { get; set; } = new List<MoodBuff>();


    public int MoodValue
    {
        get
        {
            int sum = 0;
            foreach (var moodBuff in MoodBuffs)
            {
                sum += moodBuff.value;
            }
            return sum;
        }
    }

    public string MoodExplanation
    {
        get
        {
            string explanation = "";
            foreach (var moodBuff in MoodBuffs)
            {
                if (explanation != "")
                    explanation += "\n";
                explanation += moodBuff.explanation;
            }
            return explanation;
        }
    }

    public string MoodBuffExplanationValues
    {
        get
        {
            string explanation = "";
            foreach (var moodBuff in MoodBuffs)
            {
                if (explanation != "")
                    explanation += "\n";
                explanation += moodBuff.value > 0 ? $"+{moodBuff.value}" : $"{moodBuff.value}";
            }
            return explanation;
        }
    }

    public void SetMoodBuffs(List<MoodBuff> newMoodBuffs) => MoodBuffs = newMoodBuffs;
}
[System.Serializable]
public class MoodBuff
{
    public string explanation;
    public int value;

    public MoodBuff(string explanation, int value)
    {
        this.explanation = explanation;
        this.value = value;
    }
}
