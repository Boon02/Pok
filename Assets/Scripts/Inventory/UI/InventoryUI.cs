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
    [SerializeField] private Text itemDescription;

    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;
    
    private Inventory inventory;
    private List<ItemSlotUI> slotUIList;
    private RectTransform itemListRect;
    
    int selectedItem = 0;
    
    const int itemsInViewport = 8;
    
    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
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
        
        HandleScrolling();
    }

    void HandleScrolling()
    {
        float scrollPos = Mathf.Clamp(selectedItem - (int)(itemsInViewport / 2),0,selectedItem) * slotUIList[0].Height;
        itemListRect.transform.localPosition = new Vector2(itemListRect.transform.localPosition.x, scrollPos);

        bool showUpArrow = selectedItem > (int)(itemsInViewport/2);
        upArrow.gameObject.SetActive(showUpArrow);
        
        bool showDownArrow = selectedItem + (int)(itemsInViewport / 2) < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);

    }
}
