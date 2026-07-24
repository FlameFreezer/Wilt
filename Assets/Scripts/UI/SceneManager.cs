using UnityEngine;

public class SceneManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPressStart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Play");
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
}
