using DRockInputBridge;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 游戏内debug面板
/// </summary>
public class Debug_InputRecPanel : MonoBehaviour
{
    [SerializeField] private Transform contentTransform;
    [SerializeField] private GameObject uiPrefab;

    private PushdownFSM debugFSM;
    private Stack<GameObject> uiStack = new();

    private void Start()
    {
        debugFSM ??= InputManager.Instance.FSM;

        debugFSM.OnInputIn += AddUI;
        debugFSM.OnInputOut += RemoveUI;
    }

    private void OnDisable()
    {
        debugFSM.OnInputOut -= AddUI;
        debugFSM.OnInputIn -= RemoveUI;
    }

    private void AddUI(IInputReceiver receiver)
    {
        var obj = Instantiate(uiPrefab, contentTransform);
        obj.GetComponent<TextMeshProUGUI>().text = receiver.ToString();
        uiStack.Push(obj);
    }

    private void RemoveUI(IInputReceiver receiver)
    {
        var obj = uiStack.Pop();
        Destroy(obj);
    }
}
