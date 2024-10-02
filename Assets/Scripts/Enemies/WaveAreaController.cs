using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAreaController : MonoBehaviour
{
    [System.Serializable]
    public struct WaveInfo
    {
        [SerializeField] public EnemySpawnArea EnemySpawner; 
    }

    [SerializeField] private WaveInfo[] m_Waves;
    [SerializeField] private GameObject[] m_Walls;

    private bool mWavesInProgress = false;
    private bool mWavesCompleted = false;
    private int mCurrentWave = 0;

    private void StartWaves()
    {
        mWavesInProgress = true;
        mCurrentWave = 0;
        SetBlockingWalls(true);
        SpawnCurrentWave();
    }

    private void SetBlockingWalls(bool newState)
    {
        foreach (GameObject wall in m_Walls)
        {
            wall.SetActive(newState);
        }
    }

    private void SpawnCurrentWave()
    {
        m_Waves[mCurrentWave].EnemySpawner.SpawnEnemyWave();
    }

    public void WaveCompleted()
    {
        mCurrentWave++;

        if (mCurrentWave >= m_Waves.Length)
        {
            mWavesCompleted = true;
            mWavesInProgress = false;
            SetBlockingWalls(false);
            return;
        }
        else
        {
            SpawnCurrentWave();
        }
    }

    public bool CheckWavesNeedStart()
    {
        return (mWavesInProgress == false) && (mWavesCompleted == false);
    }

    public void WaveStartTriggered()
    {
        StartWaves();
    }
}
