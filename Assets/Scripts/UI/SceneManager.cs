using FMODUnity;
using System.Collections;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    //Audios
    [SerializeField] StudioEventEmitter playSound;
    int clipLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Game.Instance().EventBus().onGlobalTimerExhausted += HandleGlobalTimerExhausted;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPressStart()
    {
        playSound.Play();
        StartCoroutine(playWait());
    }

    public void OnPressSettings()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SettingsScene");
    }

    public void OnPressCredits()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("CreditsScene");
    }

    public void OnPressTitle()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
    }

	public void HandleGlobalTimerExhausted() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("CreditsScene");
	}


    IEnumerator playWait()
    {
        
        yield return new WaitForSeconds(1);
        UnityEngine.SceneManagement.SceneManager.LoadScene("FinalPlay");
    }
}
