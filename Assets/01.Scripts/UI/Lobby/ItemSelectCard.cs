using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelectCard : MonoBehaviour
{
    [SerializeField] private Image _soldOutImage;
    [SerializeField] private Image _itemImage;
    private ItemData _itemData;

    private Button _selectBtn;

    void Start()
    {
        if(_selectBtn == null)
            _selectBtn = GetComponent<Button>();

        _selectBtn.onClick.AddListener(OnSelectCard);
    }

    void OnEnable()
    {
        EventBus.Subscribe<BuyItemEvent>(OnBuyItem);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<BuyItemEvent>(OnBuyItem);
    }

    public void SetItemData(ItemData data)
    {
        _itemData = data;
        _itemImage.sprite = data.Icon;

        if(!GameManager.Instance.ShopData.CheckpurchasedItem(_itemData))
        {
            VisibleSoldOut(true);
        }
        else 
        {
            VisibleSoldOut(false);
        }
    }

    private void VisibleSoldOut(bool visible)
    {
        if(_soldOutImage != null)
        {
            _soldOutImage.gameObject.SetActive(visible);
        }
    }

    public void ClearData()
    {
        _itemData = null;
        _itemImage.sprite = null;
        _soldOutImage.gameObject.SetActive(false);
    }

    private void OnSelectCard()
    {
        EventBus.Publish(new UIItemSelectEvent(_itemData));
    }

#region Event Handlers

    private void OnBuyItem(BuyItemEvent buyItemEvent)
    {
        if (_itemData == null || buyItemEvent.ItemData != _itemData)
            return;

        if(!GameManager.Instance.ShopData.CheckpurchasedItem(_itemData))
        {
            VisibleSoldOut(true);
        }
        else
        {
            VisibleSoldOut(false);
        }
    }

#endregion

}
