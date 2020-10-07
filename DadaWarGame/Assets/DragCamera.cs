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

    //fog of war
    public GameObject fogOfWarPlane;
    public GameObject[] playerUnits;
    public LayerMask fogLayer;
    public float radius;
    private float radiusSquared { get { return radius * radius; } }
    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;

    private void Initialize()
    {
        playerUnits = GameObject.FindGameObjectsWithTag("AI");
        fogOfWarPlane.SetActive(true);
        mesh = fogOfWarPlane.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        colors = new Color[vertices.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.black;
        }
        UpdateColor();
    }

    void UpdateColor()
    {
        mesh.colors = colors;
    }

    private void Start()
    {
        Initialize();

        zMinBound += Camera.main.transform.position.z;
        zMaxBound += Camera.main.transform.position.z;

        xMinBound += Camera.main.transform.position.x;
        xMaxBound += Camera.main.transform.position.x;
    }


    private void Update()
    {
        //fog of war section
        for (int j = 0; j < playerUnits.Length; j++)
        {
            Ray ray = new Ray(transform.position, playerUnits[j].transform.position - transform.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, fogLayer, QueryTriggerInteraction.Collide))
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 v = fogOfWarPlane.transform.TransformPoint(vertices[i]);
                    float dist = Vector3.SqrMagnitude(v - hit.point);
                    if (dist < radiusSquared)
                    {
                        float alpha = Mathf.Min(colors[i].a, dist / radiusSquared);
                        colors[i].a = alpha;
                    }
                }
                UpdateColor();
            }
        }

        //end fog section


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