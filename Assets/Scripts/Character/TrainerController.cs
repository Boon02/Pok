using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
<<<<<<< HEAD
    [SerializeField] private string name;
    [SerializeField] private Sprite sprite;
=======
>>>>>>> origin/master
    [SerializeField] private Dialog dialog;
    [SerializeField] private GameObject exclamation;
    [SerializeField] private GameObject fov;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
        
    }

    private void Start()
    {
        exclamation.SetActive(false);
        SetFovRotation(character.Animator.DefaultDirection);
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
<<<<<<< HEAD
            GameController.Instance.StartTrainertBattle(this);
=======
            Debug.Log("Starting Trainer Battle.");
>>>>>>> origin/master
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
    
<<<<<<< HEAD
    public Sprite Sprite
    {
        get => sprite;
    }
    
    public string Name
    {
        get => name;
    }
=======
>>>>>>> origin/master
}
