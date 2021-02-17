using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour
{
    public float sensitivity;

    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {

        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.gameObject.transform.Translate(Vector3.back*sensitivity);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.gameObject.transform.Translate(Vector3.forward * sensitivity);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.gameObject.transform.Translate(Vector3.right * sensitivity);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.gameObject.transform.Translate(Vector3.left * sensitivity);
        }
    }
}
