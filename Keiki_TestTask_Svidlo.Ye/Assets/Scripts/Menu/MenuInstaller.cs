using UnityEngine;
using Zenject;

public class MenuInstaller : MonoInstaller
{
    [SerializeField] private MainMenuManager _mainMenuManager;
    
    public override void InstallBindings()
    {
        Container.BindInstance(_mainMenuManager).AsSingle();
    }
}