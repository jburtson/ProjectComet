/* Jonathan Burtson
 * 4/17/2018
 * 
 */
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StellarGen : MonoBehaviour {
	private GameObject ship;
	public GameObject planetFab;
	public int numOfPlanets;
	public int[] planetDist;


	// Use this for initialization
	void Start () {
		ship = GameObject.Find ("Ship");
		numOfPlanets = StarSystem.current.numOfPlanets;
		planetDist = StarSystem.current.planetDist;

		for (int i=0; i<numOfPlanets; i++) {
			GameObject planetSpawn = ((GameObject)Instantiate (planetFab, new Vector3 (0, 0, 0), Quaternion.identity)).transform.Find("Planet").gameObject;
			int scale = StarSystem.current.planets[i].size; // get size of planet
			planetSpawn.transform.localScale = new Vector3 (scale, scale, scale);
			planetSpawn.transform.Translate(Vector3.forward*StarSystem.current.planetDist[i]*5,planetSpawn.transform);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Mathf.Abs (ship.transform.position.x) > 300 || Mathf.Abs (ship.transform.position.z) > 300) {
            SceneManager.LoadScene(1); // go to saved universe scene
        }
	}
}
