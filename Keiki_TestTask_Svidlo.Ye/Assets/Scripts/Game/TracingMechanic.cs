using System;
using System.Collections.Generic;
using Data;
using Utilities;
using UnityEngine;
using Zenject;

public class TracingMechanic : MonoBehaviour
{
    [SerializeField] private GameObject _tracerObject;
    
    [Header("Tracing Mask Stamp Parameters")]
    [SerializeField] private GameObject _maskStampPrefab;
    [SerializeField] private float _touchThreshold = 1.2f;
    [SerializeField] private float _stampIntervalDistance = 0.2f;
    [SerializeField] private float _autoGenerationDensity = 0.1f;
    
    private Transform _tracerTransform;
    private Camera _mainCamera;
    private LevelManager _levelManager;
    
    private List<Vector2> _calculatedPathPoints = new List <Vector2>();
    private int _targetPointIndex = 1;
    private bool _isTracingActive = false;
    private Vector3 _lastStampPosition;

    public event Action OnPlayerActivity;

    [Inject]
    public void Construct(LevelManager levelManager)
    {
        _levelManager = levelManager;
        _levelManager.OnLevelInitialized += OnLevelInitialized;
    }

    private void Start()
    {
        _tracerTransform = _tracerObject.transform;
        _mainCamera = Camera.main;
        _tracerObject.SetActive(false);
    }
    
    private void Update()
    {
        HandleInput();
    }
    
    private void OnDestroy()
    {
        if (_levelManager != null)
        {
            _levelManager.OnLevelInitialized -= OnLevelInitialized;
        }
    }
    
    private void OnLevelInitialized(LevelConfig config) => PrepareStrokePoints();

    private void PrepareStrokePoints()
    {
        _calculatedPathPoints.Clear();
        StrokeData data = _levelManager.GetCurrentStrokeData();

        if (data.Waypoints == null || data.Waypoints.Length < 2) return; // at least 2 waypoints make a stroke vector
        
        for (int i = 0; i < data.Waypoints.Length - 1; i++)
        {
            Vector2 start = data.Waypoints[i];
            Vector2 end = data.Waypoints[i + 1];
            
            _calculatedPathPoints.Add(start);

            float distance = Vector2.Distance(start, end);
            int intermediateCount = Mathf.FloorToInt(distance / _autoGenerationDensity);

            for (int j = 1; j <= intermediateCount; j++)
            {
                float newPoint = (float)j / (intermediateCount + 1);
                _calculatedPathPoints.Add(Vector2.Lerp(start, end, newPoint));
            }
        }

        _calculatedPathPoints.Add(data.Waypoints[data.Waypoints.Length - 1]);
        _targetPointIndex = 1;
    }
    
    private void HandleInput()
    {
        if (Input.touchCount == 0) return;
        
        Touch touch = Input.GetTouch(0);
        
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(touch.position);
        worldPos.z = 0f; 

        if (touch.phase == TouchPhase.Began)
        {
            Vector2 activeStartPoint = _calculatedPathPoints[_targetPointIndex - 1];
            if (Vector2.Distance(worldPos, activeStartPoint) <= _touchThreshold)
            {
                _isTracingActive = true;
                _tracerObject.SetActive(true);
                _tracerTransform.position = _calculatedPathPoints[0];
                _lastStampPosition = _tracerTransform.position;
                
                SpawnMaskStamp();
            }
            
            OnPlayerActivity?.Invoke();
        }
        else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && _isTracingActive)
        {
            UpdateTracing(worldPos);
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            _isTracingActive = false;
        }
    }
    
    private void UpdateTracing(Vector3 touchPosition)
    {
        if (_targetPointIndex >= _calculatedPathPoints.Count) return;
        
        Vector2 currentTarget = _calculatedPathPoints[_targetPointIndex];
        Vector2 previousTarget = _calculatedPathPoints[_targetPointIndex - 1];
        
        if (IsOnPathStroke(touchPosition, previousTarget, currentTarget))
        {
            Vector2 projectedPos = Vector2Extensions.ProjectOnSegment(touchPosition, previousTarget, currentTarget);
            _tracerTransform.position = new Vector3(projectedPos.x, projectedPos.y, 0f);

            TrySpawnStamp();
            
            if (Vector2.Distance(_tracerTransform.position, currentTarget) <= _touchThreshold)
            {
                _targetPointIndex++;
                
                if (_targetPointIndex >= _calculatedPathPoints.Count)
                {
                    CompleteStroke();
                }
            }
        }
        else
        {
            _isTracingActive = false;
        }
    }

    #region Private Helper Methods
    private bool IsOnPathStroke(Vector3 position, Vector2 start, Vector2 end)
    {
        Vector2 projection = Vector2Extensions.ProjectOnSegment(position, start, end);
        return Vector2.Distance(position, projection) <= _touchThreshold;
    }

    private void TrySpawnStamp()
    {
        if (Vector3.Distance(_tracerTransform.position, _lastStampPosition) >= _stampIntervalDistance)
        {
            SpawnMaskStamp();
        }
    }

    private void SpawnMaskStamp()
    {
        Instantiate(_maskStampPrefab, _tracerTransform.position, Quaternion.identity,
            _levelManager.GetStrokeContainer());
        _lastStampPosition = _tracerTransform.position;
    }

    private void CompleteStroke()
    {
        _isTracingActive = false;
        _tracerObject.SetActive(false);
        
        _levelManager.CompleteCurrentStroke();
        PrepareStrokePoints();
    }

    private void ResetCurrentStroke()
    {
        _isTracingActive = false;
        _tracerObject.SetActive(false);
        
        Transform currentContainer = _levelManager.GetStrokeContainer();
        if (currentContainer is not null)
        {
            foreach (Transform child in currentContainer)
            {
                if (child.name.Contains(_maskStampPrefab.name))
                {
                    Destroy(child.gameObject);
                }
            }
        }

        _targetPointIndex = 1;
    }
    #endregion
    
    #region Public Methods
    
    public Vector3 GetCurrentTracerPosition()
    {
        if (_targetPointIndex <= 1) return _calculatedPathPoints[0];
        return _calculatedPathPoints[_targetPointIndex - 1];
    }

    public Vector3 GetCurrentTargetPosition()
    {
        if (_targetPointIndex >= _calculatedPathPoints.Count) 
            return _calculatedPathPoints[_calculatedPathPoints.Count - 1];
        
        return _calculatedPathPoints[_targetPointIndex];
    }
    #endregion
}