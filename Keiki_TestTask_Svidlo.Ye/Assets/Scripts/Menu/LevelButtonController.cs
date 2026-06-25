using System;
using UnityEngine;
using UnityEngine.UI;
using Data;

[RequireComponent(typeof(Button))]
public class LevelButtonController : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    
    private Button _button;
    private LevelConfig _myLevelConfig;
    private Action<LevelConfig> _onClickCallback;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
    }

    public void InitializeButton(LevelConfig config, Action<LevelConfig> onClickCallback)
    {
        _myLevelConfig = config;
        _onClickCallback = onClickCallback;
        
        if (_iconImage != null && _myLevelConfig.PathImage != null)
        {
            _iconImage.sprite = _myLevelConfig.PathImage;
            _iconImage.color = _myLevelConfig.LevelColor;
        }
    }

    private void HandleClick()
    {
        if (_myLevelConfig != null)
        {
            _onClickCallback?.Invoke(_myLevelConfig);
        }
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(HandleClick);
    }
}