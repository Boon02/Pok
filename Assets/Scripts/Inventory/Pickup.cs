using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, Interactable
{
    [SerializeField] private ItemBase item;

    public bool Used { get; set; } = false;
    
    public IEnumerator Interact(Transform initator)
    {
        if (!Used)
        {
            initator.GetComponent<Inventory>().AddItem(item);
            
            Used = true;
            
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = true;

            yield return DialogManager.Instance.ShowDialogText($"Player found {item.Name}");
        }
        
    }
}
 