using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using InventorySystem;

public class DynamicInventoryDisplay : InventoryDisplay
{
    [SerializeField] private InventorySlot_UI slotPrefab;
    [SerializeField] private Transform _transformParent;

    protected override void Start()
    {
        base.Start();
    }

    public void RefreshDynamicInventory(InventoryStorage invToDisPlay, int offest)
    {
        ClearSlots();
        inventorySystem = invToDisPlay;
        if (inventorySystem != null) inventorySystem.OnSlotChanged += UpdateSlot;
        AssignSlot(invToDisPlay, offest);
    }

    public override void AssignSlot(InventoryStorage invToDisplay, int offest)
    {
        slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();

        if (invToDisplay == null) return;

        for (int i = offest; i < invToDisplay.StorgeSize; i++)
        {
            var uiSlot = Instantiate(slotPrefab, _transformParent);
            slotDictionary.Add(uiSlot, invToDisplay.InventorySlots[i]);
            uiSlot.Init(invToDisplay.InventorySlots[i]);
            uiSlot.UpdateUISlot();
        }
    }

    private void ClearSlots()
    {
        foreach (var item in _transformParent.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }

        slotDictionary?.Clear();
    }

    private void OnDisable()
    {
        if (inventorySystem != null) inventorySystem.OnSlotChanged -= UpdateSlot;
    }
}
