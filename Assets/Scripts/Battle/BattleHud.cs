using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private HPBar hpBar;
    [SerializeField] private GameObject expBar;

    [SerializeField] private Text statusText;

    [SerializeField] private Color psnColor;
    [SerializeField] private Color brnColor;
    [SerializeField] private Color slpColor;
    [SerializeField] private Color parColor;
    [SerializeField] private Color frzColor;
    [SerializeField] private Color confusionColor;

    private Pokemon _pokemon;
    private Dictionary<ConditionID, Color> statusColors;
    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl: " + pokemon.Level;
        hpBar.SetHP((float)pokemon.HP/pokemon.MaxHp);
        SetExp();
        
        statusColors = new Dictionary<ConditionID, Color>()
        {
            { ConditionID.brn,brnColor },
            { ConditionID.slp,slpColor },
            { ConditionID.par,parColor },
            { ConditionID.frz,frzColor },
            { ConditionID.psn,psnColor },
            { ConditionID.confusion, confusionColor}
            
        };
        
        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;
    }

    public void SetStatusText()
    {
        if (_pokemon.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _pokemon.Status.ID.ToString().ToUpper();
            statusText.color = statusColors[_pokemon.Status.ID];
        }
    }

    public void SetExp()
    {
        if (expBar == null) return;
        float normalizedExp = GetNormalizedExp();

        expBar.transform.localScale = new Vector3(normalizedExp, 1f, 1f);
    }
    
    public IEnumerator SetExpSmooth()
    {
        if (expBar == null) yield break;
        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
        
    }

    float GetNormalizedExp()
    {
        int currLevelExp = _pokemon.Base.GetExpForlevel(_pokemon.Level);
        int nextLevelExp = _pokemon.Base.GetExpForlevel(_pokemon.Level + 1);
        
        float normalizedExp = (float)((_pokemon.EXP - currLevelExp)) / (nextLevelExp - currLevelExp);

        return Mathf.Clamp01(normalizedExp);
    }

    public IEnumerator UpdateHp()
    {
        if (_pokemon.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float)_pokemon.HP/_pokemon.MaxHp);
            _pokemon.HpChanged = false;
        }
    }

}
