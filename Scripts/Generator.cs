/* Jonathan Burtson
 * 4/17/18
 * Generator (this class) creates the stars, and also any ships that need generating
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Programming.WeightedUndirected;

public class Generator : MonoBehaviour {
	public GameObject star;
	public GameObject ship0; // default alien ship prefab
	public GameObject ship1; // alien war ship prefab
    public GameObject ship2; // alien colony ship prefab
    public GameObject boom; // explosion gameobject
    public List<GameObject> stars= new List<GameObject>(); // all stars in the universe
	public int spawnRange=500;
    public int minRandomStars;
    public int maxRandomStars;
    public int minStarSize = 2;
    public int maxStarSize = 5;
    public int minRaces = 5;
    public int maxRaces = 20;

    public bool starGridGeneration = false;

    public int healAmount = 10;
    public int refuelAmount = 1000;

    public float fuelGraphWeightCost;
    public EdgeWeightedGraph starGraph;

    // What's done immediately
    void Start () {
		GameObject starSpawn;
		Races.gen (minRaces,maxRaces);
		for (int i=0; i<Races.size(); i++) { // make all the homestars
			starSpawn = createStarRandom (minStarSize, maxStarSize);
            Races.get (i).setHome (starSpawn);
        }
        //testShip (Races.size () - 1);// testing spawnShip
        if (starGridGeneration) {
            createStarsGrid(minStarSize / 2, maxStarSize / 2, Random.Range(minRandomStars, maxRandomStars));
        }
        else {
            for (int i = 0; i < Random.Range(minRandomStars, maxRandomStars); i++) { // spawn random stars
                createStarRandom(minStarSize / 2, maxStarSize / 2);
            }
        }
        // STAR GRAPH THING
        fuelGraphWeightCost = 2.5f * refuelAmount; // 2.5 is roughly the amount of distance per fuel, multiplied by amount refueled at each star
        starGraph = new EdgeWeightedGraph(stars.Count);
        Edge tempEdge;
        float weight;
        for (int i=0; i<stars.Count; i++){
            for (int j=i+1; j<stars.Count; j++){
                weight = Vector3.Distance(stars[i].transform.position, stars[j].transform.position);
                weight -= fuelGraphWeightCost; // ACCOUNT FOR FUEL -------------------------------
                tempEdge = new Edge(i,j,weight);
                starGraph.AddEdge(tempEdge);
            }
        }
        // find all parents. This is nessesary to do pathfind() from the star
        foreach (GameObject s in this.stars) {
            s.GetComponent<Star>().findParents();
        }
	}

	GameObject createStar (int minSize, int maxSize, float x, float z){ // create a star
		GameObject starSpawn = (GameObject)Instantiate(star, new Vector3 (x, 0, z), Quaternion.identity);
		int scale = Random.Range(minSize,maxSize);
		starSpawn.transform.localScale = new Vector3(scale,scale,scale);
        starSpawn.GetComponent<Light>().range = scale*2;
        starSpawn.GetComponent<Star>().starNumber = stars.Count; // label star
        stars.Add(starSpawn);
        return starSpawn;
	}
    // generate a single star randomly
    GameObject createStarRandom(int minSize, int maxSize) {
        return createStar(minSize, maxSize, Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange, spawnRange));
    }
    // alternate way of generating stars; in a grid
    void createStarsGrid(int minSize, int maxSize, int starsCount) {
        float squareRoot = Mathf.Sqrt((float)starsCount);
        int rowSize = (int)squareRoot; // truncate to nearest square root
        if (rowSize - squareRoot != 0f) rowSize++; // round up
        float realX, realZ;
        for (int x=0; x<rowSize && starsCount > 0; x++) {
            for (int z = 0; z < rowSize && starsCount > 0; z++) {
                realX = (x - ((rowSize - 1) / 2f)) * (spawnRange/ ((rowSize - 1) / 2f));
                realZ = (z - ((rowSize - 1) / 2f)) * (spawnRange / ((rowSize - 1) / 2f));
                createStar(minSize,maxSize,realX,realZ);
                starsCount--;
            }
        }
    }
    // METHOD TO SPAWN A NEW SHIP AT A LOCATION
    GameObject createShip(int race, int shipType, float x, float z, float fuel=10000f, float hp=100){
        GameObject shipSpawn;
        Species shipRace = Races.get(race);
        //float size = ((float)shipRace.getSize() + 1) / 5; // ship size between .2 and 2.2
        float size = ((float)shipRace.getSize()+shipRace.getIntellect() + 1) / 8; // ship size between .1 and 2.1 using intelligence and size
        switch (shipType) {
            case 2:
                // create the ship game object
                shipSpawn = (GameObject)Instantiate(ship2, new Vector3(x, 0, z), Quaternion.identity);
                shipSpawn.transform.localScale = new Vector3(size * 3f, size * 1f, size * 3f); // set ship to size of race
                break;
            case 1:
                // create the ship game object
                shipSpawn = (GameObject)Instantiate(ship1, new Vector3(x, 0, z), Quaternion.identity);
                shipSpawn.transform.localScale = new Vector3(size * 2f, size * 2f, size * 2f); // set ship to size of race
                break;
            case 0:
            default:
                // create the ship game object
                shipSpawn = (GameObject)Instantiate(ship0, new Vector3(x, 0, z), Quaternion.identity);
                shipSpawn.transform.localScale = new Vector3(size, size, size * 2); // set ship to size of race
                break;
        }
        shipSpawn.GetComponent<EnemyShip>().race = race;
        //shipSpawn.GetComponent<EnemyShip>().speed = 2 * (shipRace.getIntellect() + 1); // set speed according to intelligence
        shipSpawn.GetComponent<EnemyShip>().speed = 10 + (shipRace.getIntellect()/2);
        shipSpawn.GetComponent<Renderer>().material.color = shipRace.getColor(); // set ship to color of race
        shipSpawn.GetComponent<EnemyShip>().fuel = fuel;
        shipSpawn.GetComponent<EnemyShip>().hp = hp;
        return shipSpawn;
    }
    GameObject createShip(StoredShip storedShip, float x, float z) {
        GameObject shipSpawn = createShip(storedShip.race, storedShip.type, x, z, storedShip.fuel, storedShip.hp);
        List<Vector3> targetQueueClone = new List<Vector3>();
        foreach (Vector3 v in storedShip.targetQueue) {
            targetQueueClone.Add(new Vector3(v.x,v.y,v.z));
        }
        shipSpawn.GetComponent<EnemyShip>().targetQueue = targetQueueClone;
        return shipSpawn;
    }

    GameObject testShip (int race) { // spawn a single ship near the center, only really for testing
		GameObject shipSpawn = createShip(race,0,4,-4);
        shipSpawn.GetComponent<EnemyShip>().flyHome();
        return shipSpawn;
	}

	public GameObject newShipStarToStar(int fromStar, StoredShip storedShip){ // creates a ship at the given star of given race to travel to a random star
        GameObject shipSpawn;
        // adding an offset where ships are spawned randomly around the same area of the star, to prevent crouding
        float spawnRange = 5f;
        float randomX= Random.Range(-spawnRange,spawnRange);
        float randomZ= Random.Range(-spawnRange,spawnRange);

        // set target for ship if none
        storedShip = stars[fromStar].GetComponent<Star>().findNewTarget(storedShip);

        // create ship
        shipSpawn = createShip(storedShip, stars[fromStar].transform.position.x + randomX, stars[fromStar].transform.position.z + randomZ);
        //shipSpawn = createShip(storedShip.race, storedShip.type, stars[fromStar].transform.position.x + randomX, stars[fromStar].transform.position.z + randomZ, storedShip.fuel, storedShip.hp);
        EnemyShip shipComponent = shipSpawn.GetComponent<EnemyShip>();

        shipComponent.hp += healAmount; // heals up at star
        if (shipComponent.hp > 100) shipComponent.hp = 100; // limits hp to 100

        shipComponent.fuel += refuelAmount; // fuels up at star

        shipComponent.state = 1; // makes sure ship is pursueing target

        // if next target is too far for remaining fuel, stay in this star to fuel up
        if (Vector3.Distance(stars[fromStar].transform.position,shipComponent.targetQueue[0]) * 2.5f > shipComponent.fuel) {
            shipComponent.queueTarget(stars[fromStar].transform.position);
        }
        
        // if ship has nowhere to go, send it to random star or home
        /*
        if (shipComponent.targetQueue.Count == 0) {
            if (Random.Range(0, 3) == 0 && stars[fromStar] != Races.get(storedShip.race).getHome()) {
                shipSpawn.GetComponent<EnemyShip>().flyHome();
            }
            else {
                shipSpawn.GetComponent<EnemyShip>().flyTo((GameObject)stars[Random.Range(0, stars.Count)]); // sets the ship to fly to a random star
            }
        }
        */
        
        return shipSpawn;
	}

    public GameObject explode(Vector3 position){
        return (GameObject)Instantiate(boom, position, Quaternion.identity);
    }

    public bool checkExtinction(int race) {
        Species extinctRace = Races.get(race);
        if (extinctRace.getTotalPopulation() <= 0) {
            extinctRace.getHome().GetComponent<Star>().setHome(false);
            Debug.Log("Race " + extinctRace.getName() + " ("+race+") has gone extinct");
            return true;
        }
        else return false;
    }
}