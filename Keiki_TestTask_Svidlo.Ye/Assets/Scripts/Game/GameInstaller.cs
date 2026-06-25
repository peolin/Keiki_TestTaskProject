using UnityEngine;
using Zenject;
using Data;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private CategoryManager _categoryManager;
    [SerializeField] private HintController _hintController;
    [SerializeField] private AudioManager _audioManager;

    public override void InstallBindings()
    {
        Container.BindInstance(_levelManager).AsSingle();
        Container.BindInstance(_categoryManager).AsSingle();
        Container.BindInstance(_hintController).AsSingle();
        Container.BindInstance(_audioManager).AsSingle();

        CategoryConfig transitConfig = Container.Resolve<CategoryConfig>();
        
        Container.Bind<CategoryType>().FromInstance(transitConfig.Category).AsSingle();
    }

    private void Start()
    {
        _hintController.OnInstructionRequested += _audioManager.PlayInstruction;
        
        _levelManager.OnLevelCompleted += _audioManager.PlayCompletion;
    }

    private void OnDestroy()
    {
        if (_hintController != null) _hintController.OnInstructionRequested -= _audioManager.PlayInstruction;
        if (_levelManager!= null) _levelManager.OnLevelCompleted -= _audioManager.PlayCompletion;
    }
}