using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIBody : MonoBehaviour
{
    Collider collider;
    [SerializeField]
    float health = 1;
    // Start is called before the first frame update
    void Start()
    {
        collider = this.GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //ContactPoint[] contacts = collision.contacts;

        //for (int i = 0; i < contacts.Length; i++)
        //{

        //}

        var parent = collision.gameObject.GetComponentInParent<AIControl>();
        if (parent)
        {
            //Debug.Log("collided with child of ai control");
            health--;

        }

        //this doesn't work for finding just collision with playerweapon (spear) and nor does below with compare tag
        //need rigidbody for collision on navagent, can only have one rigidbody on object and children combined
        //var spear = collision.gameObject.GetComponent<PlayerWeapon>();
        //if (spear)
        //{
        //    Debug.Log("collision with spear");
        //}


        //Collider myCollider = collision.contacts[0].thisCollider;//this should get actually contacted collider?

        //if (myCollider.gameObject.CompareTag("PlayerWeapon"))
        //{
        //    Debug.Log("hit by weapon of player unit");
        //    health--;
        //}

        if (health <= 0)
        {
            this.gameObject.SetActive(false);
            //Destroy(this.gameObject);
        }
    }

}
