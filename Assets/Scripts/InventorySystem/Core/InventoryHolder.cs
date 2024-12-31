using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    [System.Serializable]
    public abstract class InventoryHolder : MonoBehaviour
    {
        [SerializeField] private int inventorySize;
        [SerializeField] protected InventorySystem primaryInventorySystem;

        public InventorySystem PrimaryInventorySystem => primaryInventorySystem;

        //Inv System to Display, amount to offest display by
        public static UnityAction<InventorySystem, int> OnDynamicInventoryDisplayRequested;

        protected virtual void Awake()
        {
            primaryInventorySystem = new InventorySystem(inventorySize);
        }
    }
}