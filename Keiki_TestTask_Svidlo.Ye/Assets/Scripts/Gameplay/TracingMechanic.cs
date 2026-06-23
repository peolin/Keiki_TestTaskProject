using UnityEngine;

public class TracingMechanic : MonoBehaviour
{
    [SerializeField] private GameObject _tracerObject; 
    private Transform _tracerTransform;
    
    private Camera _mainCamera;

    private void Start()
    {
        _tracerTransform = _tracerObject.transform;
        _mainCamera = Camera.main;
        
        _tracerObject.SetActive(false);
    }
    
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            TraceTouch();
        }
    }
    
    private void TraceTouch()
    {
        Touch touch = Input.GetTouch(0);
        
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(touch.position);
        
        // IMPORTANT: ScreenToWorldPoint returns a Z value equal to the camera's distance.
        // We force it to 0 (or whatever depth your tracing plane is at) to keep it in 2D space.
        worldPos.z = 0f; 

        if (touch.phase == TouchPhase.Began)
        {
            _tracerTransform.position = worldPos;
            _tracerObject.SetActive(true);
        }
        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            _tracerTransform.position = worldPos;
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            // _tracerObject.SetActive(false); // setting line invisible on touch stop
        }
    }
}