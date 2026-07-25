using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioMuter : MonoBehaviour
{
    [SerializeField] StudioEventEmitter[] audios;
    [SerializeField] InputActionReference muteButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        muteButton.action.Enable();
        // Subscribe to the action ('Player' is the Action Map, 'Jump' is the Action)
        muteButton.action.performed += muteAll;
    }

    private void OnDisable()
    {
        // Unsubscribe and disable to avoid memory leaks
        muteButton.action.performed -= muteAll;
        muteButton.action.Disable();
    }
    void muteAll(InputAction.CallbackContext context)
    {
        foreach (StudioEventEmitter audio in audios)
        {
            audio.EventInstance.setVolume(0);
        }
    }
}

