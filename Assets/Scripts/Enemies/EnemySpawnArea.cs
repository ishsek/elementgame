using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnArea : MonoBehaviour
{
    [System.Serializable]
    public struct WeightedEnemy
    {
        [SerializeField] public Enemy EnemyType;
        [SerializeField] public float EnemyWeight;
    }

    [SerializeField] private float m_TargetDifficulty;
    [SerializeField] private Transform[] m_SpawnLocations;
    [SerializeField] private WeightedEnemy[] m_PossibleEnemies;

    private List<Enemy> mEnemyWeightedList;
    [SerializeField] private List<Enemy> mEnemySpawnedTracker;
    private float mRemainingDifficulty = 0;

    // Start is called before the first frame update
    void Start()
    {
        mEnemyWeightedList = new List<Enemy>();
        mEnemySpawnedTracker = new List<Enemy>();
        SpawnEnemies();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && CheckEnemiesNeedSpawning())
        {
            SpawnEnemies();
        }
    }

    private bool CheckEnemiesNeedSpawning()
    {
        bool enemiesNeedSpawning = false;

        for (int i = 0; i < mEnemySpawnedTracker.Count; i++)
        {
            if (mEnemySpawnedTracker[i].GetState() == Enemy.State.Dead)
            {
                mEnemySpawnedTracker.RemoveAt(i);
                i--;
            }
        }

        if (mEnemySpawnedTracker.Count <= 0)
        {
            enemiesNeedSpawning = true;
        }

        return enemiesNeedSpawning;
    }

    private void BuildEnemyWeightList()
    {
        ClearEnemyWeightList();

        foreach (WeightedEnemy enemy in m_PossibleEnemies)
        {
            for (int i = 0; i < enemy.EnemyWeight; i++)
            {
                mEnemyWeightedList.Add(enemy.EnemyType);
            }
        }
    }

    private void ClearEnemyWeightList()
    {
        mEnemyWeightedList.Clear();
    }


    private void SpawnEnemies()
    {
        if (m_PossibleEnemies.Length <= 0)
        {
            Debug.Log("No enemies to spawn in EnemySpawnArea on " + gameObject.name);
            return;
        }

        if (m_SpawnLocations.Length <= 0)
        {
            Debug.Log("No spawn locations in EnemySpawnArea on " + gameObject.name);
        }

        BuildEnemyWeightList();
        mRemainingDifficulty = m_TargetDifficulty;
        int spawnLocationIndex = 0;

        Enemy enemyToSpawn = null;
        Enemy enemyReturned = SelectAnEnemyToSpawn();

        while (enemyReturned != null)
        {
            enemyToSpawn = enemyReturned;
            Transform spawnLocation = m_SpawnLocations[spawnLocationIndex];
            Enemy spawnedEnemy = Instantiate(enemyToSpawn, spawnLocation.transform.position, spawnLocation.transform.rotation);
            mEnemySpawnedTracker.Add(spawnedEnemy);
            enemyReturned = SelectAnEnemyToSpawn();
            
            spawnLocationIndex++;
            if (spawnLocationIndex >= m_SpawnLocations.Length)
            {
                spawnLocationIndex = 0;
            }
        }

        //ClearEnemyWeightList();
    }

    private Enemy SelectAnEnemyToSpawn()
    {
        Enemy enemyToSpawn = null;
        int randomEntry = 0;

        BuildEnemyWeightList();

        while ((enemyToSpawn == null) && (mEnemyWeightedList.Count > 0))
        {
            randomEntry = Random.Range(0, mEnemyWeightedList.Count - 1);
            enemyToSpawn = mEnemyWeightedList[randomEntry];

            if (enemyToSpawn.GetInitialDifficultyLevel() <= mRemainingDifficulty)
            {
                mRemainingDifficulty -= enemyToSpawn.GetInitialDifficultyLevel();
            }
            else
            {
                mEnemyWeightedList.RemoveAt(randomEntry);
                enemyToSpawn = null;
            }
        }

        return enemyToSpawn;
    }
}
