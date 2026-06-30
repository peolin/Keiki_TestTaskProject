using UnityEngine;
using Zenject;
using Data;

public class GameInstaller : MonoInstaller
{
    [Header("Game Scene Managers")]
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private CategoryManager _categoryManager;
    [SerializeField] private TracingMechanic _tracingMechanic;
    [SerializeField] private HintController _hintController;
    [SerializeField] private AudioManager _audioManager;

    [Header("Game Scene Default Data")] 
    [SerializeField] private CategoryConfig _defaultCategoryConfig;
    
    public override void InstallBindings()
    {
        Container.BindInstance(_levelManager).AsSingle();
        Container.BindInstance(_categoryManager).AsSingle();
        Container.BindInstance(_tracingMechanic).AsSingle();
        Container.BindInstance(_hintController).AsSingle();
        Container.BindInstance(_audioManager).AsSingle();

        if (Container.HasBinding<CategoryConfig>()) // default fallback
        {
            Container.Bind<CategoryType>()
                .FromMethod(ctx => ctx.Container.Resolve<CategoryConfig>().Category).AsSingle();
        }
        else
        {
            Container.BindInstance(_defaultCategoryConfig).AsSingle();
            Container.Bind<CategoryType>().FromInstance(_defaultCategoryConfig.Category).AsSingle();
        }

        if (!Container.HasBinding<LevelConfig>())
        {
            Container.BindInstance(_defaultCategoryConfig.Levels[0]).AsSingle();
        }
    }
}