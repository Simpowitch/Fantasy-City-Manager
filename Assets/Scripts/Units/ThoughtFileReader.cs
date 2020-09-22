using System.Collections.Generic;
using System.IO;

public static class ThoughtFileReader 
{
    const string resourcesPath = "Assets/Resources/Textfiles/Thoughts/";
    const string tavernVisitPath = "TavernVisit.txt";

    public static string GetTavernVisit()
    {
        string path = resourcesPath + tavernVisitPath;
        string[] lines = File.ReadAllLines(path);
        return Utility.ReturnRandom(lines);
    }
}
