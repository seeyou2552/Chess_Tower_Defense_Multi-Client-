using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject _shopPanel;

    [Header("Card")]
    [SerializeField] private GameObject _itemCardPrefab;
    [SerializeField] private Transform _cardParent;

    [Header("Info")]
    [SerializeField] private ItemInfoUI _itemInfoUI;
    

    [Header("Button")]
    [SerializeField] private Button _quitBtn;
    [SerializeField] private Button _buyBtn;

    private ShopItemType _currentCategory = ShopItemType.ChessPiece;
    private ItemData _selectedItemData;
    private List<ItemSelectCard> _itemCards = new List<ItemSelectCard>();

    void Start()
    {
        if(_itemCardPrefab == null)
            _itemCardPrefab = Resources.Load<GameObject>("UI/ItemSelectCard");

        _quitBtn.onClick.AddListener(OnBackShopUI);
        _buyBtn.onClick.AddListener(OnBuyItem);
    }

    void OnEnable()
    {
        EventBus.Subscribe<UIShopEvent>(OnShopEvent);
        EventBus.Subscribe<CategoryChangeEvent>(OnCategoryChange);
        EventBus.Subscribe<UIItemSelectEvent>(OnItemSelect);
    }

    void OnDisable()
    {
        ClearCards();

        EventBus.Unsubscribe<UIShopEvent>(OnShopEvent);
        EventBus.Unsubscribe<CategoryChangeEvent>(OnCategoryChange);
        EventBus.Unsubscribe<UIItemSelectEvent>(OnItemSelect);       
    }

    public ItemSelectCard CreateSelectCard(ItemData data)
    {
        GameObject obj = Instantiate(_itemCardPrefab, _cardParent);
        
        ItemSelectCard card = obj.GetComponent<ItemSelectCard>();

        card.SetItemData(data);
        return card;
    }

    private void ClearCards()
    {
        foreach (var card in _itemCards)
        {
            card.gameObject.SetActive(false);
            card.ClearData();
        }
        
    }

    public void VisibleUI(bool isVisible)
    {
        _shopPanel.SetActive(isVisible);

        if(isVisible)
        {
            CommonUIManager.Instance.AddUIStack(() => VisibleUI(false));
            SoundManager.Instance.PlaySFX("ShowShopUI");
        }
            
    }

    private void ChangeCategory(ShopItemType category)
    {
        for (int i = _itemCards.Count - 1; i >= 0; i--)
        {
            if (_itemCards[i] == null)
            {
                _itemCards.RemoveAt(i);
                continue;
            }

            _itemCards[i].gameObject.SetActive(false);
        }
        
        _currentCategory = category;
        int cardCount = 0;

        switch (_currentCategory)
        {
            case ShopItemType.ChessPiece:
                
                foreach (var itemEntry in GameManager.Instance.ShopData.ShopItems)
                {
                    var item = GameManager.Instance.ShopData.CachedItems.Find(i => i != null && i.ItemIndex == itemEntry.ItemIndex);
                    if (item == null)
                        continue;
                    var minionItem = item as MinionItemData;

                    if (minionItem == null)
                        continue;

                    cardCount++;
                    if (_itemCards.Count < cardCount)
                    {
                        ItemSelectCard card = CreateSelectCard(item);
                        _itemCards.Add(card);
                    }
                    else
                    {
                        _itemCards[cardCount - 1].SetItemData(item);
                    }

                    _itemCards[cardCount - 1].gameObject.SetActive(true);                        
                }

                break;
            case ShopItemType.Upgrade:
                
                foreach (var itemEntry in GameManager.Instance.ShopData.ShopItems)
                {
                    var item = GameManager.Instance.ShopData.CachedItems.Find(i => i != null && i.ItemIndex == itemEntry.ItemIndex);
                    if (item == null)
                        continue;

                    if (item.ItemType != ShopItemType.Upgrade)
                        continue;

                    cardCount++;
                    if (_itemCards.Count < cardCount)
                    {
                        ItemSelectCard card = CreateSelectCard(item);
                        _itemCards.Add(card);
                    }
                    else
                    {
                        _itemCards[cardCount - 1].SetItemData(item);
                    }

                    _itemCards[cardCount - 1].gameObject.SetActive(true);                        
                }
                break;
            case ShopItemType.Other:
                
                foreach (var itemEntry in GameManager.Instance.ShopData.ShopItems)
                {
                    var item = GameManager.Instance.ShopData.CachedItems.Find(i => i != null && i.ItemIndex == itemEntry.ItemIndex);
                    if (item == null)
                        continue;

                    if (item.ItemType != ShopItemType.Other)
                        continue;

                    cardCount++;
                    if (_itemCards.Count < cardCount)
                    {
                        ItemSelectCard card = CreateSelectCard(item);
                        _itemCards.Add(card);
                    }
                    else
                    {
                        _itemCards[cardCount - 1].SetItemData(item);
                    }

                    _itemCards[cardCount - 1].gameObject.SetActive(true);                        
                }
                break;
        }
    }

#region Button Handlers

    private void OnBackShopUI()
    {
        _selectedItemData = null;
        CommonUIManager.Instance.BackUI();
        SoundManager.Instance.PlaySFX("BackBtnClick", 1f);
    }

    private void OnBuyItem()
    {
        if(_selectedItemData == null)
        {
            EventBus.Publish(new UIAlertEvent("구매 물품을 선택해주세요."));
            return;
        }
    
    }

#endregion

#region Event Handlers

    private void OnShopEvent(UIShopEvent e)
    {
        VisibleUI(true);
        ChangeCategory(_currentCategory);
        EventBus.Publish(new UIGoldChangedEvent(PlayerManager.Instance.Data.Gold));
    }

    private void OnCategoryChange(CategoryChangeEvent e)
    {
        ChangeCategory(e.Category);
    }

    private void OnItemSelect(UIItemSelectEvent e)
    {
        _selectedItemData = e.ItemData;

        _itemInfoUI.ItemSelect(_selectedItemData);
        CommonUIManager.Instance.AddUIStack(() => _itemInfoUI.VisibleUI(false));
    }

#endregion

}

#region EventBus

/// <summary>
/// 로비에서 상점 UI 오픈 시 이벤트
/// </summary>
public readonly struct UIShopEvent {}

public readonly struct BuyItemEvent
{
    public readonly ItemData ItemData;

    public BuyItemEvent(ItemData itemData)
    {
        ItemData = itemData;
    }
}

public readonly struct UIItemSelectEvent
{
    public readonly ItemData ItemData;

    public UIItemSelectEvent(ItemData itemData)
    {
        ItemData = itemData;
    }
}

public readonly struct CategoryChangeEvent
{
    public readonly ShopItemType Category;

    public CategoryChangeEvent(ShopItemType category)
    {
        Category = category;
    }
}

#endregion