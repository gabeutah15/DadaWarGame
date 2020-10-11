using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    //fog of war
    public GameObject[] fogOfWarPlanes;
    [HideInInspector]
    public GameObject[] playerUnitsInitial;
    [HideInInspector]
    public List<GameObject> playerUnits;
    private List<float> playerUnitSightDistances;
    public LayerMask fogLayer;
    public float radius;
    private float radiusSquared { get { return radius * radius; } }
    private Mesh[] meshes;
    private Vector3[][] vertices;//int[,] array = new int[4, 2];
    private Color[] colors;
    float timerForFogUpdate = 0;
    [SerializeField]
    float fogUpdateInterval = .1f;

    private void Initialize()
    {
        playerUnitsInitial = GameObject.FindGameObjectsWithTag("AI");
        int unitNum = -1;
        //could probably stick with teh array here because i don't think playerUnits ever decreases in size
        playerUnits = new List<GameObject>();
        playerUnitSightDistances = new List<float>();
        for (int i = 0; i < playerUnitsInitial.Length; i++)
        {
            AIControl control = playerUnitsInitial[i].GetComponent<AIControl>();
            if(unitNum != control.selectedUnitNum)
            {
                unitNum = control.selectedUnitNum;
                playerUnits.Add(playerUnitsInitial[i]);//Not that this fets the 'first' unit which is actually the back right unit of a formation
                playerUnitSightDistances.Add(playerUnitsInitial[i].GetComponent<AIControl>().lineOfSight);
            }
        }

        vertices = new Vector3[fogOfWarPlanes.Length][];
        meshes = new Mesh[fogOfWarPlanes.Length];

        for (int i = 0; i < fogOfWarPlanes.Length; i++)
        {
            fogOfWarPlanes[i].SetActive(true);
            meshes[i] = fogOfWarPlanes[i].GetComponent<MeshFilter>().mesh;
            vertices[i] = new Vector3[meshes[i].vertexCount];
            vertices[i] = meshes[i].vertices;

        }

        colors = new Color[vertices[0].Length];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }
        UpdateColor();
    }

    void UpdateColor()
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].colors = colors;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //fog of war section
        timerForFogUpdate += Time.deltaTime;
        if (timerForFogUpdate > fogUpdateInterval)
        {
            timerForFogUpdate = 0;
            for (int j = 0; j < playerUnits.Count; j++)
            {
                //could probably cache some of these positions to be more performant, or even just do a playerUnits array instead of list and make it of vector3s not game objects
                Vector3 playerPosition = playerUnits[j].transform.position;
                float playerZ = playerPosition.z;
                if(playerZ > DragCamera.zMaxBound)
                {
                    DragCamera.zMaxBound = playerZ;
                }

                Ray ray = new Ray(transform.position, playerPosition - transform.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 200, fogLayer, QueryTriggerInteraction.Collide))
                {
                    radius = playerUnitSightDistances[j];

                    for (int x = 0; x < fogOfWarPlanes.Length; x++)
                    {
                        for (int i = 0; i < vertices[x].Length; i++)
                        {
                            //Debug.Log("X:" + x);
                            //Debug.Log("i:" + i);
                            //Debug.Log("j:" + j);



                            Vector3 v = fogOfWarPlanes[x].transform.TransformPoint(vertices[x][i]);
                            float dist = Vector3.SqrMagnitude(v - hit.point);
                            if (dist < radiusSquared)
                            {
                                float alpha = Mathf.Min(colors[i].a, dist / radiusSquared);
                                colors[i].a = alpha;
                            }
                        }
                    }
                    UpdateColor();
                }
            }
        }

        //end fog section
    }
}
