using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using UnityEngine;
using UnityEngine.AI;

public class StrollingEnemyAISpawner : MonoBehaviour
{
    private GameObject[] strollingEnemyAiLocs;
    public GameObject SwordsmanPrefab;
    private List<GameObject> strollingEnemies;
    private int rows = 5;
    private int cols = 7;
    private bool[] moving;
    private float[] distBetweenExtremeSoldiers;

    [SerializeField]
    public Territory territory;

    void Awake()
    {
        strollingEnemyAiLocs = GameObject.FindGameObjectsWithTag("RandomEnemyAILocs");
        moving = new bool[strollingEnemyAiLocs.Length];
        distBetweenExtremeSoldiers = new float[strollingEnemyAiLocs.Length];
        strollingEnemies = new List<GameObject>();
        SpawnStrollingEnemyAIs();
        startStrollAI();
    }

    void SpawnStrollingEnemyAIs()
    {
        for (int x = 0; x < strollingEnemyAiLocs.Length; x++)
        {
            //strollingEnemies.Add(Instantiate(SwordsmanPrefab, strollingEnemyAiLocs[x].transform.position, Quaternion.identity) as GameObject) ;
            //strollingEnemies.ElementAt(x).GetComponent<EnemyAIBody>().territory = territory;
            //go over set rows and cols and spawn row x cols number of AIs per AILocation(count given by strollingEnemyAiLocs.Length)
            for(int i=0; i< rows; i++)
            {
                for(int j=0; j< cols; j++)
                {
                    strollingEnemies.Add(Instantiate(SwordsmanPrefab, strollingEnemyAiLocs[x].transform.position + new Vector3(2.5f *j , 0, -2.5f*i), Quaternion.identity) as GameObject);
                    strollingEnemies.ElementAt(strollingEnemies.Count -1).GetComponent<EnemyAIBody>().territory = territory;

                    //strollingEnemies.ElementAt(strollingEnemies.Count - 1).GetComponent<NavMeshAgent>().SetDestination(new Vector3(50, 0, 0));
                    //TODO add AI for stroll motion motion
                    /*if ( (j == cols - 1) && (i==0) )
                    {
                        if (x != 0)
                        {
                            GameObject firstSoldier = strollingEnemies.ElementAt(x * (rows * cols - 1) + 1);//gives first soldier of strolling soldier group in topmost row
                            GameObject lastSoldier = strollingEnemies.ElementAt(x * (rows * cols - 1) + 1);//gives last soldier of strolling soldier group in topmost row
                        }
                    }*/
                }
            }
            distBetweenExtremeSoldiers[x] = Vector3.Distance(strollingEnemies.ElementAt((x * (rows * cols - 1)) + x).transform.position,
                strollingEnemies.ElementAt((x * (rows * cols - 1)) + x + (cols - 1)).transform.position);
        }
    }

    void startStrollAI()
    {   
        int soldierIndex = 0;
        for (int x = 0; x < strollingEnemyAiLocs.Length; x++)
        {
            /*float distBetTopExtremeSoldiers = Vector3.Distance(strollingEnemies.ElementAt((x * (rows * cols - 1)) + x).transform.position,
                strollingEnemies.ElementAt((x * (rows * cols - 1)) + x + (cols - 1)).transform.position);*/
            
            for (int i = 0; i < rows; i++)
            {
                Vector3 destination;
                //destination = strollingEnemyAiLocs[x].transform.position + new Vector3(2 * distBetTopExtremeSoldiers, 0, -2.5f * i);
                destination = strollingEnemyAiLocs[x].transform.position + new Vector3(2 * distBetweenExtremeSoldiers[x], 0, -2.5f * i);
                for (int j = 0; j < cols; j++)
                {
                    if (strollingEnemies.ElementAt(soldierIndex) != null && strollingEnemies.ElementAt(soldierIndex).GetComponent<NavMeshAgent>() != null)
                        strollingEnemies.ElementAt(soldierIndex).GetComponent<NavMeshAgent>().SetDestination(destination);

                    soldierIndex++;
                }
            }
            moving[x] = true;
        }
    }

