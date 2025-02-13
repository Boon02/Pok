using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] private int lettersPerSecond = 30;
    
    [SerializeField] private Text dialogText;
    [SerializeField] private GameObject actionSelection;
    [SerializeField] private GameObject moveSelection;
    [SerializeField] private GameObject moveDetails;
    [SerializeField] private GameObject choiceBox;

    [SerializeField] private List<Text> actionTexts;
    [SerializeField] private List<Text> moveTexts;

    [SerializeField] private Text ppText;
    [SerializeField] private Text typeText;
    [SerializeField] private Text yesText;
    [SerializeField] private Text noText;
    

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(bool enable)
    {
        dialogText.enabled = enable;
    }

    public void EnableActionSelection(bool enable)
    {
        actionSelection.SetActive(enable);
    }
    
    public void EnableMoveSelection(bool enable)
    {
        moveSelection.SetActive(enable);
        moveDetails.SetActive(enable);
    }
    
    public void EnableChoiceBox(bool enable)
    {
        choiceBox.SetActive(enable);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = GlobalSetting.i.HighLightedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selectedMove)
            {
                moveTexts[i].color = GlobalSetting.i.HighLightedColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }

        ppText.text = $"PP {move.PP}/{move.Base.Pp}";
        typeText.text = move.Base.Type.ToString();

        if (move.PP == 0)
        {
            ppText.color = Color.red;
        }
        else
        {
            ppText.color = Color.black;
        }
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                moveTexts[i].text = moves[i].Base.Name;
            }
            else
            {
                moveTexts[i].text = "-";
            }
        }
    }

    
    public void UpdateChoiceSelection(bool yesSelected)
    {
        if (yesSelected)
        {
            yesText.color = GlobalSetting.i.HighLightedColor;
            noText.color = Color.black;
        }else 
        {
            yesText.color = Color.black;
            noText.color = GlobalSetting.i.HighLightedColor;
        }
    }
}
