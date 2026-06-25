using UnityEngine;
using Data;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [Header("Category Instructions")]
    [SerializeField] private AudioClip _letterCategoryInstructionClip;
    [SerializeField] private AudioClip _numberCategoryInstructionClip;
    [SerializeField] private AudioClip _shapesCategoryInstructionClip;
    
    [Header("Completion Messages")]
    [SerializeField] private AudioClip[] _levelCompletionClip;

    private AudioSource _voiceOverSource;

    private void Awake()
    {
        _voiceOverSource = GetComponent<AudioSource>();
    }
    
    public void PlayInstruction(CategoryType categoryType)
    {
        if (_voiceOverSource is null) return;

        AudioClip clipToPlay = null;
        switch (categoryType)
        {
            case CategoryType.Letters:
                if (_letterCategoryInstructionClip != null)
                {
                    clipToPlay = _letterCategoryInstructionClip;
                }
                break;
            case CategoryType.Numbers:
                if (_numberCategoryInstructionClip != null)
                {
                    clipToPlay = _numberCategoryInstructionClip;
                }
                break;            
            case CategoryType.Shapes:
                if (_shapesCategoryInstructionClip != null)
                {
                    clipToPlay = _shapesCategoryInstructionClip;
                }
                break;
        }

        if (clipToPlay is not null)
        {
            _voiceOverSource.PlayOneShot(clipToPlay);
        }
    }

    public void PlayCompletion()
    {
        if (_levelCompletionClip != null)
        {
            _voiceOverSource.PlayOneShot(_levelCompletionClip[Random.Range(0, _levelCompletionClip.Length)]);
        }
    }
}