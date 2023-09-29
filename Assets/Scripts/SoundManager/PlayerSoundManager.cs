using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSourceForSteps;
    [SerializeField] private AudioSource _audioSourceForGameEvents;
    [SerializeField] private AudioClip _stepSound;
    [SerializeField] private AudioClip _hitSound;

    public void StepSoundAnimation()
    {
        _audioSourceForSteps.volume = 0.1f;
        _audioSourceForSteps.PlayOneShot(_stepSound);
    }

    public void HitSound()
    {
        _audioSourceForGameEvents.clip = _hitSound;
        _audioSourceForGameEvents.volume = 0.1f;
        _audioSourceForGameEvents.Play();
    }

}
