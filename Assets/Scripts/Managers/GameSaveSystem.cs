using UnityEngine;
using System.IO;
using System.Collections.Generic;

// Si, sono più classi in un file, ma non sono MonoBehaviour quindi fa niente

// ===========================================
// 1. CLASSI DI DATI SERIALIZZABILI
// ===========================================

[System.Serializable]
public class InventoryData
{
    public List<SpellData> spells;
    public List<Perk> perks;
    public int money;

    public InventoryData(List<SpellData> currentSpells, List<Perk> currentPerks, int currentMoney)
    {
        spells = currentSpells;
        perks = currentPerks;
        money = currentMoney;
    }
}

[System.Serializable]
public class GameStatsData
{
    public int currentLevel;
    public int enemiesKilled;
    public string currentDifficulty;
    public float playTimeSeconds;

    public GameStatsData(int level, int killed, string difficulty, float playTime)
    {
        currentLevel = level;
        enemiesKilled = killed;
        currentDifficulty = difficulty;
        playTimeSeconds = playTime;
    }
}

// --- Contenitore di Salvataggio Principale ---
[System.Serializable]
public class GameSaveData
{
    public InventoryData inventory;
    public GameStatsData gameStats;

    public GameSaveData(InventoryData invData, GameStatsData statsData)
    {
        inventory = invData;
        gameStats = statsData;
    }
}

// ===========================================
// 2. LOGICA DI SALVATAGGIO (Sistema Statico)
// ===========================================

public static class SaveSystem
{
    private static readonly string SaveFileName = "game_save.json";
    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public static void SaveGame(GameSaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"Gioco salvato su: {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Errore durante il salvataggio: {e.Message}");
        }
    }

    public static GameSaveData LoadGame()
    {
        if (SaveFileExists())
        {
            try
            {
                string json = File.ReadAllText(SavePath);
                GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
                Debug.Log("Gioco caricato con successo.");
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Errore durante il caricamento: {e.Message}");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("Nessun file di salvataggio trovato. Ritorno dati vuoti.");
            return null;
        }
    }

    public static bool SaveFileExists()
    {
        return File.Exists(SavePath);
    }
}