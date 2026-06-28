using UnityEngine;
using Zenject;
using Data;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private CategoryManager _categoryManager;
    [SerializeField] private TracingMechanic _tracingMechanic;
    [SerializeField] private HintController _hintController;
    [SerializeField] private AudioManager _audioManager;

    public override void InstallBindings()
    {
        Container.BindInstance(_levelManager).AsSingle();
        Container.BindInstance(_categoryManager).AsSingle();
        Container.BindInstance(_tracingMechanic).AsSingle();
        Container.BindInstance(_hintController).AsSingle();
        Container.BindInstance(_audioManager).AsSingle();
        
        Container.Bind<CategoryType>()
            .FromMethod(ctx => ctx.Container.Resolve<CategoryConfig>().Category).AsSingle();
    }
}