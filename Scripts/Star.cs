/* Jonathan Burtson
 * 4/17/18
 * Star (this class) handles star behavior
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct StoredShip {
    public int race;
    public float fuel;
    public float hp;
    public int type;
    public List<Vector3> targetQueue;

    public StoredShip(int race, float fuel, float hp, int type, List<Vector3> targetQueue) {
        this.race = race;
        this.fuel = fuel;
        this.hp = hp;
        this.type = type;
        this.targetQueue = targetQueue;
    }
    public void queueTarget(Vector3 to) {
        targetQueue.Add(to);
    }
    public void reachedTarget() {
        if (targetQueue.Count>0) {
            targetQueue.RemoveAt(0);
        }
        if (targetQueue.Count == 0) {
            // if no targets in Queue, go home
            targetQueue.Add(Races.get(race).getHome().transform.position);
        }
    }
}

public class Star : MonoBehaviour {
    public int starNumber;
	public int race=-1;
	public bool isHome=false;
	public List<StoredShip> residing = new List<StoredShip> (); // list of ships residing, saved as struct StoredShip
    public int population = 0;
    private Generator gen;
	private StarSystem system; // stores planetary bodies in star
	private bool isGenerated=false; // true if StarSystem is already created
    private int[] graphParents; // needed for pathfinding from here

    public bool singleShipMode = false; // for debugging. Makes star spit out ships quickly, and home stars only make 1 ship per race

    // Use this for initialization
    void Start () {
		gen = GameObject.Find ("Generator").GetComponent<Generator>();
        findParents();
        //genShipInvoker();
        //shipLeaveInvoker();
        //Invoke("shipLeaveInvoker", 100);
        //shipLeave();
	}

    // Update is called once per frame
    void Update () {
        ///*
        if (singleShipMode) {
            if (population > 0 && (Time.frameCount % 100 == 0)) {
                shipLeave();
            }
        }
        else {
            // increases population, faster if current population is higher, but still works if currently empty
            if (race >= 0 && Time.frameCount % (2000 + (200/(1+population))) == 0) {
                genShip();
            }
            // a ship ship leaves every few frames, proportional to amount of population
            if (population > 0 && Time.frameCount % (30 + (1000 / population)) == 0) {
                shipLeave();
            }
        }
        //*/
	}
    // Method for creating new ships at this star
    //     if none currently at star, creates ship of same race as star. Otherwise makes another ship of same race as random resident
    void genShip() {
        if (race >= 0) {
            // type of ship. Warship typically if race more agressive
            int shipType = 0;
            if ((Races.get(race).getAggro() + Random.Range(0, 10)) > 8) shipType = 1;
            else if ((Races.get(race).getPop() + Random.Range(0, 10)) > 8) shipType = 2;

            List<Vector3> newTargetQueue = new List<Vector3>();

            int shipRace = race;
            if (residing.Count > 0) shipRace = residing[Random.Range(0, residing.Count)].race;

            // adds to residing list of ships the new generated StoredShip
            residing.Add(new StoredShip(shipRace, shipStartingFuel, shipStartingHP, shipType, newTargetQueue));
            population++;
            Races.get(shipRace).addTotalPopulation(); // add to total population count
        }
    }
    void genShipInvoker() {
        float time = 30 + 15/(1+population); // creates new ship every 45-30 seconds, depending on population
        if (race >= 0) {
            Invoke("genShip", time);
        }
        Invoke("genShipInvoker", time);
    }
    void shipLeave() {
        if (population > 0) {
            StoredShip toLeave = residing[0];
            residing.RemoveAt(0);
            population--;

            GameObject shipSpawn = gen.newShipStarToStar(this.starNumber, toLeave);
            Debug.Log("Ship left");
            // Is ship heading to where it is?
            if (shipSpawn.GetComponent<EnemyShip>().hasTarget()) {
                Vector3 v2 = shipSpawn.GetComponent<EnemyShip>().targetQueue[0];
                Vector3 v1 = transform.position;
                float range = 1f;
                if (Mathf.Abs(v1.x - v2.x) < range && Mathf.Abs(v1.z - v2.z) < range) {
                    Debug.Log("Ship should have stayed");
                }
            }
        }
    }
    void shipLeaveInvoker() {
        float time = (.5f + (20 / (1+population))); // ship leaves anywhere between 20 seconds to .5 at 10 population it's every 2.5
        if (population > 0) {
            Invoke("shipLeave", time);
        }
        Invoke("shipLeaveInvoker", time);
    }

    // WHEN SHIPS ENTER STAR
    void OnTriggerEnter(Collider other){
		if (other.gameObject.layer == 9) { // if enemy ship is heading here, store them
            if (other.gameObject.GetComponent<EnemyShip>().hasTarget()) {
                Vector3 v2 = other.gameObject.GetComponent<EnemyShip>().getCurrentTarget();
                Vector3 v1 = transform.position;
                float range = 1f;
                if (Mathf.Abs(v1.x - v2.x) < range && Mathf.Abs(v1.z - v2.z) < range) {
                    EnemyShip shipToStore = other.gameObject.GetComponent<EnemyShip>();
                    //if (this.race < 0) setRace(shipToStore.race); // ships will colonise free stars

                    // change the attitude of race depending on ships attitude toward races
                    shipToStore.updateAnger();
                    
                    other.gameObject.GetComponent<EnemyShip>().reachedTarget();
                    residing.Add(getStoredShip(other.gameObject.GetComponent<EnemyShip>()));
                    population++;
                    Debug.Log("Star has a new visitor");
                    Destroy(other.gameObject);

                    // changes race to race with most population at star
                    if (!isHome) {
                        int[] raceTally = new int[Races.size()];
                        foreach (StoredShip s in residing) {
                            raceTally[s.race]++;
                        }
                        int max = -1;
                        int newRace = -1;
                        for (int i=0; i<raceTally.Length; i++) {
                            if (raceTally[i] > max) {
                                max = raceTally[i];
                                newRace = i;
                            }
                        }
                        setRace(newRace);
                    }
                }
            }
		}
	}

	public void setRace(int newRace){
        if (this.race != newRace) {
            this.race = newRace;
            Color raceColor = Races.get(race).getColor();
            transform.gameObject.GetComponent<Renderer>().material.color = raceColor; // set ship to color of race
            transform.GetComponent<Light>().color = saturate(raceColor / 10f, 2f);
        }
    }
    private Color saturate(Color originalColor, float saturationAmount) {
        float h;
        float s;
        float v;
        Color.RGBToHSV(originalColor, out h, out s, out v);
        s *= saturationAmount;
        return Color.HSVToRGB(h,s,v);
    }

    int shipStartingFuel = 2000;
    int shipStartingHP = 100;
    // IMPORTANT METHOD, fills the home stars with ships at beginning
    public void setHome(bool newIsHome=true){
        if (newIsHome == true) {
            isHome = true;

            int shipsToGenerate;
            if (singleShipMode) shipsToGenerate = 1;
            else shipsToGenerate = (int)Mathf.Pow(2, 1 + Races.get(race).getPop());

            for (int i = 0; i < shipsToGenerate; i++) {
                genShip();
            }
        }
        else {
            isHome = false;
        }
	}

    // creates a StoredShip struct from an enemyShip component
    public StoredShip getStoredShip(EnemyShip ship) {
        List<Vector3> targetQueueClone = new List<Vector3>();
        foreach (Vector3 v in ship.targetQueue) {
            targetQueueClone.Add(new Vector3(v.x, v.y, v.z));
        }
        return new StoredShip(ship.race, ship.fuel, ship.hp, ship.type, targetQueueClone);
        //return new StoredShip(ship.race, ship.fuel, ship.hp, ship.type, new List<Vector3>(ship.targetQueue));
    }

    // Needed for pathfinding with graph
    public void findParents() {
        this.graphParents = gen.starGraph.findParents(this.starNumber); // if graph changed, will need to findParents() again
    }

    public StoredShip findNewTarget(StoredShip storedShip) {
        // if ship has no target
        if (storedShip.targetQueue.Count == 0) {
            // DETERMINE WHERE SHIPS SHOULD GO -------------------------------------------------------------------------
            if (this.starNumber == Races.get(storedShip.race).getHome().GetComponent<Star>().starNumber) { // if home, go here
                // go to random star
                int targetStar = Random.Range(0, gen.stars.Count-1);
                if (targetStar >= this.starNumber) targetStar++; // avoid targeting home star

                // Smart method
                if (Races.get(storedShip.race).getIntellect() + Random.Range(0,7) >= 8) {
                    // find best path to target from here
                    List<int> path = gen.starGraph.pathfind(this.starNumber, targetStar, this.graphParents);
                    // goes through path
                    foreach (int nextStar in path) {
                        // add next star in path to queue of ship
                        storedShip.queueTarget(gen.stars[nextStar].transform.position);
                    }
                }
                // Dumb method
                else {
                    storedShip.queueTarget(gen.stars[targetStar].transform.position); // to go directly to target
                }
            }
            else { // if not at home, go home
                // going home
                int target = Races.get(storedShip.race).getHome().GetComponent<Star>().starNumber;

                // Smart method
                if (Races.get(storedShip.race).getIntellect() + Random.Range(0, 7) >= 8) {
                    List<int> path = gen.starGraph.pathfind(this.starNumber, target, this.graphParents);
                    // goes through path
                    foreach (int nextStar in path) {
                        // add next star in path to queue of ship
                        storedShip.queueTarget(gen.stars[nextStar].transform.position);
                    }
                }
                // Dumb method
                else {
                    storedShip.queueTarget(Races.get(storedShip.race).getHome().transform.position);// to go directly home instead
                }
            }
        }
        return storedShip;
    }
    // generate the star system (currently kinda unused)
	public void genSystem(){
		if (isGenerated == false) {
			isGenerated=true;
			system= new StarSystem();
		}
		system.makeCurrent ();
	}
    public string toString() {
        return "Race: " + this.race + " - " + "Home?: " + this.isHome + " - Occupied: " + this.population+ " - X:" + this.transform.position.x+ " - Z:" + this.transform.position.z + "\n";

    }
}