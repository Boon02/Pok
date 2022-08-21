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

        if (_pokemon != null)
        {
            _pokemon.OnStatusChanged -= SetStatusText;
            _pokemon.OnHpChanged -= UpdateHp;
        }
        
        nameText.text = pokemon.Base.Name;
        SetLevel();
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
        _pokemon.OnHpChanged += UpdateHp;
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
    
    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (expBar == null) yield break;

        if (reset)
            expBar.transform.localScale = new Vector3(0f, 1f, 1f);
        
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
    
    public void UpdateHp()
    {
        StartCoroutine( UpdateHpAsync());
    }

    public IEnumerator UpdateHpAsync()
    {
        yield return hpBar.SetHPSmooth((float)_pokemon.HP/_pokemon.MaxHp);
    }

    public IEnumerator WaitForUpdateHp()
    { 
        yield return new WaitUntil(() => hpBar.IsUpdating == false);
    }

    public void SetLevel()
    {
        levelText.text = "Lvl: " + _pokemon.Level;
    }
    
    public void ClearData(){
        if (_pokemon != null)
        {
            _pokemon.OnStatusChanged -= SetStatusText;
            _pokemon.OnHpChanged -= UpdateHp;
        }
    }

}
