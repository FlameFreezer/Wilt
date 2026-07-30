using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private FMOD.Studio.Bus masterBus;
    private static bool isMuted = false;
    private string busPath = "bus:/";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        masterBus = FMODUnity.RuntimeManager.GetBus(busPath);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void ToggleAudio()
    {
        //AudioListener.volume = AudioListener.volume == 0f ? 1f : 0f;
        if (isMuted) {
            masterBus.setVolume(1);
        } else {
            masterBus.setVolume(0);
        }

        isMuted = !isMuted;
    }
}
