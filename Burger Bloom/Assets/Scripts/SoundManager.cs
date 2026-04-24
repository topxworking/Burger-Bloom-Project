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

    [Header("BGM")]
    public AudioSource bgmSource;
    public AudioClip bgmClip;

    private const string KEY_BGM = "BGMEnabled";

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        bool bgmOn = PlayerPrefs.GetInt(KEY_BGM, 1) == 1;
        if (bgmClip) bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        if (bgmOn) bgmSource.Play();
    }

    public bool IsBGMOn() => bgmSource.isPlaying;

    public void SetBGM(bool on)
    {
        if (on) bgmSource.Play();
        else bgmSource.Stop();
        PlayerPrefs.SetInt(KEY_BGM, on ? 1 : 0);
        PlayerPrefs.Save();
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
        sfxSource.PlayOneShot(sauceSquirt, 0.4f);
    }

    public void PlayAssemble()
    {
        sfxSource.PlayOneShot(burgerAssemble, 0.4f);
    }

    public void PlayThrow()
    {
        sfxSource.PlayOneShot(throwSound, 0.4f);
    }

    public void PlayServeSuccess()
    {
        sfxSource.PlayOneShot(serveSuccess, 0.4f);
    }

    public void PlayFootstep()
    {
        sfxSource.PlayOneShot(footstep, 0.2f);
    }

    public void PlayPickup()
    {
        sfxSource.PlayOneShot(pickupSound, 0.4f);
    }
}
