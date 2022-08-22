using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;

    public virtual string Name => name;
    public string Description => description;
    public Sprite Icon => icon;

    public virtual bool Use(Pokemon pokemon)
    {
        return false;
    }

    public virtual bool IsReusable => false;
    public virtual bool CanUseInBattle => true;
    public virtual bool CanUsedOutsideBattle => true;

}
