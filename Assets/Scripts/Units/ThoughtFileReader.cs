using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class ThoughtFileReader
{
    const string resourcesPath = "Assets/Resources/Textfiles/Thoughts/";
    const string dividerEnd = "#END";

    public static string GetText(Unit.Personality personality, string action)
    {
        string path = GetPersonalityPath(personality);
        string[] lines = File.ReadAllLines(path);
        List<string> acceptedLines = new List<string>();
        action = NormalizeString(action);

        acceptedLines.AddRange(GetAcceptedLines(lines, action));

        return Utility.ReturnRandom(acceptedLines);
    }

    private static string GetPersonalityPath(Unit.Personality personality)
    {
        return resourcesPath + personality.ToString() + ".txt";
    }

    private static string NormalizeString(string baseString)
    {
        char[] chars = baseString.ToCharArray();
        string newString = "";

        if (chars.Length == 0)
            return "#DEFAULT";

        if (chars[0] != '#')
            newString += "#";
        for (int i = 0; i < chars.Length; i++)
        {
            newString += char.ToUpper(chars[i]);
        }
        return newString;
    }


    private static List<string> GetAcceptedLines(string[] lines, string dividerStart = "#DEFAULT")
    {
        List<string> acceptedLines = new List<string>();
        bool startFound = false;

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == dividerStart)
            {
                startFound = true;
                continue;
            }
            else if (lines[i] == dividerEnd)
                break;
            else if (startFound)
                acceptedLines.Add(lines[i]);
        }

        if (dividerStart == "#DEFAULT")
            return acceptedLines;

        if (acceptedLines.Count == 0)
            acceptedLines.AddRange(GetAcceptedLines(lines));

        if (acceptedLines.Count == 0)
        {
            acceptedLines.Add("Who are you to read my thoughts!");
            acceptedLines.Add("Get out of my head!");
            Debug.LogError("No lines found");
        }

        return acceptedLines;
    }
}
