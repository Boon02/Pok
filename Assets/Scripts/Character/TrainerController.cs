using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TrainerController : MonoBehaviour, Interactable
{
    [SerializeField] private string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Dialog dialog;
    [SerializeField] private Dialog dialogAfterBattle;
    [SerializeField] private GameObject exclamation;
    [SerializeField] private GameObject fov;

    private Character character;
    private bool battleLost = false;

    private void Awake()
    {
        character = GetComponent<Character>();
        
    }

    private void Start()
    {
        exclamation.SetActive(false);
        SetFovRotation(character.Animator.DefaultDirection);
    }

    private void Update()
    {
        character.HandleUpdate();
    }

    public void Interact(Transform initator)
    {
        character.LookTowards(initator.position);
        if (!battleLost)
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
            {
                GameController.Instance.StartTrainertBattle(this);
            }));
        }
        else
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterBattle));
        }
            
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        // Show Exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        // Walk towards the player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        
        moveVec = new Vector3(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));
        
        yield return character.Move(moveVec);

        // Show dialog
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
        {
            GameController.Instance.StartTrainertBattle(this);
        }));
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180;
        else if (dir == FacingDirection.Left)
            angle = 270;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
    }
    
    public Sprite Sprite
    {
        get => sprite;
    }
    
    public string Name
    {
        get => name;
    }
}
