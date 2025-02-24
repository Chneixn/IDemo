using TMPro;
using UnityEngine;
using UnityEngine.UI;
using InventorySystem;

public class ShopSlot_UI : MonoBehaviour
{
    [SerializeField] private Image _itemSprite;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemCount;
    [SerializeField] private ShopSlot _assignedItemSlot;

    public ShopSlot AssignedItemSlot => _assignedItemSlot;

    [SerializeField] private Button _addItemToCartButton;
    [SerializeField] private Button _reomveItemFromCartButton;

    private int _tempAmount; //用于交易确认前展示商品剩余数量

    public ShopKeeperDisplay ParentDisplay { get; private set; }
    public float MarkUp { get; private set; }

    private void Awake()
    {
        _itemSprite.sprite = null;
        _itemSprite.preserveAspect = true;
        _itemSprite.color = Color.clear;
        _itemName.text = string.Empty;
        _itemCount.text = string.Empty;

        _addItemToCartButton?.onClick.AddListener(AddItemToCart);
        _reomveItemFromCartButton?.onClick.AddListener(RemoveItemFromCart);
        ParentDisplay = transform.parent.GetComponentInParent<ShopKeeperDisplay>();
    }

    /// <summary>
    /// 商品数据传入
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="markUp"></param>
    public void Init(ShopSlot slot, float markUp)
    {
        _assignedItemSlot = slot;
        MarkUp = markUp;
        _tempAmount = slot.StackSize;
        UpdataUISlot();
    }

    private void UpdataUISlot()
    {
        if (_assignedItemSlot.ItemDate != null)
        {
            _itemSprite.sprite = _assignedItemSlot.ItemDate.icon;
            _itemSprite.color = Color.white;
            _itemCount.text = "x" + _assignedItemSlot.StackSize.ToString();
            int price = ShopKeeperDisplay.GetModifiedPrice(_assignedItemSlot.ItemDate, 1, MarkUp);

            _itemName.text = $"{_assignedItemSlot.ItemDate.displayName} - {price}";
        }
        else
        {
            _itemSprite.sprite = null;
            _itemSprite.preserveAspect = true;
            _itemSprite.color = Color.clear;
            _itemName.text = string.Empty;
            _itemCount.text = string.Empty;
        }

    }

    private void AddItemToCart()
    {
        if (_tempAmount > 0)
        {
            _tempAmount--;
            ParentDisplay.AddItemToCart(this);
            _itemCount.text = _tempAmount.ToString();

            //if (_tempAmount <= 0)
            //{
            //    MyTools.ISetActive(gameObject, false);
            //}
        }
    }

    private void RemoveItemFromCart()
    {
        if (_tempAmount == _assignedItemSlot.StackSize) return;

        _tempAmount++;
        ParentDisplay.RemoveItemFromCart(this);
        _itemCount.text = _tempAmount.ToString();
    }
}
