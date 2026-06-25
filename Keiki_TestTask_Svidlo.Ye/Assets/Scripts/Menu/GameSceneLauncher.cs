using Zenject;
using Data;
using UnityEngine;

public class GameSceneLauncher
{
    private readonly ZenjectSceneLoader _sceneLoader;

    public GameSceneLauncher(ZenjectSceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }

    public void LaunchGame(CategoryConfig category, LevelConfig level)
    {
        _sceneLoader.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single, container =>
        {
            container.Bind<CategoryConfig>().FromInstance(category).AsSingle();
            container.Bind<LevelConfig>().FromInstance(level).AsSingle();
        });
    }
}