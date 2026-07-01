using System;
using UnityEngine;
using TMPro;
using Data;
using UnityEngine.UI;
using Zenject;

public class CategoryController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _categoryTitleText;
    
    [Header("Level Setup")]
    [SerializeField] private LevelButtonController _levelButtonPrefab;
    [SerializeField] private Transform _levelsContainer;
    
    private CategoryConfig _categoryConfig;
    private Action<CategoryConfig, LevelConfig> _onLevelSelectedCallback;

    public void InitializeRow(CategoryConfig config, Action<CategoryConfig, LevelConfig> onLevelSelected, DiContainer diContainer)
    {
        _categoryConfig = config;
        _onLevelSelectedCallback = onLevelSelected;
        
        if (_categoryTitleText != null)
        {
            _categoryTitleText.text = "Trace " + _categoryConfig.Category.ToString();
        }

        if (_categoryConfig.Levels is null || _levelButtonPrefab is null) return;

        foreach (var levelConfig in _categoryConfig.Levels)
        {
            if (levelConfig is null) continue;

            LevelButtonController buttonInstance = diContainer.InstantiatePrefabForComponent<LevelButtonController>(
                _levelButtonPrefab,
                _levelsContainer
            );
            buttonInstance.InitializeButton(levelConfig, OnButtonClicked);
        }

        /*if (_levelsContainer is RectTransform rectTransform)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

            if (transform is RectTransform parentRectTransform)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(parentRectTransform);
            }
        }*/
    }

    private void OnButtonClicked(LevelConfig selectedLevel)
    {
        //_onLevelSelectedCallback?.Invoke(_categoryConfig, selectedLevel);
    }
}