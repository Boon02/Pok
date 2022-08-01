using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public LayerMask solidObejctsLayer;
    public LayerMask interactableLayer;
    public LayerMask grassLayer;

    public event Action OnEcountered;
    
    private Vector2 input;
    private bool isMoving;
    private CharacterAnimator anim;
    private void Awake()
    {
        anim = GetComponent<CharacterAnimator>();
    }

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            
            if (input != Vector2.zero)
            {
                anim.MoveX = input.x;
                anim.MoveY = input.y;
                
                var targetPos = transform.position;
                
                targetPos.x += input.x;
                targetPos.y += input.y;
                if(IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
            }
        }
        
        anim.IsMoving = isMoving;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }

    void Interact()
    {
        var facingDir = new Vector3(anim.MoveX, anim.MoveY);
        var interacPos = transform.position + facingDir;
        
        //Debug.DrawLine(transform.position, interacPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interacPos, 0.2f, interactableLayer);
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
        CheckForEncounter();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObejctsLayer | interactableLayer) != null)
        {
            return false;
        }

        return true;
    }

    private void CheckForEncounter()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
        {
            if (Random.Range(1, 101) <= 10)
            {
                anim.IsMoving = false;
                OnEcountered();
            }
        }
    }
}
