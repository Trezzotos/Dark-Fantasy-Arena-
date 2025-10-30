using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

// Si, sono più classi in un file, ma non sono MonoBehaviour quindi fa niente

// ===========================================
// 1. CLASSI DI DATI SERIALIZZABILI
// ===========================================

[System.Serializable]
public class InventoryData
{
    [NonSerialized] public Dictionary<SpellData, int> spells;

    // Queste liste sono usate solo per il json
    public List<SpellData> spellKeys;
    public List<int> spellValues;

    public List<PerkData> perks;
    public int money;

    public InventoryData(Dictionary<SpellData, int> currentSpells, List<PerkData> currentPerks, int currentMoney)
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
    public int structuresDestroyed;
    public int currentDifficulty;
    public float playTimeSeconds;
    public int score;
    public String name;

    public GameStatsData(int level, int killed, int destroyed, int difficulty, float playTime, int score, String name)
    {
        currentLevel = level;
        enemiesKilled = killed;
        structuresDestroyed = destroyed;
        currentDifficulty = difficulty;
        playTimeSeconds = playTime;
        this.score = score;
        this.name = name;
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
            PreSaveConversion(data.inventory);
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
                PostLoadConversion(data.inventory);
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

    public static void PreSaveConversion(InventoryData data)
    {
        // Converte il Dictionary di gioco in due List serializzabili.
        data.spellKeys = new List<SpellData>(data.spells.Keys);
        data.spellValues = new List<int>(data.spells.Values);
    }

    public static void PostLoadConversion(InventoryData data)
    {
        // Se le liste serializzabili sono state caricate, ricostruisce il Dictionary di gioco.
        if (data.spellKeys != null && data.spellValues != null)
        {
            data.spells = new Dictionary<SpellData, int>();
            for (int i = 0; i < data.spellKeys.Count; i++)
            {
                data.spells.Add(data.spellKeys[i], data.spellValues[i]);
            }
        }
    }

    public static void GenerateEmptySaveFile(int difficulty, String name, SpellData[] spellDatas)
    {
        Dictionary<SpellData, int> spells = new Dictionary<SpellData, int>();

        for (int i = 0; i < spellDatas.Length; i++)
        {
            spells.Add(spellDatas[i], 0);
        }
        
        GameSaveData newGameData = new GameSaveData(
            new InventoryData(spells, new List<PerkData>(), 0),
            new GameStatsData(1, 0, 0, difficulty, 0.0f, 0, name)
        );
        SaveGame(newGameData);
    }

    public static void DeleteGame()
    {
        if (!SaveFileExists())
        {
            Debug.LogWarning("Save file does not exists");
            return;
        }

        try
        {
            File.Delete(SavePath);
            Debug.Log($"File di salvataggio eliminato con successo: {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Errore durante l'eliminazione del file: {e.Message}");
        }
    }

    public static bool SaveFileExists()
    {
        return File.Exists(SavePath);
    }
}