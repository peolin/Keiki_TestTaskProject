using UnityEngine;
using Zenject;
using Data;

public class CategoryManager : MonoBehaviour
{
    private CategoryConfig _categoryConfig;
    private LevelConfig _currentLevelConfig;
    private LevelManager _levelManager;
    private int _currentLevelIndex = 0;

    [Inject]
    public void Construct(CategoryConfig category, LevelConfig level, LevelManager levelManager)
    {
        _categoryConfig = category;
        _currentLevelConfig = level;
        _levelManager = levelManager;
        
        _levelManager.OnLevelCompleted += MoveToNextLevel;
    }

    private void Start()
    {
        DetermineInitialIndex();
        
        if (_currentLevelConfig != null)
        {
            _levelManager.InitializeLevel(_currentLevelConfig);
        }
    }

    private void DetermineInitialIndex()
    {
        for (int i = 0; i < _categoryConfig.Levels.Length; i++)
        {
            if (_categoryConfig.Levels[i] == _currentLevelConfig)
            {
                _currentLevelIndex = i;
                break;
            }
        }
    }

    private void MoveToNextLevel()
    {
        _currentLevelIndex++;

        if (_currentLevelIndex < _categoryConfig.Levels.Length)
        {
            _currentLevelConfig = _categoryConfig.Levels[_currentLevelIndex];
        }
        else
        {
            RestartCategory();
            return;
        }

        _levelManager.InitializeLevel(_currentLevelConfig);
    }

    private void RestartCategory()
    {
        _currentLevelIndex = 0;
        _currentLevelConfig = _categoryConfig.Levels[_currentLevelIndex];
        _levelManager.InitializeLevel(_currentLevelConfig);
    }

    private void OnDestroy()
    {
        if (_levelManager != null)
        {
            _levelManager.OnLevelCompleted -= MoveToNextLevel;
        }
    }
}
