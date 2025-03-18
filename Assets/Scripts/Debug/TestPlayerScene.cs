using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DRockInputBridge;
using UnityEngine;

public class TestPlayerScene : MonoBehaviour
{
    public PlayerManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = PlayerManager.Instance;
        InputManager.Instance.Push(manager.PlayerInput);

    }

    [ContextMenu("Test")]
    private void Test()
    {
        manager.PlayerInventory.AddToInventory(manager.ItemDatabase.GetItem(0), 100);
    }
}
