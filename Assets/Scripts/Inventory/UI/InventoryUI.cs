using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum InventoryUIState {ItemSelection, PartySelection, Busy}

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemDescription;

    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;
    
    [SerializeField] private PartyScreen partyScreen;
    
    private Inventory inventory;
    private List<ItemSlotUI> slotUIList;
    private RectTransform itemListRect;
    
    int selectedItem = 0;
    private InventoryUIState state;
    
    const int itemsInViewport = 10;
    
    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();
        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);
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
        if (state == InventoryUIState.ItemSelection)
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
            
            if (Input.GetKeyDown(KeyCode.Z))
            {
                OpenPartyScreen();
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                onBack?.Invoke();
            }
        }
        else if (state == InventoryUIState.PartySelection)
        {
            
            Action onSelectedPartyScreem = () =>
            {
                // Use the item on the selected pokemon 
            };
            
            Action onBackPartyScreem = () =>
            {
                ClosePartyScreen();
            };
            
            // handle party selection
            partyScreen.HandleUpdate(onSelectedPartyScreem, onBackPartyScreem);
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
        if (slotUIList.Count <= itemsInViewport) return;
        
        float scrollPos = Mathf.Clamp(selectedItem - (int)(itemsInViewport / 2),0,selectedItem) * slotUIList[0].Height;
        itemListRect.transform.localPosition = new Vector2(itemListRect.transform.localPosition.x, scrollPos);

        bool showUpArrow = selectedItem > (int)(itemsInViewport/2);
        upArrow.gameObject.SetActive(showUpArrow);
        
        bool showDownArrow = selectedItem + (int)(itemsInViewport / 2) < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);

    }

    void OpenPartyScreen()
    {
        partyScreen.Init();
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
    }
    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;
        partyScreen.gameObject.SetActive(false);
    }
}
