using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private WaveData[] waves;
    private int currentRound = 0;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.STARTING:
                BeginRound(0);
                break;
            case GameState.PLAYING:
                // riprendi round in pausa, se serve
                break;
            case GameState.GAMEOVER:
                ResetRounds();
                break;
        }
    }

    private void BeginRound(int index)
    {
        currentRound = index;
        SpawnWave(waves[index]);
    }

    private void ResetRounds()
    {
        StopAllCoroutines();
        currentRound = 0;
    }

    private void SpawnWave(WaveData wave)
    {
        EntityManager.Instance.SpawnWave(wave);
    }
}
