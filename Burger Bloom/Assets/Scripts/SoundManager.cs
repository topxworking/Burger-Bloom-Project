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
        sfxSource.PlayOneShot(sauceSquirt);
    }

    public void PlayAssemble()
    {
        sfxSource.PlayOneShot(burgerAssemble);
    }

    public void PlayThrow()
    {
        sfxSource.PlayOneShot(throwSound, 0.2f);
    }

    public void PlayServeSuccess()
    {
        sfxSource.PlayOneShot(serveSuccess, 0.2f);
    }
}
