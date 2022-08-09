using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Text name;
    [SerializeField] private Text count;

    public Text Name => name;
    public Text Count => count;

    public void SetData(ItemSlot itemSlot)
    {
        name.text = itemSlot.Item.Name;
        count.text = $"X {itemSlot.Count}";
    }
}
