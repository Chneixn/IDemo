using UnityEngine;

public class DropSomething : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    [SerializeField] private Transform handPos;

    [SerializeField] private float dropForce;
    [SerializeField] private bool tryDrop;
    [SerializeField] private bool droped;

    private void Update()
    {
        tryDrop = Input.GetKey(KeyCode.Mouse0);
        droped = Input.GetKeyUp(KeyCode.Mouse0);

        if (tryDrop)
        {

        }

        if (droped)
        {
            GameObject _obj = Instantiate(obj, handPos.position, Quaternion.identity);
            _obj.GetComponent<Rigidbody>().AddForce((transform.up * 0.5f + transform.forward) * dropForce, ForceMode.Impulse);
        }
    }
}
