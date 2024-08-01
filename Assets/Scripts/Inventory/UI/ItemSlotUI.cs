using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Text name;
    [SerializeField] private Text count;

    private RectTransform rectTransform;
    

    public Text Name => name;
    public Text Count => count;
    public float Height => rectTransform.rect.height;

    public void SetData(ItemSlot itemSlot)
    {
        rectTransform = GetComponent<RectTransform>();
        name.text = itemSlot.Item.Name;
        count.text = $"X {itemSlot.Count}";
    }
}
