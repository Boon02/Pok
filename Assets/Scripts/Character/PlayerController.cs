using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public event Action OnEcountered;
    
    private Vector2 input;
    
    private Character character;
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            
            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, CheckForEncounter));
            }
        }
        
        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }

    void Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interacPos = transform.position + facingDir;
        
        //Debug.DrawLine(transform.position, interacPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interacPos, 0.2f, GameLayers.i.InteractableLayer);
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    private void CheckForEncounter()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.GrassLayer) != null)
        {
            if (Random.Range(1, 101) <= 10)
            {
                character.Animator.IsMoving = false;
                OnEcountered();
            }
        }
    }
}
