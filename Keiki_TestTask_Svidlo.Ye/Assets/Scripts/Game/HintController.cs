using System.Collections;
using UnityEngine;
using Zenject;
using Data;

public class HintController : MonoBehaviour
{
    [Header("Hint Visuals")]
    [SerializeField] private GameObject _hintGuideVisual;
    [SerializeField] private float _fingerSpeed = 3f;

    private TracingMechanic _tracingMechanic;
    private AudioManager _audioManager;
    private CategoryType _currentCategory;
    private LevelManager _levelManager;
    
    private float _idleTimer;
    private bool _hasPlayedVoiceHint;
    private bool _canShowVisualHint = false;
    private Coroutine _fingerAnimationCoroutine;

    [Inject]
    public void Construct(TracingMechanic tracingMechanic, AudioManager audioManager, 
                            CategoryType categoryType, LevelManager levelManager)
    {
        _tracingMechanic = tracingMechanic;
        _audioManager = audioManager;
        _currentCategory = categoryType;
        _levelManager = levelManager;
        
        _levelManager.OnLevelInitialized += OnLevelChanged;
        _levelManager.OnLevelReadyForHint += EnableHintsDirectly;
    }

    private void Start()
    {
        _hintGuideVisual.SetActive(false);
        _tracingMechanic.OnPlayerActivity += ResetIdleTimer;
        
        TriggerInitialVoiceInstruction();
    }

    private void Update()
    {
        if (!_canShowVisualHint) return;
        
        _idleTimer += Time.deltaTime;
        
        if (_idleTimer >= 7f && !_hasPlayedVoiceHint)
        {
            _hasPlayedVoiceHint = true;
            if (_audioManager is not null)
            {
                TriggerInitialVoiceInstruction();
            }
        }

        if (_idleTimer >= 14f && _fingerAnimationCoroutine == null)
        {
            _fingerAnimationCoroutine = StartCoroutine(AnimateVisualHintRoutine());
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
        _hintGuideVisual.SetActive(false);
    }
    
    private void TriggerInitialVoiceInstruction()
    {
        _canShowVisualHint = false; 
        
        if (_audioManager is not null)
        {
            _audioManager.PlayInstruction(_currentCategory);
        }
    }
    
    private void EnableHintsDirectly()
    {
        _canShowVisualHint = true;
        ResetIdleTimer();
    }

    private IEnumerator AnimateVisualHintRoutine()
    {
        _hintGuideVisual.SetActive(true);

        while (true)
        {
            Vector3 startPos = _tracingMechanic.GetCurrentTracerPosition();
            Vector3 targetPos = _tracingMechanic.GetCurrentTargetPosition();

            _hintGuideVisual.transform.position = startPos;

            while (Vector3.Distance(_hintGuideVisual.transform.position, targetPos) > 0.1f)
            {
                _hintGuideVisual.transform.position = Vector3.MoveTowards(
                    _hintGuideVisual.transform.position, 
                    targetPos,
                    _fingerSpeed * Time.deltaTime
                );
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnLevelChanged(LevelConfig _)
    {
        ResetIdleTimer();
        TriggerInitialVoiceInstruction();
        
        _audioManager.OnInstructionAudioFinished += HandleInitialAudioFinished;
    }

    private void HandleInitialAudioFinished()
    {
        _canShowVisualHint = true;
        _audioManager.OnInstructionAudioFinished -= HandleInitialAudioFinished;
        
        ResetIdleTimer();
    }
    
    private void OnDestroy()
    {
        if (_tracingMechanic is not null) _tracingMechanic.OnPlayerActivity -= ResetIdleTimer;
        if (_levelManager is not null)
        {
            _levelManager.OnLevelInitialized -= OnLevelChanged;
            _levelManager.OnLevelReadyForHint -= EnableHintsDirectly;
        }
        if (_audioManager is not null)
        {
            _audioManager.OnInstructionAudioFinished -= HandleInitialAudioFinished;
        }
    }
}