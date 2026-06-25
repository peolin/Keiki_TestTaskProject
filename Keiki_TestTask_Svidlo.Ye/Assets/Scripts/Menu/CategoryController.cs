using System;
using UnityEngine;
using TMPro;
using Data;

public class CategoryRowController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _categoryTitleText;
    [SerializeField] private LevelButtonController[] _levelButtons;

    private CategoryConfig _myCategoryConfig;
    private Action<CategoryConfig, LevelConfig> _onLevelSelectedCallback;

    public void InitializeRow(CategoryConfig config, Action<CategoryConfig, LevelConfig> onLevelSelected)
    {
        _myCategoryConfig = config;
        _onLevelSelectedCallback = onLevelSelected;
        
        if (_categoryTitleText != null)
        {
            _categoryTitleText.text = _myCategoryConfig.Category.ToString();
        }
        
        for (int i = 0; i < _levelButtons.Length; i++)
        {
            if (i < _myCategoryConfig.Levels.Length)
            {
                _levelButtons[i].gameObject.SetActive(true);
                _levelButtons[i].InitializeButton(_myCategoryConfig.Levels[i], OnButtonClicked);
            }
            else
            {
                _levelButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnButtonClicked(LevelConfig selectedLevel)
    {
        _onLevelSelectedCallback?.Invoke(_myCategoryConfig, selectedLevel);
    }
}