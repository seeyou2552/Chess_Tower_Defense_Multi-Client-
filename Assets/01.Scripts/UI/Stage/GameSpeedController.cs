using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSpeedController : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button _speedBtn;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _speedText;

    [Header("Button Sprite")]
    [SerializeField] private Sprite _normalSpeedSprite;
    [SerializeField] private Sprite _fastSpeedSprite;

    private Image _speedBtnImage;

    private bool _isFast = false;

    // Time Scale 설정
    private const float NormalTimeScale = 1f;
    private const float FastTimeScale = 1.5f;

    void Start()
    {
        if (_speedBtn == null)
            _speedBtn = GetComponent<Button>();

        
        if (_speedBtn != null)
        {
            _speedBtn.onClick.AddListener(ToggleGameSpeed);
            _speedBtnImage = _speedBtn.GetComponent<Image>();
        }   

        if (_speedText != null)
        {
            if (Time.timeScale > NormalTimeScale)
            {
                _isFast = true;
                _speedText.text = "1.5x";
                _speedBtnImage.sprite = _fastSpeedSprite;
            }
            else
            {
                _isFast = false;
                _speedText.text = "1.0x";
                _speedBtnImage.sprite = _normalSpeedSprite;
            }
        }
    }


    private void ToggleGameSpeed()
    {
        _isFast = !_isFast;
        Time.timeScale = _isFast ? FastTimeScale : NormalTimeScale;

        _speedText.text = _isFast ? "1.5x" : "1.0x";
        _speedBtnImage.sprite = _isFast ? _fastSpeedSprite : _normalSpeedSprite;
    }
}
