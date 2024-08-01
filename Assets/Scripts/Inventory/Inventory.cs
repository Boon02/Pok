using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory{Items, Pokeball, TMs}

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemSlot> slots;
    [SerializeField] private List<ItemSlot> pokeballSlots;
    [SerializeField] private List<ItemSlot> tmSlots;

    private List<List<ItemSlot>> allSlots; 

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { slots, pokeballSlots, tmSlots};
    }

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "ITEMS", "POKEBALLS", "TMs & HMs"
    };

    public List<ItemSlot> GetSlotsByCategory(int index)
    {
        return allSlots[index];
    }

    public ItemBase GetItem(int itemIndex, int categoryIndex)
    {
        var currentCategory = GetSlotsByCategory(categoryIndex);
        return currentCategory[itemIndex].Item;
    }

    public event Action OnUpdated;

    public ItemBase UseItem(int itemIndex, Pokemon selectedPokemon, int selectedCategory)
    {
        var item = GetItem(itemIndex, selectedCategory);
        bool itemUsed = item.Use(selectedPokemon);
        
        if(itemUsed)
        {
            if(!item.IsReusable)
                RemoveItem(item, selectedCategory);
            return item;
        }

        return null;
    }

    public void RemoveItem(ItemBase item, int selectedCategory)
    {
        var currentSlots = GetSlotsByCategory(selectedCategory);
        
        
        var itemSlot = currentSlots.First(slot => slot.Item == item);
        itemSlot.Count--;
        if (itemSlot.Count <= 0)
        {
            currentSlots.Remove(itemSlot);
        }
        
        OnUpdated?.Invoke();
    }

    public void AddItem(ItemBase item, int count = 1)
    {
        int category = (int)GetCategoryByItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
        if (itemSlot != null)
        {
            itemSlot.Count += count;
        }
        else
        {
            currentSlots.Add(new ItemSlot()
            {
                Item = item,
                Count = count
            });
        }
        
        OnUpdated?.Invoke();
    }

    public ItemCategory GetCategoryByItem(ItemBase item)
    {
        if (item is RecoveryItem)
            return ItemCategory.Items;
        else if (item is PokeballItem)
            return ItemCategory.Pokeball;
        else
            return ItemCategory.TMs;
    }
    
    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }
    
}

[System.Serializable]
public class ItemSlot
{
    [SerializeField] private ItemBase item;
    [SerializeField] private int count;

    public ItemBase Item {
        get => item;
        set => item = value;
    }
    
    public int Count {
        get => count;
        set => count = value;
    }
    
}
