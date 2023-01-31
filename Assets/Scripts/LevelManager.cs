using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager levelManager;
    public enum Phase
    {
        Starting, Playing, Result
    };

    public Phase phase;

    [SerializeField] List<Transform> healthSpawns;
    [SerializeField] GameObject healthPickupPrefab;
    [SerializeField] float minHealthSpawnTime, maxHealthSpawnTime;
    float timeUntilLastHealth;
    float nextSpawnTime;

    private void Awake()
    {
        if(levelManager != null && levelManager != this)
        {
            Destroy(this);
        }
        else
        {
            levelManager = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        nextSpawnTime = Random.Range(minHealthSpawnTime,maxHealthSpawnTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (phase == Phase.Playing)
        {
            timeUntilLastHealth += Time.deltaTime;

            if (timeUntilLastHealth >= nextSpawnTime)
            {
                var randomPositionIndex = Random.Range(0, healthSpawns.Count);
                nextSpawnTime = Random.Range(minHealthSpawnTime, maxHealthSpawnTime);
                if (healthSpawns.All(spawn => spawn.childCount == 0))
                {
                    Instantiate(healthPickupPrefab, healthSpawns[randomPositionIndex].position, Quaternion.identity, healthSpawns[randomPositionIndex]);
                }
                timeUntilLastHealth = 0;
            }
        }
    }
}
