using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public UInt32 _money;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Game.Instance()._player = gameObject;
        Game.Instance().EventBus()._onTick += OnTick;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTick()
    {
        Debug.Log($"Player: {_money} Time Shekels");
        ++_money;
    }    
}
