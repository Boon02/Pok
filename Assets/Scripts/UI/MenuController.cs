using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    public event Action<int> onMenuSelected;
    public event Action onBack;
    
    private List<Text> menuItems;

    private int selectedItem = 0;
    private void Awake()
    {
        menuItems = menu.GetComponentsInChildren<Text>().ToList();
        menu.SetActive(false);
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
        UpdateItemSelection();
    }
    
    public void CloseMenu()
    {
        menu.SetActive(false);
    }

    public void HandleUpdate()
    {
        int prev = selectedItem;
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++selectedItem;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);
        
        if(prev != selectedItem)
            UpdateItemSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onMenuSelected?.Invoke(selectedItem);
            CloseMenu();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
            CloseMenu();
        }
    }

    void UpdateItemSelection()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            if (selectedItem == i)
            {
                menuItems[i].color = GlobalSetting.i.HighLightedColor;
            }
            else
            {
                menuItems[i].color = Color.black;
            }
        }
    }
}
