using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour, ISavable
{
    [SerializeField] private string name;
    [SerializeField] private Sprite sprite;
    
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
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }
        
        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(Interact());
        }
    }

    IEnumerator Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interacPos = transform.position + facingDir;
        
        //Debug.DrawLine(transform.position, interacPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interacPos, 0.2f, GameLayers.i.InteractableLayer);
        if(collider != null)
        {
            yield return collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private void OnMoveOver()
    {
        // CheckForEncounter();
        // CheckIfInTrainersView();
        var collider2Ds = Physics2D.OverlapCircleAll(transform.position - new Vector3(0f, character.offSetY), 0.2f, GameLayers.i.TriggerableLayer);

        foreach (var collider2D in collider2Ds)
        {
            var triggerable = collider2D.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                Character.Animator.IsMoving = false;
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
        
    }

    public Sprite Sprite=> sprite;
    public string Name => name;
    public Character Character => character;

    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y , 0f},
            pokemons = GetComponent<PokemonParty>().Pokemons.Select(p => p.GetSaveData()).ToList()
        };
        
        return saveData;
    }

    public void RestoreState(object state)
    {
        var save = (PlayerSaveData)state;
        
        // Restor Position
        var pos = save.position;
        transform.position = new Vector3(pos[0], pos[1]);
        
        //Restor Party
        GetComponent<PokemonParty>().Pokemons 
            = save.pokemons.Select(s => new Pokemon(s)).ToList();
    }
}

[System.Serializable]
public class PlayerSaveData{
    public float[] position;
    public List<PokemonSaveData> pokemons;
}
