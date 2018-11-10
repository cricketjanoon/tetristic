using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public bool musicEnabled = true;
    public bool fxEnabled = true;

    [Range(0.0f, 1.0f)] public float musicVolume = 1.0f;
    [Range(0.0f, 1.0f)] public float fxVolume = 1.0f;

    public AudioClip clearRowSound;
    public AudioClip moveSound;
    public AudioClip dropSound;
    public AudioClip gameOverSound;
    public AudioClip backgroundMusic;
    public AudioClip erroSound;
    public AudioSource musicSource;

    public AudioClip[] musicClips;
    private AudioClip randomMusicClip;

    public AudioClip[] vocalClips;
    public AudioClip gameOverVocalClip;
    public AudioClip levelUpVocalClip;

    public AudioClip holdSound;

    public IconToggle musicIconToggle;
    public IconToggle fxIconToggel;

    // Use this for initialization
    private void Start()
    {
        randomMusicClip = GetRandomClip(musicClips);
        PlayBackGroundMusic(randomMusicClip);
    }

    public AudioClip GetRandomClip(AudioClip[] audioClips)
    {
        AudioClip randomClip = audioClips[Random.Range(0, audioClips.Length)];
        return randomClip;
    }

    public void PlayBackGroundMusic(AudioClip musicClip)
    {
        if (!musicEnabled || !musicClip || !musicSource)
            return;

        musicSource.Stop();
        musicSource.clip = musicClip;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void UpdateMusic()
    {
        if (musicSource.isPlaying != musicEnabled)
        {
            if (musicEnabled)
            {
                PlayBackGroundMusic(GetRandomClip(musicClips));
            }
            else
            {
                musicSource.Stop();
            }
        }
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        UpdateMusic();

        if (musicIconToggle)
            musicIconToggle.ToggleIcon(musicEnabled);
    }

    public void ToggleFX()
    {
        fxEnabled = !fxEnabled;
        if (fxIconToggel)
            fxIconToggel.ToggleIcon(fxEnabled);
    }
}