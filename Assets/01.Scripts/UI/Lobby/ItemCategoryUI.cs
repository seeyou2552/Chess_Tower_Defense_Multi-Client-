using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCategoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _categoryBtnPrefab;
    
    private List<Button> _categoryButtons = new List<Button>();

    void Start()
    {
        if(_categoryBtnPrefab == null)
            _categoryBtnPrefab = Resources.Load<GameObject>("UI/CategorySelectButton");
        
        CreateCategoryButtons();
    }

    private void CreateCategoryButtons()
    {
        foreach (ShopItemType category in System.Enum.GetValues(typeof(ShopItemType)))
        {
            GameObject obj = Instantiate(_categoryBtnPrefab, transform);
            Button btn = obj.GetComponent<Button>();
            TextMeshProUGUI btnText = obj.GetComponentInChildren<TextMeshProUGUI>();

            if (btnText != null)
            {
                btnText.text = category.ToString();
            }

            btn.onClick.AddListener(() => OnCategoryButtonClicked(category));
            _categoryButtons.Add(btn);
        }
    }

#region Event Handlers

    private void OnCategoryButtonClicked(ShopItemType category)
    {
        EventBus.Publish(new CategoryChangeEvent(category));
    }

#endregion
}
