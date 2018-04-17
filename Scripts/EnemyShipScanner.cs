/* Jonathan Burtson
 * 4/17/2018
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipScanner : MonoBehaviour {
    private List<Collider> triggerList = new List<Collider>();
    public float colliderNetSize = 15f;
    EnemyShip parentShip;

    private void Start()
    {
        // calculates adjusted scale of collider so it's same size despite parent's scale
        float colliderRadius = colliderNetSize / transform.parent.transform.localScale.z;
        this.GetComponent<SphereCollider>().radius = colliderRadius;

        //parentShip = transform.parent.gameObject.GetComponent<EnemyShip>();
    }
    void Awake() {
        parentShip = transform.parent.gameObject.GetComponent<EnemyShip>();
    }
    //called when something enters the trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 11){ // layer 11 is "SightColliders", basically this shouldn't collider with triggers meant for detection
            //if the object is not already in the list
            if (!triggerList.Contains(other)){
                //add the object to the list
                triggerList.Add(other);
                updateTriggerList();
                parentShip.initialSearch();
            }
        }
    }

    //called when something exits the trigger
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 11){ // layer 11 is "SightColliders", basically this shouldn't collider with triggers meant for detection
          //if the object is in the list
            if (triggerList.Contains(other)){
                //remove it from the list
                triggerList.Remove(other);
                updateTriggerList();
            }
        }
    }
    public List<Collider> getTriggerList(){
        return triggerList;
    }
    private void updateTriggerList() {
        parentShip.triggerList = triggerList;
    }
}
