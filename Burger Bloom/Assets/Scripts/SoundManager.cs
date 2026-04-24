using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource loopSource;

    [Header("Audio Clips")]
    public AudioClip grillLoop;
    public AudioClip sauceSquirt;
    public AudioClip burgerAssemble;
    public AudioClip throwSound;
    public AudioClip serveSuccess;
    public AudioClip footstep;
    public AudioClip pickupSound;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void PlayGrill()
    {
        if (loopSource.isPlaying) return;
        loopSource.clip = grillLoop;
        loopSource.loop = true;
        loopSource.Play();
    }

    public void StopGrill()
    {
        loopSource.Stop();
    }

    public void PlaySauce()
    {
        sfxSource.PlayOneShot(sauceSquirt, 0.6f);
    }

    public void PlayAssemble()
    {
        sfxSource.PlayOneShot(burgerAssemble, 0.6f);
    }

    public void PlayThrow()
    {
        sfxSource.PlayOneShot(throwSound, 0.6f);
    }

    public void PlayServeSuccess()
    {
        sfxSource.PlayOneShot(serveSuccess, 0.6f);
    }

    public void PlayFootstep()
    {
        sfxSource.PlayOneShot(footstep, 0.4f);
    }

    public void PlayPickup()
    {
        sfxSource.PlayOneShot(pickupSound, 0.6f);
    }
}
