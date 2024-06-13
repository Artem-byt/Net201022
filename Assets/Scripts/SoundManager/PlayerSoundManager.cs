using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSourceForSteps;
    [SerializeField] private AudioSource _audioSourceForHit;
    [SerializeField] private AudioSource _audioSourceForLaser;
    [SerializeField] private AudioClip _stepSound;
    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private AudioClip _lazerSound;

    public void StepSoundAnimation()
    {
        _audioSourceForSteps.volume = 0.1f;
        _audioSourceForSteps.PlayOneShot(_stepSound);
    }

    public void HitSound()
    {
        _audioSourceForHit.clip = _hitSound;
        _audioSourceForHit.volume = 0.1f;
        _audioSourceForHit.Play();
    }

    public void LaserSound()
    {
        if(_audioSourceForLaser.isPlaying)
        {
            return;
        }
        _audioSourceForLaser.clip = _lazerSound;
        _audioSourceForLaser.volume = 0.1f;
        _audioSourceForLaser.Play();
    }

}
