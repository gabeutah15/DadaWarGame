﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    //fog of war
    public GameObject fogOfWarPlane;
    public GameObject[] playerUnitsInitial;
    public List<GameObject> playerUnits;
    public LayerMask fogLayer;
    public float radius;
    private float radiusSquared { get { return radius * radius; } }
    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;
    float timerForFogUpdate = 0;
    [SerializeField]
    float fogUpdateInterval = .1f;

    private void Initialize()
    {
        playerUnitsInitial = GameObject.FindGameObjectsWithTag("AI");
        int unitNum = -1;
        playerUnits = new List<GameObject>();
        for (int i = 0; i < playerUnitsInitial.Length; i++)
        {
            AIControl control = playerUnitsInitial[i].GetComponent<AIControl>();
            if(unitNum != control.selectedUnitNum)
            {
                unitNum = control.selectedUnitNum;
                playerUnits.Add(playerUnitsInitial[i]);//Not that this fets the 'first' unit which is actually the back right unit of a formation
            }
        }

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

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        //fog of war section
        timerForFogUpdate += Time.deltaTime;
        if (timerForFogUpdate > fogUpdateInterval)
        {
            timerForFogUpdate = 0;
            for (int j = 0; j < playerUnits.Count; j++)
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
        }

        //end fog section
    }
}