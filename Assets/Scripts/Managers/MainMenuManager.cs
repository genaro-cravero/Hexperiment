using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private AudioClip _hoverSound;

    private void Awake()
    {
        SceneManager.LoadScene("PlayableLevel", LoadSceneMode.Additive);
    }

    public void PlayGame()
    {

        var mainCamera = Camera.main;
        var brain = mainCamera.GetComponent<CinemachineBrain>();
        var activeCamera = brain.ActiveVirtualCamera as CinemachineVirtualCameraBase;
        if (activeCamera != null)
            activeCamera.gameObject.SetActive(false);

        WaveManager.Instance.StartWaves();

        SceneManager.UnloadSceneAsync("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayClickSound()
    {
        SoundFXManager.Instance.PlaySound(_clickSound, transform.position);
    }

    public void PlayHoverSound()
    {
        SoundFXManager.Instance.PlaySound(_hoverSound, transform.position);
    }
}
