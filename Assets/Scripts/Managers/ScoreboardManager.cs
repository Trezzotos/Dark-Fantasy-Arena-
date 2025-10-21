using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// =========================================================
// CLASSI WRAPPER NECESSARIE PER UNITY'S JSONUTILITY
// =========================================================

// Classe per un singolo punteggio
[System.Serializable]
public class ScoreEntry
{
    public string name;
    public int score;
}

// Classe per l'intera collezione di punteggi
[System.Serializable]
public class ScoreboardData
{
    public List<ScoreEntry> highscores = new List<ScoreEntry>();
}

// =========================================================
// CLASSE PRINCIPALE DI GESTIONE
// =========================================================

public class ScoreboardManager : MonoBehaviour
{
    static readonly String FileName = "Scoreboard.json";
    static String FilePath => Path.Combine(Application.persistentDataPath, FileName);
    static readonly int MAX_HIGHSCORE_SLOTS = 5;

    public static List<ScoreEntry> GetScoreboard()
    {
        if (SaveFileExists())
        {
            try
            {
                string json = File.ReadAllText(FilePath);
                ScoreboardData dataWrapper = JsonUtility.FromJson<ScoreboardData>(json);

                // Ordina in modo che la lista restituita sia già pronta per la visualizzazione
                return dataWrapper.highscores.OrderByDescending(entry => entry.score).ToList();
            }
            catch (Exception e)
            {
                Debug.LogError("Errore durante il caricamento o parsing: " + e.Message);
                return new List<ScoreEntry>();
            }
        }
        else
        {
            CreateNewScoreboard();
            return GetScoreboard();
        }
    }

    public static void TryAddHighscore(string name, int score)
    {
        List<ScoreEntry> highscores = GetScoreboard(); 
        
        bool isScoreboardFull = highscores.Count >= MAX_HIGHSCORE_SLOTS;

        if (!isScoreboardFull || highscores.Any(entry => score > entry.score))
        {
            if (isScoreboardFull)
            {
                // Trova l'entry con il punteggio più basso ordinando e prendendo il primo
                var lowestEntry = highscores.OrderBy(entry => entry.score).First();
                highscores.Remove(lowestEntry); // Rimuovi l'elemento più basso per fare spazio
            }

            // Aggiungi il nuovo punteggio
            highscores.Add(new ScoreEntry { name = name, score = score });

            // Ri-ordina l'intera lista e la limita (per assicurare che la Top 5 sia mantenuta)
            var updatedHighscores = highscores.OrderByDescending(entry => entry.score).Take(MAX_HIGHSCORE_SLOTS).ToList();

            // Serializzazione e Salvataggio
            try
            {
                ScoreboardData dataWrapper = new ScoreboardData { highscores = updatedHighscores };
                string updatedJson = JsonUtility.ToJson(dataWrapper, true);
                File.WriteAllText(FilePath, updatedJson);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Errore durante il salvataggio: {ex.Message}");
            }
        }
    }

    private static void CreateNewScoreboard()
    {
        List<ScoreEntry> initialScores = new List<ScoreEntry>
        {
            new ScoreEntry { name = "AAA", score = 50 },
            new ScoreEntry { name = "BBB", score = 40 },
            new ScoreEntry { name = "CCC", score = 30 },
            new ScoreEntry { name = "DDD", score = 20 },
            new ScoreEntry { name = "EEE", score = 10 }
        };

        ScoreboardData dataWrapper = new ScoreboardData { highscores = initialScores };

        try
        {
            string json = JsonUtility.ToJson(dataWrapper, true);
            File.WriteAllText(FilePath, json);
            Debug.Log($"Classifica creata su: {FilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Errore durante la creazione della classifica: {e.Message}");
        }
    }
    
    public static bool SaveFileExists()
    {
        return File.Exists(FilePath);
    }
}