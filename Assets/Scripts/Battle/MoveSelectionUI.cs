using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] private List<Text> moveTexts;
    [SerializeField] private Color hightlightedColor;
    private int currentMove = 0;

    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (int i = 0; i < currentMoves.Count; i++)
        {
            moveTexts[i].text = currentMoves[i].Name;
        }

        moveTexts[currentMoves.Count].text = newMove.Name;
    }
    
    void UpdateMoveSelection(int moveSelected)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if(i == moveSelected)
                moveTexts[i].color = hightlightedColor;
            else
                moveTexts[i].color = Color.black;
        }
    }
    
    public void HandleMoveSelection(Action<int> onSelected)
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --currentMove;
        }else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++currentMove;
        }

        currentMove = Mathf.Clamp(currentMove, 0, PokemonBase.MaxNumOfMove);
        UpdateMoveSelection(currentMove);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke(currentMove);
        }
    }
}
