using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    [System.Serializable]
    public abstract class InventoryHolder : MonoBehaviour
    {
        [SerializeField] private int storageSize;
        [SerializeField] protected InventoryStorage primaryStorage;

        public InventoryStorage Storage => primaryStorage;

        //Inv System to Display, amount to offest display by
        public static UnityAction<InventoryStorage, int> OnDisplayRequested;

        protected virtual void Awake()
        {
            primaryStorage = new InventoryStorage(storageSize);
        }
    }
}