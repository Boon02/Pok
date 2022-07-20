using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleHud playerHud;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHud enemyHud;

    private void Start()
    {
        SetUpBattle();
    }
    

    public void SetUpBattle()
    {
        playerUnit.SetUp();
        enemyUnit.SetUp();
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);
    }
}
