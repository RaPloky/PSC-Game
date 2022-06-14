﻿using System.Collections;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public GameplayManager satelliteToDamage;
    public GameObject enemyPrefab;
    [Range(3f, 20f)]
    public float spawnDelay;
    [Range(0, 1)] 
    public float chanceToInstantiate;
    [HideInInspector]
    public bool enemyIsSpawned = false;
    [HideInInspector]
    public bool spawnFreezed = false;

    private Transform _spawnerTrans;

    private void Awake()
    {
        _spawnerTrans = gameObject.transform;
    }
    private void Start()
    {
        StartSpawn();
    }
    private bool CheckIfDischarged()
    {
        bool isDischarged = false;
        if (satelliteToDamage.isDicharged)
        {
            isDischarged = true;
        }
        return isDischarged;
    }
    public IEnumerator TryToSpawn()
    {
        yield return new WaitForSeconds(spawnDelay);

        if (CheckIfDischarged()) yield break;

        float rnd = Random.Range(0f, 1f);
        if (rnd < chanceToInstantiate && !enemyIsSpawned && !spawnFreezed)
        {
            enemyIsSpawned = true;
            Instantiate(enemyPrefab, _spawnerTrans);
        }
        // If enemy spawns, coroutine must have
        // to stop working while enemy is active:
        if (!enemyIsSpawned) 
        {
            StartCoroutine(TryToSpawn()); 
        }
    }
    public void StartSpawn()
    {
        StartCoroutine(TryToSpawn());
    }
}
