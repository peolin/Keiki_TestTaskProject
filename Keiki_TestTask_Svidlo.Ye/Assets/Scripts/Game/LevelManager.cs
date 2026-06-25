using System;
using UnityEngine;
using Data;

public class LevelManager : MonoBehaviour
{
    [Header("Level Path")]
    [SerializeField] private SpriteRenderer _pathImageRenderer;
    [SerializeField] private SpriteRenderer _pathColorImageRenderer;
    [SerializeField] private Transform _strokeGuideContainer;

    public event Action OnLevelCompleted;
    
    private LevelConfig _currentLevelConfig;
    private int _currentStrokeIndex = 0;
    private GameObject _currentGuidePath;

    public void InitializeLevel(LevelConfig config)
    {
        _currentLevelConfig = config;
        _currentStrokeIndex = 0;
        
        PrepareLevel();
    }

    private void PrepareLevel()
    {
        if (_currentLevelConfig is null) return;
        
        _pathImageRenderer.sprite = _currentLevelConfig.PathImage;
        _pathColorImageRenderer.sprite = _currentLevelConfig.PathImage;
        _pathColorImageRenderer.color = _currentLevelConfig.LevelColor;

        foreach (Transform child in _strokeGuideContainer)
        {
            Destroy(child.gameObject);
        }
        
        _currentGuidePath = Instantiate(_currentLevelConfig.StrokeGuidesPrefab, _strokeGuideContainer);
        _currentGuidePath.transform.localPosition = Vector3.zero;
        
        UpdateVisualGuides();
    }

    private void UpdateVisualGuides()
    {
        if (_currentGuidePath is null) return;
        
        for (int i = 0; i < _currentGuidePath.transform.childCount; i++)
        {
            Transform strokeTransform = _currentGuidePath.transform.GetChild(i);
            
            var spriteRenderers = strokeTransform.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sr in spriteRenderers)
            {
                sr.enabled = (i == _currentStrokeIndex);
            }
        }
    }

    public StrokeData GetCurrentStrokeData()
    {
        return _currentLevelConfig.StrokePath[_currentStrokeIndex];
    }

    public Transform GetStrokeContainer()
    {
        if (_currentGuidePath == null) return _strokeGuideContainer;
        return _currentGuidePath.transform.GetChild(_currentStrokeIndex);
    }

    public void CompleteCurrentStroke()
    {
        _currentStrokeIndex++;
        
        if (_currentStrokeIndex < _currentLevelConfig.StrokePath.Length)
        {
            UpdateVisualGuides();
        }
        else
        {
            OnLevelCompleted?.Invoke();
        }
    }
}
