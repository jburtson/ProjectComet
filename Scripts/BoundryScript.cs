/* Jonathan Burtson
 * 4/17/2018
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundryScript : MonoBehaviour {
    public float damage = 0.3f; // damage done per update
    float scrollSpeed = 0.40f;
    float scrollSpeed2 = 0.40f;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    // Applies an upwards force to all rigidbodies that enter the trigger.
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 8 && other.transform.parent.GetComponent<Ship>()){ // layer "ship", the player
            other.transform.parent.GetComponent<Ship>().hp -= damage; //damage player
        }
        else if (other.gameObject.layer == 9 && other.GetComponent<EnemyShip>()){ // layer "enemyShip"
            other.GetComponent<EnemyShip>().hp -= damage; //damage enemy ship
        }
    }
    void FixedUpdate()
    {

        float offset = Time.time * scrollSpeed;
        float offset2 = Time.time * scrollSpeed2;
        transform.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset2, -offset);
    }
}
