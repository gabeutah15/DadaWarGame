using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AmmoManager : MonoBehaviour
{

    public GameObject AmmoPrefab = null;
    public GameObject LeadPrefab = null;
    public int PoolSize = 100;
    public Queue<Transform> AmmoQueue = new Queue<Transform>();
    public Queue<Transform> LeadQueue = new Queue<Transform>();

    private GameObject[] AmmoArray;
    private GameObject[] LeadBallArray;
    public static AmmoManager AmmoManagerSingleton = null;


    private void Awake()
    {
        if (AmmoManagerSingleton != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        AmmoManagerSingleton = this;
    }
    private void Start()
    {
        AmmoArray = new GameObject[PoolSize];
        LeadBallArray = new GameObject[PoolSize];

        for (int i = 0; i < PoolSize; i++)
        {
            AmmoArray[i] = Instantiate(AmmoPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            Transform ObjTransform = AmmoArray[i].GetComponent<Transform>();
            ObjTransform.parent = transform;
            AmmoQueue.Enqueue(ObjTransform);
            AmmoArray[i].SetActive(false);

            LeadBallArray[i] = Instantiate(LeadPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            ObjTransform = LeadBallArray[i].GetComponent<Transform>();
            ObjTransform.parent = transform;
            LeadQueue.Enqueue(ObjTransform);
            LeadBallArray[i].SetActive(false);
        }

    }

    public static Transform SpawnAmmo(Vector3 Position, Quaternion Rotation, string name)
    {
        Transform SpawnedAmmo = null;
        bool isLead = false;
        if (name.Contains("Catapult"))
        {
            SpawnedAmmo = AmmoManagerSingleton.LeadQueue.Dequeue();
            isLead = true;
            Position += new Vector3(0, 1, 0);
        }
        else
        {
            SpawnedAmmo = AmmoManagerSingleton.AmmoQueue.Dequeue();
        }

        SpawnedAmmo.gameObject.SetActive(true);
        SpawnedAmmo.position = Position;//add to this if it's a lead ammo
        SpawnedAmmo.rotation = Rotation;
        AmmoManagerSingleton.AmmoQueue.Enqueue(SpawnedAmmo);

        //print(SpawnedAmmo);
        return SpawnedAmmo;

    }
}
