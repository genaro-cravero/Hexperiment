using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _soundVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;

    private void Awake()
    {
        _masterVolumeSlider.value = GetCurrentLoggedVolume("masterVolume");
        _soundVolumeSlider.value = GetCurrentLoggedVolume("soundVolume");
        _musicVolumeSlider.value = GetCurrentLoggedVolume("musicVolume");
    }

    public void SetMasterVolume(float volume)
    {
        _audioMixer.SetFloat("masterVolume", GetLoggedVolume(volume));
    }
    public void SetSoundVolume(float volume)
    {
        _audioMixer.SetFloat("soundVolume", GetLoggedVolume(volume));
    }
    public void SetMusicVolume(float volume)
    {
        _audioMixer.SetFloat("musicVolume", GetLoggedVolume(volume));
    }

    private float GetLoggedVolume(float volume)
    {
        return Mathf.Log10(volume) * 20;
    }

    private float GetCurrentLoggedVolume(string parameterName)
    {
        if (_audioMixer.GetFloat(parameterName, out float volume))
        {
            return Mathf.Pow(10, volume / 20);
        }
        return 1f;
    }
}
