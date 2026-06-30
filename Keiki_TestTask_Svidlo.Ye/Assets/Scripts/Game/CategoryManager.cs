using UnityEngine;
using Zenject;
using Data;

public class CategoryManager : MonoBehaviour
{
    private CategoryConfig _categoryConfig;
    private LevelConfig _currentLevelConfig;
    private LevelManager _levelManager;
    private int _currentLevelIndex;

    [Inject]
    public void Construct(CategoryConfig category, LevelConfig level, LevelManager levelManager)
    {
        _categoryConfig = category;

        if (level is null)
        {
            _currentLevelConfig = category.Levels[0];
            _currentLevelIndex = 0;
        }
        else
        {
            _currentLevelConfig = level;
            DetermineInitialIndex();
        }
        
        _levelManager = levelManager;
        _levelManager.OnLevelCompleted += MoveToNextLevel;
    }

    private void Start()
    {
        _levelManager.InitializeLevel(_currentLevelConfig);
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
