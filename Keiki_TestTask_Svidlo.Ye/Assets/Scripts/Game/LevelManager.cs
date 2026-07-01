using System;
using UnityEngine;
using Zenject;
using DG.Tweening;
using Data;

public class LevelManager : MonoBehaviour
{
    [Header("Level Path")]
    [SerializeField] private SpriteRenderer _pathImageRenderer;
    [SerializeField] private SpriteRenderer _pathColorImageRenderer;
    [SerializeField] private Transform _strokeGuideContainer;

    public event Action<LevelConfig> OnLevelInitialized; 
    public event Action OnLevelCompleted;
    public event Action OnLevelReadyForHint;
    
    private LevelConfig _currentLevelConfig;
    private int _currentStrokeIndex;
    private GameObject _currentGuidePath;
    
    private AudioManager _audioManager;
    private bool _guidesAreRevealed;

    [Inject]
    public void Construct(AudioManager audioManager)
    {
        _audioManager = audioManager;
        _audioManager.OnInstructionAudioFinished += HandleInstructionAudioFinished;
    }
    
    public void InitializeLevel(LevelConfig config)
    {
        _currentLevelConfig = config;
        _currentStrokeIndex = 0;
        _guidesAreRevealed = false;
        
        PrepareLevel();
        OnLevelInitialized?.Invoke(config);
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
    
    private void HandleInstructionAudioFinished()
    {
        if (!_guidesAreRevealed)
        {
            _guidesAreRevealed = true;
            UpdateVisualGuides();
        }
        else
        {
            OnLevelReadyForHint?.Invoke();
        }
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
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
                sr.DOFade(1f, 1f);
                sr.enabled = (i == _currentStrokeIndex);
            }
        }
    }

    public StrokeData GetCurrentStrokeData()
    {
        if (_currentLevelConfig is null) return default;
        return _currentLevelConfig.StrokePath[_currentStrokeIndex];
    }

    public Transform GetStrokeContainer()
    {
        if (_currentGuidePath is null) return _strokeGuideContainer;
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
            _audioManager.PlayCompletion();
            OnLevelCompleted?.Invoke();
        }
    }
    
    private void OnDestroy()
    {
        if (_audioManager is not null)
        {
            _audioManager.OnInstructionAudioFinished -= HandleInstructionAudioFinished;
        }
    }
}
