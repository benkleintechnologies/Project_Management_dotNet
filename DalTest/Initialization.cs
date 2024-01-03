namespace DalTest;
using DO;
using DalApi;
using System.Xml.Linq;
using System;

public static class Initialization
{
    private static IEngineer? s_dalEngineer; //stage 1
    private static IDependency? s_dalDependency; //stage 1
    private static ITask? s_dalTask; //stage 1

    private static readonly Random s_rand = new();

    private static void createEngineers()
    {
        String[] engineerNames = {
            "Moishe Goldstein", "Shloimy Rosenberg", "Yitzy Schwartz", "Shmuly Cohen", "Dovid Greenbaum",
            "Shloime Rosenblatt", "Leiby Levine", "Yudi Abramowitz", "Mordy Eisenberg", "Yakov Fried",
            "Avrumy Stern", "Ahrele Goldman", "Tzvi Berger", "Yossi Adler", "Meir Silber", "Simchy Perlman",
            "Aryeh Feldman", "Zevy Klein", "Moshe Blum", "Yisroel Hochberg"
        };

        foreach (var engineerName in engineerNames)
        {
            int id; 
            do
                id = s_rand.Next(200000000, 400000000); 
            while (s_dalEngineer!.Read(id) != null);

            //randomaly choose experience level
            var values = Enum.GetValues(typeof(EngineerExperience));
            int randomIndex = s_rand.Next(values.Length);
            EngineerExperience engineerExperience = (EngineerExperience)values.GetValue(randomIndex);
            
            Engineer newEng = new(id, engineerName, null, s_rand.Next(80000, 200000), engineerExperience);
            s_dalEngineer!.Create(newEng);
        }

    }

    private static void createTasks()
    {

    }

    private static void createDependencies()
    {

    }
}
