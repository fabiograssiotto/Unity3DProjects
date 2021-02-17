using UnityEngine;
using System.Collections;

public class ResetCamera : MonoBehaviour
{
    private float fieldOfView;
    private Vector3 cameraPosition;

    void Start()
    {
        fieldOfView = Camera.main.fieldOfView;
        cameraPosition = Camera.main.transform.position;
    }

    // Called when MouseDown on this gameObject
    void OnMouseDown()
    {
        // Reset Camera to default setting.
        Camera.main.fieldOfView = fieldOfView;
        Camera.main.transform.position = cameraPosition;
    }

    // Exit on Esc
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}