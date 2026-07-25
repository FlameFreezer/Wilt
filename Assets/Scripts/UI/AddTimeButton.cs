using TMPro;
using UnityEngine;

public class AddTimeButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Game.Instance().EventBus().onGlobalTimeAdded += UpdateDisplay;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateDisplay()
    {
        GlobalTimer globalTimer = Game.Instance().globalTimer;
        GetComponent<TextMeshProUGUI>().text = $"Add Time\n${globalTimer.GetAddTimeCost()}";
    }
}
