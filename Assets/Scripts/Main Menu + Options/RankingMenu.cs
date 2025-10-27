using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RankingMenu : MonoBehaviour
{
    public TMP_Text[] nameLabels;
    public TMP_Text[] scoreLabels;

    void Start()
    {
        List<ScoreEntry> scores = ScoreboardManager.GetScoreboard();

        int i = 0;
        foreach (ScoreEntry record in scores)   // QUALCUNO HA DETTO RECORD?!?!?!?
        {
            nameLabels[i].text = record.name;
            scoreLabels[i].text = "" + record.score;
            i++;
        }
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
