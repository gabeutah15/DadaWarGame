using UnityEngine;
using System.Collections;

public class DragCamera : MonoBehaviour
{
    [SerializeField]
    int cameraDragSpeed = 80;

    [SerializeField]
    float xMaxBound = 30;
    [SerializeField]
    float xMinBound = -30;
    [SerializeField]
    float zMaxBound = 30;
    [SerializeField]
    float zMinBound = -30;

    float minFov = 15f;
    float maxFov = 90f;
    float sensitivity = 10f;

    private void Start()
    {
        zMinBound += Camera.main.transform.position.z;
        zMaxBound += Camera.main.transform.position.z;

        xMinBound += Camera.main.transform.position.x;
        xMaxBound += Camera.main.transform.position.x;
    }


    private void Update()
    {
        float fov = Camera.main.fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;


        if (Input.GetMouseButton(0))
        {
            float speed = cameraDragSpeed * Time.deltaTime;
            Camera.main.transform.position -= new Vector3(Input.GetAxis("Mouse X") * speed, 0, Input.GetAxis("Mouse Y") * speed);

            if (Camera.main.transform.position.x > xMaxBound)
            {
                Camera.main.transform.position = new Vector3(xMaxBound, Camera.main.transform.position.y, Camera.main.transform.position.z);
            }

            if (Camera.main.transform.position.x < xMinBound)
            {
                Camera.main.transform.position = new Vector3(xMinBound, Camera.main.transform.position.y, Camera.main.transform.position.z);
            }

            if (Camera.main.transform.position.z > zMaxBound)
            {
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, zMaxBound);
            }

            if (Camera.main.transform.position.z < zMinBound)
            {
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, zMinBound);
            }
        }
    }
}