using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum InventoryUIState {ItemSelection, PartySelection, MoveToForget, Busy}

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;
    
    [SerializeField] private Text categoryText;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemDescription;

    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;
    
    [SerializeField] private PartyScreen partyScreen;
    [SerializeField] private MoveSelectionUI moveSelectionUI;
    
    private Inventory inventory;
    private List<ItemSlotUI> slotUIList;
    private RectTransform itemListRect;
    private Action<ItemBase> onItemUse;
    
    int selectedItem = 0;
    int selectedCategory = 0;
    
    private MoveBase moveToLearn;
    
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

        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
        //Clear all the existing items
        foreach (Transform item in itemList.transform)
        {
            Destroy(item.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);
            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }

    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUse = null)
    {
        this.onItemUse = onItemUse;
        
        if (state == InventoryUIState.ItemSelection)
        {
            int prevItem = selectedItem;
            int prevCategory = selectedCategory;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --selectedItem;
            else if (Input.GetKeyDown(KeyCode.RightArrow)) 
                selectedCategory++;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                selectedCategory--;

            if (selectedCategory > Inventory.ItemCategories.Count - 1)
                selectedCategory = 0;
            else if (selectedCategory < 0)
            {
                selectedCategory = Inventory.ItemCategories.Count - 1;
            }
            
            selectedItem 
                = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);

            if (prevCategory != selectedCategory)
            {
                ResetSelection();
                categoryText.text = Inventory.ItemCategories[selectedCategory];
                UpdateItemList();
            }
            else if (prevItem != selectedItem)
            {
                UpdateItemSelection();
            }
            
            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(ItemSelected());
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
                StartCoroutine(UseItem());
            };
            
            Action onBackPartyScreem = () =>
            {
                ClosePartyScreen();
            };
            
            // handle party selection
            partyScreen.HandleUpdate(onSelectedPartyScreem, onBackPartyScreem);
        }
        else if (state == InventoryUIState.MoveToForget)
        {
            Action<int> onSelected = (int moveIndex) =>
            {
                StartCoroutine(OnMoveToForgetSelected(moveIndex));
            };
            
            moveSelectionUI.HandleMoveSelection(onSelected);
        }
        
        
    }

    void UpdateItemSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);
        
        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);
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

        if (slots.Count > 0)
        {
            ItemBase slot = slots[selectedItem].Item;
            
            itemIcon.sprite = slot.Icon;
            itemDescription.text = slot.Description;
        }

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

    private IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        yield return HandleTmItems();

        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);

        if (usedItem != null)
        {
            if (usedItem is RecoveryItem)
                yield return DialogManager.Instance.ShowDialogText($"The player used {usedItem.Name}"); 
            onItemUse?.Invoke(usedItem);
        }
        else 
        {
            if (selectedCategory == (int)ItemCategory.Items)
                yield return DialogManager.Instance.ShowDialogText($"It won't have any effect!");
        }
        
        ClosePartyScreen();
    }
    
    IEnumerator HandleTmItems()
    {
        var tmItem = inventory.GetItem(selectedItem, selectedCategory) as TmItem;
        if (tmItem == null)
            yield break;

        var pokemon = partyScreen.SelectedMember;

        if (pokemon.HasMove(tmItem.Move))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} already know {tmItem.Move.Name}");
            yield break;
        }

        if (!tmItem.CanBeTaught(pokemon))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} can't learn {tmItem.Move.Name}");
            yield break;
        }

        if (pokemon.Moves.Count < PokemonBase.MaxNumOfMove)
        {
            pokemon.LearnToMove(tmItem.Move);
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} learned {tmItem.Move.Name}");
        }
        else
        {
            yield return DialogManager.Instance.TypeDialog($"{pokemon.Base.Name} trying to learn {tmItem.Move.Name}");
            yield return DialogManager.Instance.TypeDialog($"But it can't learn more than {PokemonBase.MaxNumOfMove} moves");
            yield return ChooseMoveForget(pokemon, tmItem.Move);
            yield return new WaitUntil(() => state != InventoryUIState.MoveToForget);
        }
    }
    
    IEnumerator ChooseMoveForget(Pokemon pokemon, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DialogManager.Instance.ShowDialogText("Choose the move want forget!", true, false);
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x=>x.Base).ToList(), newMove);
        moveToLearn = newMove;
        
        state = InventoryUIState.MoveToForget;
    }

    void ResetSelection()
    {
        selectedItem = 0;
        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        itemDescription.text = "";
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
        partyScreen.ClearMemberSlotsMessage();
        partyScreen.gameObject.SetActive(false);
    }

    IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;
        
        var item = inventory.GetItem(selectedItem, selectedCategory);
        
        if (GameController.Instance.State == GameState.Battle)
        {
            // In Battle
            if (!item.CanUseInBattle)
            {
                yield return DialogManager.Instance.ShowDialogText($"This item can't be used in battle");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            // Outside Battle
            if (!item.CanUsedOutsideBattle)
            {
                yield return DialogManager.Instance.ShowDialogText($"This item can't be used outside battle");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        
        if (selectedCategory == (int)ItemCategory.Pokeball)
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();
            
            if(item is TmItem)
                partyScreen.ShowIfTmIsUsable(item as TmItem);
        }
        
    }

    IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        var pokemon = partyScreen.SelectedMember;
        
        DialogManager.Instance.CloseDialog();
        moveSelectionUI.gameObject.SetActive(false);

        if (moveIndex == PokemonBase.MaxNumOfMove)
        {
            // don't learn the new move
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} didn't learn {moveToLearn.Name}.");

        }
        else
        {
            // forget the move selected
            var selectedMove = pokemon.Moves[moveIndex];
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} forgot {selectedMove.Base.Name} and learn {moveToLearn.Name}.");
            pokemon.Moves[moveIndex] = new Move(moveToLearn);

        }

        moveToLearn = null;
        state = InventoryUIState.ItemSelection;
    }
}
