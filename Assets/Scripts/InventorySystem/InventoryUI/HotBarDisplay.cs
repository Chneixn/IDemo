using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;

public class HotBarDisplay : StaticInventoryDisplay
{
    private int _maxIndexSize = 9;
    private int _currentIndex = 0;

    protected override void Start()
    {
        base.Start();

        _currentIndex = 0;
        _maxIndexSize = slots.Length - 1;

        slots[_currentIndex].ToggleHighlight();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void UseItems()
    {
        //if (slots[_currentIndex].AssignedInventorySlot.ItemDate != null) slots[_currentIndex].AssignedInventorySlot.ItemDate.UseItem();
    }
}
