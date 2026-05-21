using UnityEngine;
using UnityEngine.Audio;
public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

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
}
