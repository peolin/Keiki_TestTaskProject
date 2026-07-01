using UnityEngine;
using Zenject;
using Data;

public class MainMenuManager : MonoBehaviour
{
    [Header("Data Configurations")]
    [SerializeField] private CategoryConfig[] _categories;

    [Header("Dynamic UI Prefab")] 
    [SerializeField] private CategoryController _categoryRowPrefab;
    [SerializeField] private Transform _categoriesContainer;

    private GameSceneLauncher _sceneLauncher;
    private DiContainer _diContainer;

    [Inject]
    public void Construct(GameSceneLauncher sceneLauncher, DiContainer diContainer)
    {
        _sceneLauncher = sceneLauncher;
        _diContainer = diContainer;
    }

    private void Start()
    {
        //ClearContainer();
        
        InitializeMenu();
    }

    /*private void ClearContainer()
    {
        foreach (Transform child in _categoriesContainer)
        {
            Destroy(child.GameObject);
        }
    }*/

    private void InitializeMenu()
    {
        if (_categories == null || _categoryRowPrefab == null || _categoriesContainer == null) return;


        foreach (var category in _categories)
        {
            if (category is null) continue;
            
            CategoryController rowInstance = _diContainer.InstantiatePrefabForComponent<CategoryController>(
                _categoryRowPrefab, 
                _categoriesContainer
            );
            
            rowInstance.InitializeRow(category, OnLevelSelected, _diContainer);
        }
    }

    private void OnLevelSelected(CategoryConfig category, LevelConfig level)
    {
        if (category == null || level == null) return;
        _sceneLauncher.LaunchGame(category, level);
    }
}