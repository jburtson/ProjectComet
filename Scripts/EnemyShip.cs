/* Jonathan Burtson
 * 4/17/18
 * EnemyShip (this class) handles alien ship behavior
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyShip : MonoBehaviour {
    public int race;
    public int state = 0;
    public float fuel = 0;
    public float hp = 100;
    public float[] angerAtRace; // anger towards a race (like if taking bullets from a certain race a lot)
    public float angerAtPlayer = 0;
    public float[] baseAngerAtRace; // race's outlook at beginning of journey
    public float baseAngerAtPlayer;
    public List<Vector3> targetQueue;
    private Rigidbody rb;
    public float speed = 10;
    public int type = 0;
    public Generator gen;
    public EnemyShipScanner scannerCollider;
    public GameObject bulletPrefab;
    //public GameObject boom;
    public GameObject enemy;
    public AudioClip alert;
    public AudioClip thud;
    public AudioClip fire;
    private AudioSource source;
    public int alertFreq = 100;
    private float detectionDistance; // distance at which ship disregards enemy

    public List<Collider> triggerList = new List<Collider>();

    // Use this for initialization
    void Start() {
        source = GetComponent<AudioSource>();
        gen = GameObject.Find("Generator").GetComponent<Generator>();
        // find the "sight" collider child object
        scannerCollider = transform.Find("Sight").GetComponent<EnemyShipScanner>();

        //angerAtRace = new float[Races.size()];
        baseAngerAtRace = GetSpecies().getAngerAtRaceArray();
        baseAngerAtPlayer = GetSpecies().getAngerAtPlayer();
        angerAtRace = (float[])baseAngerAtRace.Clone();
        angerAtPlayer = baseAngerAtPlayer;
        detectionDistance = scannerCollider.colliderNetSize;
    }

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update() {
        if (hp <= 0f) { // On death
            //GameObject temp = (GameObject)Instantiate(boom, this.transform.position, Quaternion.identity); // spawns explosion on death NOT WORKING YET
            gen.explode(this.transform.position);
            //Debug.Log("Ship death at X:" + this.transform.position.x + " Z:" + this.transform.position.z + "\nFlying to X:" + getCurrentTarget().x + " Z:" + getCurrentTarget().z);
            //Debug.Log("Ship death",this);
            updateAnger();
            this.GetSpecies().subtractTotalPopulation(); // subtract from total population count
            gen.checkExtinction(race);
            Destroy(this.gameObject);
        }
        switch (state) { // decides what action the ship takes
            case 0: // going aimlessly forward
                if (fuel > 0f) {
                    rb.AddRelativeForce(Vector3.forward * Time.deltaTime * speed * 3f); // fly forward
                    fuel -= 0.1f;
                }
                else {
                    runOutOfFuel();
                }
                break;

            case 1: // pursueing a target
                if (fuel > 0f) {
                    if (hasTarget()) {
                        rb.AddRelativeForce(Vector3.forward * Time.deltaTime * speed * 3f); // fly to target vector
                        Vector3 corrected = new Vector3(-rb.velocity.x * 2, -rb.velocity.y * 2, 0); // compensate for non-forward momentum
                        Vector3 targetDir = (getCurrentTarget() + corrected) - rb.transform.position; // rotate toward target
                                                                                                      //Vector3 targetDir = currentTarget - rb.transform.position; // rotate toward target, uncorrected for velocity
                        float step = speed * Time.deltaTime;
                        Vector3 newDir = Vector3.RotateTowards(rb.transform.forward, targetDir, step, 0.0F);
                        Debug.DrawRay(transform.position, newDir, Color.red);
                        rb.transform.rotation = Quaternion.LookRotation(newDir);
                        fuel -= 0.1f;
                        //shoot(targetDir); //CONSTANTLY SHOOT
                        // CHECK FOR IF TARGET IS REACHED
                        /*
                        Vector3 pos = this.transform.position;
                        Vector3 tar = getCurrentTarget();
                        float range = 2f;
                        if (Mathf.Abs(pos.x - tar.x) < range && Mathf.Abs(pos.z - tar.z) < range) { // reaches target
                            reachedTarget();
                        }
                        */
                    }
                    else {
                        Debug.Log("Ship has no targets, but in state 1");
                        runOutOfFuel();
                    }
                }
                else {
                    runOutOfFuel();
                }
                break;

            case 2: // FIGHT
                if (enemy==null) {
                    state = 1;
                    Debug.Log("Target Destroyed.");
                    break;
                }
                if (Vector3.Distance(enemy.transform.position, this.transform.position) > detectionDistance) {
                    state = 1;
                    Debug.Log("Target Evaded.");
                    break;
                }

                searchForEnemy(); // will find new target if no longer in combat state

                if (Time.frameCount % 100 == 0) { // fire every 100th frame
                    shoot(enemy.transform.position);
                }
                break;

            case 3: // Out of fuel
                if (Time.frameCount % alertFreq == 0) {
                    // makes alert noise
                    source.PlayOneShot(alert, .7f);
                }
                break;
        }
    }

    // When scanner finds something new
    public void initialSearch() {
        // FIGHT FIRST ENEMY SHIP ENCOUNTERED
        if (state != 2 && triggerList.Count > 0) {
            Collider c = triggerList[triggerList.Count - 1];
            if (c != null) {
                if (c.gameObject.layer == 9) { // enemy ship
                    if (shouldIFightAlien(c.gameObject.GetComponent<EnemyShip>())) {
                        enemy = c.gameObject;
                        state = 2;
                    }
                }
                if (c.gameObject.layer == 8) { // player ship
                    if (shouldIFightPlayer(c.gameObject.GetComponent<Ship>())) {
                        enemy = c.gameObject;
                        state = 2;
                    }
                }
            }
        }
    }

    // Look around for enemies
    public bool searchForEnemy() {
        bool enemyFound = false;
        if (state != 2 && triggerList.Count > 0) { // checks for new enemies if need be
            foreach (Collider c in triggerList) {
                if (c != null) {
                    if (c.gameObject.layer == 9) { // enemy ship
                        if (shouldIFightAlien(c.gameObject.GetComponent<EnemyShip>())) {
                            enemy = c.gameObject;
                            state = 2;
                            enemyFound = true;
                        }
                    }
                    if (c.gameObject.layer == 8) { // player ship
                        if (shouldIFightPlayer(c.gameObject.GetComponent<Ship>())) {
                            enemy = c.gameObject;
                            state = 2;
                            enemyFound = true;
                        }
                    }
                }
                if (enemyFound) break;
            }
        }
        return enemyFound;
    }

    // runs when hit with bullet
    public void onHit(int fromRace, float angerAmount=1f) {
        if (fromRace == -1) angerAtPlayer += angerAmount;
        else if (fromRace == -2) Debug.Log("Bullet not assigned a race");
        else {
            angerAtRace[fromRace] += angerAmount;
        }
        searchForEnemy();
    }

    // determine whether or not this ship should fight an alien ship
    public bool shouldIFightAlien(EnemyShip otherShip) {
        //if (otherShip.race >= 0 && otherShip.race < Races.size()) {
        if (this.angerAtRace.Length == Races.size()) {
            float chance = this.GetSpecies().getAggro() + Random.Range(0, 4) + this.angerAtRace[otherShip.race];
            if (otherShip.race == this.race) chance *= 0.5f; // if target is same race, reduce chance of fighting
            if (chance >= 10) {
                return true;
            }
            else {
                return false;
            }
        }
        else return false;
    }
    // determine whether or not this ship should fight the player ship
    public bool shouldIFightPlayer(Ship otherShip) {
        float chance = this.GetSpecies().getAggro() + Random.Range(0, 4) + this.angerAtPlayer;
        if (chance >= 10) {
            return true;
        }
        else {
            return false;
        }
    }

    // Takes the anger stats of ship, and updates the races anger stats to incorporate it
    public void updateAnger() {
        // change the attitude of race depending on ships attitude toward races
        float newAngerAtPlayer = (this.GetSpecies().getAngerAtPlayer() + this.angerAtPlayer - this.baseAngerAtPlayer)/2;
        this.GetSpecies().setAngerAtPlayer(newAngerAtPlayer);
        for (int i = 0; i < Races.size(); i++) {
            // if change in anger
            if (this.angerAtRace[i] != this.baseAngerAtRace[i]) {
                float newAngerAtRace = (this.GetSpecies().getAngerAtRace(i) + this.angerAtRace[i] - this.baseAngerAtRace[i]) / 2;
                //if (race == i) newAngerAtRace /= 2; // half anger further if it's for own race
                this.GetSpecies().setAngerAtRace(newAngerAtRace, i);
            }
        }
                    
    }

    private void OnCollisionEnter(Collision other) {
        if (hp > 0) { // JUST AS A POSSIBLE FIX FOR PLAYONESHOT ERROR
            // if both from the same race, start ignoring collisions
            if (other.gameObject.layer == transform.gameObject.layer && this.race == other.gameObject.GetComponent<EnemyShip>().race) {
                Physics.IgnoreCollision(other.collider, GetComponent<Collider>());
            }
            else if (other.gameObject.GetComponent<Rigidbody>()) { // if collided object has rigidbody
                // take damage equal to difference in velocities
                float collisionMagnitude = (rb.velocity - other.gameObject.GetComponent<Rigidbody>().velocity).magnitude;
                hp -= collisionMagnitude;
                source.PlayOneShot(thud, Mathf.Clamp(collisionMagnitude / 2, .05f, 2f)); // play thud noise

                // get angry
                if (other.gameObject.layer == 8) onHit(-1,.25f); //player
                else if (other.gameObject.layer == 9) onHit(other.gameObject.GetComponent<EnemyShip>().race, .25f); //alien ship
            }
        }
    }
    public void shoot(Vector3 target) {
        source.PlayOneShot(fire, .3f); // play thud noise
        GameObject bullet = Instantiate(bulletPrefab,this.transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().fromRace = this.race;
        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());
        bullet.transform.LookAt(target);
    }

    // Useful methods!!!
    public Vector3 getCurrentTarget() {
        if (targetQueue.Count == 0) {
            Debug.Log("No current target to get.");
            return Vector3.positiveInfinity;
        }
        return targetQueue[0];
    }
    public bool hasTarget() {
        return targetQueue.Count>0;
    }
    public void flyTo(Vector3 to) {
        state = 1;
        targetQueue.Add(to);
    }
    public void flyTo(GameObject to) {
        state = 1;
        targetQueue.Add(to.transform.position);
    }
    public void flyHome() {
        flyTo(GetSpecies().getHome());
    }
    public void queueTarget(Vector3 to) {
        targetQueue.Add(to);
    }
    public void reachedTarget() {
        if (hasTarget()) {
            targetQueue.RemoveAt(0);
        }
        if (!hasTarget()) {
            // if no targets in Queue, go home
            //targetQueue.Add(GetSpecies().getHome().transform.position);
        }
    }
    public void runOutOfFuel() {
        state = 3;
        gameObject.GetComponent<Light>().range = transform.localScale.z*2;
    }
    public Species GetSpecies() {
        return Races.get(race);
    }
}
