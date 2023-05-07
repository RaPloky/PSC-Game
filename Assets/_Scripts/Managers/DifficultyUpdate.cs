using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class DifficultyUpdate : MonoBehaviour
{
    public static DifficultyUpdate Instance;

    [SerializeField, Range(10f, 40f)] private float difficultyIncreaseDelay;

    [SerializeField] private List<Spawner> ufoSpawners;
    [SerializeField] private List<Spawner> meteorSpawners;

    [SerializeField, Range(0.01f, 0.05f)] private float spawnChanceIncrease;
    [SerializeField, Range(0.01f, 0.25f)] private float spawnDelayDecrease;
    [SerializeField, Range(0, 0.25f)] private float consumptionDelayDecrease;
    [SerializeField, Range(0.2f, 0.5f)] private float consumptionDelayLimit; 

    [SerializeField] private List<Guard> activeGuards;

    [Header("For charger off:")]
    [SerializeField] private List<GainEnergyOnTouch> energyUpdaters;
    [SerializeField] private GameObject chargerDestroyedMessage;


    [Header("For destroyer off:")]
    [SerializeField] private GameObject destroyerDestroyedMessage;


    [Header("For protector off:")]
    [SerializeField] private GameObject protectorDestroyedMessage;

    public List<Guard> ActiveGuards => activeGuards;

    private WaitForSeconds DifficultyIncreaseDelay;

    private void Awake()
    {
        Instance = this;
        StartCoroutine(IncreaseDifficulty());
        DifficultyIncreaseDelay = new WaitForSeconds(difficultyIncreaseDelay);
    }

    private void OnEnable() => EventManager.OnGuardDischarged += IncreaseEnergyConsumption;
    private void OnDisable() => EventManager.OnGuardDischarged -= IncreaseEnergyConsumption;

    private IEnumerator IncreaseDifficulty()
    {
        while (true)
        {
            yield return DifficultyIncreaseDelay;

            UpdateSpawnersStats(ufoSpawners);
            UpdateSpawnersStats(meteorSpawners);
        }
    }

    private void UpdateSpawnersStats(List<Spawner> spawners)
    {
        for (int i = 0; i < spawners.Count; i++)
        {
            spawners[i].LaunchChance += spawnChanceIncrease;
            spawners[i].SpawnDelay -= spawnDelayDecrease;
        }
    }

    public void RemoveGuardFromList(ref Guard guard) => activeGuards.Remove(guard);

    private void IncreaseEnergyConsumption()
    {
        for (int i = 0; i < activeGuards.Count; i++)
        {
            activeGuards[i].ConsumptionDelay = Mathf.Clamp(activeGuards[i].ConsumptionDelay - consumptionDelayDecrease, 
                consumptionDelayLimit, activeGuards[i].StartConsumptionDelay);
        }
    }

    public void SpecificDifficultyIncrease(GuardType guardType)
    {
        if (guardType == GuardType.None)
        {
            print("Guard type haven't selected");
            return;
        }

        switch (guardType)
        {
            case GuardType.Charger:
                DecreaseEnergyAddAmount();
                break;
            case GuardType.Destroyer:
                IncreaseEnemyHp();
                break;
            case GuardType.Protector:
                IncreaseEnemyDamage();
                break;
        }
    }

    private void DecreaseEnergyAddAmount()
    {
        EnableMessage(chargerDestroyedMessage);
        int energyAddAmountDecrease;

        for (int i = 0; i < energyUpdaters.Count; i++)
        {
            energyAddAmountDecrease = energyUpdaters[i].EnergyAddAmount / 4;
            energyUpdaters[i].EnergyAddAmount -= energyAddAmountDecrease;
        }

        StartCoroutine(DisableMessage(chargerDestroyedMessage));
    }

    private void IncreaseEnemyHp()
    {
        EnableMessage(destroyerDestroyedMessage);


        StartCoroutine(DisableMessage(destroyerDestroyedMessage));
    }

    private void IncreaseEnemyDamage()
    {
        EnableMessage(protectorDestroyedMessage);


        StartCoroutine(DisableMessage(protectorDestroyedMessage));
    }

    private IEnumerator DisableMessage(GameObject destructionMessage)
    {
        yield return new WaitForSeconds(3);
        destructionMessage.SetActive(false);
    }

    private void EnableMessage(GameObject message)
    {
        if (activeGuards.Count == 1)
            return;

        message.SetActive(true);
    }

}
