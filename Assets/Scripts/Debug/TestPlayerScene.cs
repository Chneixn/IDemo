using System.Collections;
using System.Collections.Generic;
using DRockInputBridge;
using UnityEngine;


public class TestPlayerScene : MonoBehaviour
{
    public PlayerManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = PlayerManager.Instance;
        manager.PlayerInventory.AddToInventory(manager.ItemDatabase.GetItem(0), 100);
        InputManager.Instance.Push(manager.PlayerInput);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