    void FixedUpdate()
    {
        //TODO check if the top left soldier of a strolling group has reached its original spawn loc or the top right soldier of a strolling group reached the right most stroll loc
        /*for (int x = 0; x < strollingEnemyAiLocs.Length; x++)
        {
            float distBetTopExtremeSoldiers = Vector3.Distance(strollingEnemies.ElementAt((x * (rows * cols - 1)) + x).transform.position,
                strollingEnemies.ElementAt((x * (rows * cols - 1)) + x + (cols - 1)).transform.position);
            Vector3 destination = strollingEnemyAiLocs[x].transform.position + new Vector3(2 * distBetTopExtremeSoldiers, 0, 0);
            if (Vector3.Distance(strollingEnemies.ElementAt((x * (rows * cols - 1)) + x + (cols - 1)).transform.position, destination) < 0.1f)
            {

            }
        }*/
        int soldierIndex = 0;
        for (int x=0; x<strollingEnemyAiLocs.Length; x++)
        {
            //calculate difference between the top rightmost and the max right loc the group can stroll to
            //if its ~ 0 means need to SetDestination to leftmost part of motion range i.e strollingEnemyAiLocs[x].transform.position
            /*float distBetTopExtremeSoldiers = Vector3.Distance(strollingEnemies.ElementAt((x * (rows * cols - 1)) + x).transform.position,
                strollingEnemies.ElementAt((x * (rows * cols - 1)) + x + (cols - 1)).transform.position);*/
            //Vector3 destination = strollingEnemyAiLocs[x].transform.position + new Vector3(2 * distBetTopExtremeSoldiers, 0, 0);
            Vector3 destination = strollingEnemyAiLocs[x].transform.position + new Vector3(2 * distBetweenExtremeSoldiers[x], 0, 0);
            float distanceLeftFromDestination = Vector3.Distance(strollingEnemies.ElementAt((x * (rows * cols - 1)) + x + (cols - 1)).transform.position,
                destination);
            UnityEngine.Debug.Log("distanceLeft from end of stroll range__ "+ distanceLeftFromDestination);
            UnityEngine.Debug.Log("distanceLeft from start of stroll range__ "+ (Vector3.Distance(strollingEnemies.ElementAt((x * (rows * cols - 1)) + x).transform.position, strollingEnemyAiLocs[x].transform.position)));
            /*UnityEngine.Debug.Log("reached end__ " + (strollingEnemies.ElementAt((x * (rows * cols - 1)) + x + (cols - 1)).transform.position == destination));
            UnityEngine.Debug.Log("reached left__ " + (strollingEnemies.ElementAt((x * (rows * cols - 1)) + x).transform.position == strollingEnemyAiLocs[x].transform.position));*/
            
            
            if (distanceLeftFromDestination < 2)
            {
                UnityEngine.Debug.Log("inside reached right__");
                for (int i = 0; i < rows; i++)
                {  
                    destination = strollingEnemyAiLocs[x].transform.position + new Vector3(0, 0, -2.5f * i);
                    for (int j = 0; j < cols; j++)
                    {
                        if(strollingEnemies.ElementAt(soldierIndex)!=null && strollingEnemies.ElementAt(soldierIndex).GetComponent<NavMeshAgent>() != null)
                            strollingEnemies.ElementAt(soldierIndex).GetComponent<NavMeshAgent>().SetDestination(destination);

                            soldierIndex++;
                    }
                }
                moving[x] = false;
            }
            else if( Vector3.Distance(strollingEnemies.ElementAt((x * (rows * cols - 1)) + x).transform.position, strollingEnemyAiLocs[x].transform.position) < 2 && !moving[x])
            {
                UnityEngine.Debug.Log("inside reached left__");
                startStrollAI();
            }
        }
    }
}
