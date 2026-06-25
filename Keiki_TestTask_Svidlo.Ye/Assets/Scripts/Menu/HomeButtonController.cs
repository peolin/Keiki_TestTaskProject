using UnityEngine;
using Zenject;

public class HomeButtonController : MonoBehaviour
{
    private ZenjectSceneLoader _sceneLoader;

    [Inject]
    public void Construct(ZenjectSceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }
    
    public void OnButtonClick()
    {
        _sceneLoader.LoadScene("Menu", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}