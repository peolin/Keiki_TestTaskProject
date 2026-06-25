using UnityEngine;
using Zenject;
using Data;

public class MainMenuManager : MonoBehaviour
{
    [Header("Data Configurations")]
    [SerializeField] private CategoryConfig[] _categories;

    [Header("UI Rows")]
    [SerializeField] private CategoryRowController[] _categoryRows;

    private GameSceneLauncher _sceneLauncher;

    [Inject]
    public void Construct(GameSceneLauncher sceneLauncher)
    {
        _sceneLauncher = sceneLauncher;
    }

    private void Start()
    {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        int rowsToInit = Mathf.Min(_categories.Length, _categoryRows.Length);
        
        for (int i = 0; i < rowsToInit; i++)
        {
            if (_categoryRows[i] != null && _categories[i] != null)
            {
                _categoryRows[i].InitializeRow(_categories[i], OnLevelSelected);
            }
        }
    }

    private void OnLevelSelected(CategoryConfig category, LevelConfig level)
    {
        if (category == null || level == null) return;
        _sceneLauncher.LaunchGame(category, level);
    }
}