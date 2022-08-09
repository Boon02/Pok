using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;
    [SerializeField] private Image itemIcon;
    [FormerlySerializedAs("description")] [SerializeField] private Text itemDescription;

    
    private Inventory inventory;
    private int selectedItem;
    private List<ItemSlotUI> slotUIList;
    private void Awake()
    {
        inventory = Inventory.GetInventory();
    }

    private void Start()
    {
        UpdateItemList();
    }

    void UpdateItemList()
    {
        //Clear all the existing items
        foreach (Transform item in itemList.transform)
        {
            Destroy(item.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.Slots)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);
            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        int prev = selectedItem;
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++selectedItem;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Slots.Count - 1);
        
        if(prev != selectedItem)
            UpdateItemSelection();
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
        
    }

    void UpdateItemSelection()
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (selectedItem == i)
            {
                slotUIList[i].Name.color = GlobalSetting.i.HighLightedColor;
                slotUIList[i].Count.color = GlobalSetting.i.HighLightedColor;
            }
            else
            {
                slotUIList[i].Name.color = Color.black;
                slotUIList[i].Count.color = Color.black;
            }
        }

        var slot = inventory.Slots[selectedItem].Item;
        itemIcon.sprite = slot.Icon;
        itemDescription.text = slot.Description;
    }
}
