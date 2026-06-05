using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;
    public Slider volumeSlider;

    private const string VolumeKey = "MusicVolume";

    [SerializeField]
    private Image volumeImage;

    [SerializeField]
    private Sprite volumeSprite, noVolumeSprite, volume1, volume2;

    private float lastVolume = 0.5f;

    void Start()
    {
        // Cargar volumen guardado o establecerlo en 1 (máximo)
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.5f);
        musicSource.volume = savedVolume;
        lastVolume = savedVolume;
        CheckSprite(savedVolume);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // Asegurar que la música se reproduce en loop
        if (!musicSource.isPlaying)
        {
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void SetVolume(float volume)
    {
        CheckSprite(volume);
        if (volume > 0)
            lastVolume = volume;
        musicSource.volume = volume;
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }

    private void CheckSprite(float volume)
    {
        if (volume <= 0)
            volumeImage.sprite = noVolumeSprite;
        else if (volume >= 0.7f)
            volumeImage.sprite = volumeSprite;
        else if (volume >= 0.33f)
            volumeImage.sprite = volume2;
        else
            volumeImage.sprite = volume1;
    }
    public void TurnVolumeOff()
    {
        if (musicSource.volume == 0)
        {
            SetVolume(lastVolume);
            volumeSlider.value = lastVolume;
        }
        else
        {
            SetVolume(0);
            volumeSlider.value = 0;
        }

    }
}