using FMODUnity;
using System.Collections;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
	[SerializeField]
	private GameObject _settingsMenu;

    //Audios
    [SerializeField] StudioEventEmitter playSound;
    int clipLength;

	private bool _oldDialogueActive = false;

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
		_oldDialogueActive = Game.Instance().dialogueActive;
		Game.Instance().dialogueActive = true;
		Game.Instance().clickthroughEnabled = false;
		Game.Instance().EventBus().OnPauseRequested();

		_settingsMenu.SetActive(true);
    }

	public void OnPressSettingsBack() {
		Game.Instance().dialogueActive = _oldDialogueActive;
		Game.Instance().clickthroughEnabled = true;
		Game.Instance().EventBus().OnPauseRequested();

		_settingsMenu.SetActive(false);
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
        
        yield return new WaitForSeconds(3);
        UnityEngine.SceneManagement.SceneManager.LoadScene("FinalPlay");
    }
}
