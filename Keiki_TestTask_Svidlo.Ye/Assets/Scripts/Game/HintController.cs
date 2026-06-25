using System;
using System.Collections;
using UnityEngine;
using Zenject;
using Data;

public class HintController : MonoBehaviour
{
    [Header("Hint Visuals")]
    [SerializeField] private GameObject _hintFingerObject;
    [SerializeField] private float _fingerSpeed = 3f;

    private TracingMechanic _tracingMechanic;
    private AudioManager _audioManager;
    private CategoryType _currentCategory;
    
    private float _idleTimer = 0f;
    private bool _hasPlayedVoiceHint = false;
    private Coroutine _fingerAnimationCoroutine;

    [Inject]
    public void Construct(TracingMechanic tracingMechanic, AudioManager audioManager, CategoryType categoryType)
    {
        _tracingMechanic = tracingMechanic;
        _audioManager = audioManager;
        _currentCategory = categoryType;
    }

    private void Start()
    {
        _hintFingerObject.SetActive(false);
        _tracingMechanic.OnPlayerActivity += ResetIdleTimer;
        
        TriggerVoiceInstruction();
    }

    private void Update()
    {
        _idleTimer += Time.deltaTime;
        
        if (_idleTimer >= 7f && !_hasPlayedVoiceHint)
        {
            TriggerVoiceInstruction();
            _hasPlayedVoiceHint = true;
        }

        if (_idleTimer >= 14f && _fingerAnimationCoroutine == null)
        {
            _fingerAnimationCoroutine = StartCoroutine(AnimateHintFingerRoutine());
        }
    }

    private void ResetIdleTimer()
    {
        _idleTimer = 0f;
        _hasPlayedVoiceHint = false;

        if (_fingerAnimationCoroutine != null)
        {
            StopCoroutine(_fingerAnimationCoroutine);
            _fingerAnimationCoroutine = null;
        }
        _hintFingerObject.SetActive(false);
    }

    private void TriggerVoiceInstruction()
    {
        if (_audioManager is not null)
        {
            _audioManager.PlayInstruction(_currentCategory);
        }
    }

    private IEnumerator AnimateHintFingerRoutine()
    {
        _hintFingerObject.SetActive(true);

        while (true)
        {
            Vector3 startPos = _tracingMechanic.GetCurrentTracerPosition();
            Vector3 targetPos = _tracingMechanic.GetCurrentTargetPosition();

            _hintFingerObject.transform.position = startPos;

            while (Vector3.Distance(_hintFingerObject.transform.position, targetPos) > 0.05f)
            {
                _hintFingerObject.transform.position = Vector3.MoveTowards(
                    _hintFingerObject.transform.position, 
                    targetPos, 
                    _fingerSpeed * Time.deltaTime
                );
                yield return null;
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnDestroy()
    {
        if (_tracingMechanic != null)
        {
            _tracingMechanic.OnPlayerActivity -= ResetIdleTimer;
        }
    }
}