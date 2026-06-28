using System;
using UnityEngine;
using TMPro;
using Data;

public class CategoryController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _categoryTitleText;
    [SerializeField] private LevelButtonController[] _levelButtons;

    private CategoryConfig _categoryConfig;
    private Action<CategoryConfig, LevelConfig> _onLevelSelectedCallback;

    public void InitializeRow(CategoryConfig config, Action<CategoryConfig, LevelConfig> onLevelSelected)
    {
        _categoryConfig = config;
        _onLevelSelectedCallback = onLevelSelected;
        
        if (_categoryTitleText != null)
        {
            _categoryTitleText.text = "Trace " + _categoryConfig.Category.ToString();
        }
        
        for (int i = 0; i < _levelButtons.Length; i++)
        {
            if (i < _categoryConfig.Levels.Length)
            {
                _levelButtons[i].gameObject.SetActive(true);
                _levelButtons[i].InitializeButton(_categoryConfig.Levels[i], OnButtonClicked);
            }
            else
            {
                _levelButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnButtonClicked(LevelConfig selectedLevel)
    {
        _onLevelSelectedCallback?.Invoke(_categoryConfig, selectedLevel);
    }
}