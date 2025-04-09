using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    private static AudioManager instance = null;

    public static AudioManager Instance
    {
        get { return instance; }
    }

    [SerializeField] private AudioClip WinTheme;
    [SerializeField] private AudioClip GameTheme;
    [SerializeField] private AudioClip VictoryHonk;
    [SerializeField] private AudioClip SadTrombone;
    [SerializeField] private AudioClip StartSchermTheme;


    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource sfxSource;


    [SerializeField] private AudioClip StartWithThisSound;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if(audioSource.isPlaying == false)
        {
            audioSource.clip = StartWithThisSound;
            audioSource.Play();
        }
    }

    public void PlayStartSchermTheme()
    {
        if (audioSource.isPlaying == false || audioSource.clip != StartSchermTheme)
        {
            audioSource.Stop();
            audioSource.clip = StartSchermTheme;
            audioSource.Play();
        }
    }

    public void PlayWinTheme()
    {
        audioSource.Stop();
        audioSource.clip = WinTheme;
        audioSource.Play();
    }

    public void PlayGameTheme()
    {
        audioSource.Stop();
        audioSource.clip = GameTheme;
        audioSource.Play();
    }

    public void PlayVictoryHonk()
    {
        sfxSource.PlayOneShot(VictoryHonk);
    }
    
    public void PlaySadTrombone()
    {
        sfxSource.PlayOneShot(SadTrombone);
    }


}
