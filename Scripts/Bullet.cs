/* Jonathan Burtson
 * 4/17/2018
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public float speed = 10;
    public float damage = 10;
    public int fromRace = -2; // the racial identity of ship that created bullet. -2 for none, -1 for player, otherwise num of race
    public int framesToDeath = 120; // how many frames the bullet exists for
    private int framesAtStart;

    void Start() {
        //source.PlayOneShot(explosion, .5f); // plays noise on creation
        framesAtStart = Time.frameCount;
    }
    // Update is called once per frame
    void Update () {
        this.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * Time.deltaTime * speed * 10f);
        int timePassed = Time.frameCount - framesAtStart;
        if (timePassed % framesToDeath == 0) {
            Destroy(this.gameObject);
        }
    }
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 8 && other.transform.parent.GetComponent<Ship>()) { // layer "ship", the player
            other.transform.parent.GetComponent<Ship>().hp -= damage; //damage player
            Destroy(this.gameObject);
        }
        else if (other.gameObject.layer == 9 && other.GetComponent<EnemyShip>()) { // layer "enemyShip"
            EnemyShip hitShip = other.GetComponent<EnemyShip>();
            hitShip.hp -= damage; //damage enemy ship
            hitShip.onHit(fromRace);
            Destroy(this.gameObject);
        }
    }
}
