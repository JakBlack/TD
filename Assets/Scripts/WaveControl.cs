using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyItem
{
    public string name;
    public Sprite sprite;
    public float speed;
    public float hp;
    public short points;
    public bool airborne;
}

[System.Serializable]
public class SpawnInfo
{
    public int enemyType;
    public float timeDelay;
}

[System.Serializable]
public class Wave
{
    public List<SpawnInfo> enemiesToSpawn = new List<SpawnInfo>();
}

public class WaveControl : MonoBehaviour {

    public static WaveControl Instance { set; get;}

    public Transform goal;
    public Transform spawnPosition;
    public ObjectPool enemyPool;

    public List<EnemyItem> enemyTypes = new List<EnemyItem>(); // Filled in Editor
    public List<Wave> waves = new List<Wave>(); // Filled in Editor

    public List<Enemy> activeEnemies;

    private void Start()
    {
        Instance = this;
        activeEnemies = new List<Enemy>();
    }

    public void StartWave()
    {
        StartCoroutine(WaveCoruotine());
    }    

    IEnumerator WaveCoruotine()
    {
        foreach(SpawnInfo info in waves[0].enemiesToSpawn)
        {
            SpawnEnemy(enemyTypes[info.enemyType]);
            yield return new WaitForSeconds(info.timeDelay);
        }
    }

    private void SpawnEnemy(EnemyItem type)
    {
        GameObject newEnemy = enemyPool.GetObject(null, false);
        newEnemy.GetComponent<Enemy>().Setup(type);
        newEnemy.transform.position = spawnPosition.position;
        newEnemy.SetActive(true);
        if(type.airborne)
            newEnemy.GetComponent<Enemy>().StartMoving(goal.position);
        else
            newEnemy.GetComponent<Enemy>().StartMoving(MyGrid.Instance.startingPosition);

        activeEnemies.Add(newEnemy.GetComponent<Enemy>());
    }
}
