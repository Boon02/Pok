using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Create New Recovery Item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] private int hpAmount;
    [SerializeField] private bool restoreMaxHP;
    
    [Header("PP")]
    [SerializeField] private int ppAmount;
    [SerializeField] private bool restoreMaxPP;

    [Header("STATUS CONDITIONS")]
    [SerializeField] private ConditionID status;
    [SerializeField] private bool recoverAllStatus;
    
    [Header("REVIVE")]
    [SerializeField] private bool revine;   // hồi sinh
    [SerializeField] private bool maxRevine;   // max hồi sinh
}
