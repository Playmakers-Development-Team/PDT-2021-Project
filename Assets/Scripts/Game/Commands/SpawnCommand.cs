using System;
using System.Collections.Generic;
using Managers;
using Turn;
using Units.Enemies;
using UnityEngine;

public class SpawnCommand : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject unitManager;
    [SerializeField] private GameObject unitToSpawn;
    [SerializeField] private int amountToSpawn;
    [SerializeField] private int minDistToSpawn;
    [SerializeField] private int parameter;
    [SerializeField] private bool unitsOnBoard;
    [SerializeField] private bool amountOfTurns;
    [SerializeField] private bool amountOfRounds;
    [SerializeField] private bool increasePhase;
    [SerializeField] private bool decreasePhase;

    private EnemyManager enemyManager;
    private TurnManager turnManager;
    private void Start()
    {
        enemyManager = ManagerLocator.Get<EnemyManager>();
        turnManager = ManagerLocator.Get<TurnManager>();
    }

    // Update is called once per frame
    private void OnValidate()
    {
        if (unitsOnBoard)
        {
            unitsOnBoard = false;
            if (parameter < enemyManager.EnemyUnits.Count)
            {
                enemyManager.SpawnSwarm(unitToSpawn, minDistToSpawn, amountToSpawn);
            }
        }
        

        if (amountOfTurns)
        {
            amountOfTurns = false;
            if (turnManager.TotalTurnCount == parameter)
            {
                enemyManager.SpawnSwarm(unitToSpawn, minDistToSpawn, amountToSpawn);
            }
        }

        if (amountOfRounds)
        {
            amountOfRounds = false;
            if (turnManager.RoundCount == parameter)
            {
                enemyManager.SpawnSwarm(unitToSpawn, minDistToSpawn, amountToSpawn);
            }
        }

        if (increasePhase)
        {
            turnManager.TurnManipulationPhaseIndex = turnManager.TurnManipulationPhaseIndex+1;
        }

        if (decreasePhase)
        {
            turnManager.TurnManipulationPhaseIndex = turnManager.TurnManipulationPhaseIndex-1;
        }
    }
}
