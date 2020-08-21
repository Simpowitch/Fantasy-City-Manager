using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NameGenerator
{
    public static string[] englishFemaleFirstNames = new string[] {
        "Olivia", "Willow","Harriet","Martha",
"Amelia","Matilda","Emma","Gracie",
"Isla", "Elsie", "Thea",  "Maryam",
"Ava","Ruby","Eleanor","Robyn",
"Emily","Scarlett","Penelope","Aurora",
"Isabella","Sofia","Holly","Iris",
"Mia","Chloe","Hannah","Sara",
"Poppy","Eva","Molly","Arabella",
"Ella","Harper","Bella","Beatrice",
"Lily","Rosie","Rose","Heidi",
"Sophia", "Emilia","Amber","Summer",
"Charlotte", "Millie","Violet","Clara",
"Grace","Layla","Georgia","Orla",
"Evie","Imogen","Lilly","Francesca",
"Jessica","Maya","Jasmine","Aisha",
"Sophie","Eliza","Darcie","Julia",
"Alice","Esme","Nancy","Darcey",
"Daisy","Lola","Annabelle","Edith",
"Florence","Elizabeth","Lottie","Victoria",
"Freya","Erin","Zara","Bonnie",
"Phoebe","Maisie","Maria","Lyla",
"Evelyn","Aria","Amelie","Darcy",
"Sienna","Luna","Abigail","Hallie",
"Isabelle","Lucy","Mila","Leah",
"Ivy","Ellie","Anna","Megan"
    };

    public static string[] englishMaleFirstNames = new string[] {
        "Oliver", "Ethan","Reggie","Luke",
"Harry","Max","Jaxon","Caleb",
"George", "Jospeh", "Harley",  "Hunter",
"Noah","Samuel","Rory","Mohammad",
"Jack","Mohammed","Luca","Elliott",
"Jacob","Finley","Jake","Ezra",
"Leo","Daniel","Albie","Louis",
"Oscar","Benjamin","Jenson","Ryan",
"Charlie","Harrison","Albert","Blake",
"Muhammad","Sebastian","Frankie","Lewis",
"William", "Adam","Tommy","Dexter",
"Alfie", "Mason","Carter","Ollie",
"Henry","Theodore","Ronnie","Nathan",
"Thomas","Teddy","Gabriel","Ellis",
"Joshua","Dylan","Bobby","Jesse",
"Freddie","Elijah","Harvey","Liam",
"James","Arlo","Matthew","Alex",
"Archie","Riley","Michael","Kai",
"Arthur","David","Elliot","Ibrahim",
"Logan","Zachary","Stanley","Tyler",
"Theo","Louie","Jayden","Finn",
"Alexander","Toby","Frederick","Austin",
"Edward","Hugo","Charles","Leon",
"Isaac","Reuben","Jackson","Ralph",
"Lucus","Jude","Roman","Felix"
    };

    public static string[] englishLastNames = new string[] {
        "Dean", "Carroll","Welch","Campbell",
"Hawkins","Parks","Griffiths","Abel",
"Jacobs", "Whiteford", "Shaw",  "Fox",
"Gardner","Doyle","Lyons","Read",
"Connolly","Charles","Chapman","Williamson",
"Gibson","Hewitt","Sanders","Cross",
"Abbott","Harrison","Montgomery","Middleton",
"Hall","Reynolds","Benson","Horton",
"Harper","Cooper","Wilkins","Fisher",
"West","Brewer","Ashton","Arnold",
"Stewart", "Higgins","Wheeler","Thomas",
"Saunders", "Scott","Aston","Morton",
"Stone","Todd","Potter","Joseph",
"Rowe","Cook","Daniel","Parry",
"Hodgson","Harvey","Bailey","Goodwin",
"Turner","Miles","Woodward","Gilbert",
"Cunningham","Roberts","Green","Allen",
"Sims","Wright","Greenwood","Davies",
"Hanson","Armstrong","Marshall","Barber",
"Hyde","Parsons","Bates","Walker",
"Bell","Ali","Wolfe","Gray",
"Sutherland","Baldwin","Coleman","Edwards",
"Tucker","Atkinson","Jennings","Palmer",
"Houghton","Alexander","Knight","Spencer",
"Garner","Weber","Hamilton","Beattie"
    };

    public static string GetName()
    {
        bool female = Utility.RandomizeBool(50);
        string name = "";
        name += female ? Utility.ReturnRandom(englishFemaleFirstNames) : Utility.ReturnRandom(englishMaleFirstNames);
        name += " " + Utility.ReturnRandom(englishLastNames);
        return name;
    }
}
