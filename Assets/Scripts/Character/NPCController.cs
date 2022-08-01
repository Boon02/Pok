using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{

    [SerializeField] private Dialog dialog;
    [SerializeField] private List<Vector2> movementPattern;
    [SerializeField] private float timeBetweenPattern;
    
    
    private float idleTimer = 0f;
    private NPCState state;
    
    
    private Character character;
    private int currentPattern = 0;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Interact(Transform initator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initator.position);
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
            {
                state = NPCState.Idle;
            }));
        }
    }

    private void Update()
    {
        Debug.Log(state);
        
        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0 )
                    StartCoroutine(Walk());
            }
        }
        character.HandleUpdate();
    }

    private IEnumerator Walk()
    {
        state = NPCState.Walking;
        var oldPos = transform.position;
        
        yield return character.Move(movementPattern[currentPattern]);
        
        if(oldPos != transform.position)
            currentPattern = (currentPattern + 1) % movementPattern.Count;

        state = NPCState.Idle;
    }
}

public enum NPCState{ Idle, Walking, Dialog }
