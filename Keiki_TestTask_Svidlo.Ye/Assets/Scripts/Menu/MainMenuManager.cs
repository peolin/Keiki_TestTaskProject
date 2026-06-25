using UnityEngine;
using Zenject;
using Data;

public class MainMenuManager : MonoBehaviour
{
    [Header("Data Configurations")]
    [SerializeField] private CategoryConfig[] _categories;

    [Header("UI Rows")]
    [SerializeField] private CategoryRowController[] _categoryRows;

    private ZenjectSceneLoader _sceneLoader;

    [Inject]
    public void Construct(ZenjectSceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
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
            _categoryRows[i].InitializeRow(_categories[i], OnLevelSelected);
        }
    }

    private void OnLevelSelected(CategoryConfig category, LevelConfig level)
    {
        if (category == null || level == null)
        {
            Debug.LogError("Не вдалося завантажити рівень: Категорія або Конфіг рівня є null!");
            return;
        }
        
        Debug.Log($"Loading Gameplay for Category: {category.Category}, Level: {level.name}");
        
        _sceneLoader.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single, container =>
        {
            container.BindInstance(category).AsSingle();
            container.BindInstance(level).AsSingle();
        });
    }
}