/* Jonathan Burtson
 * 4/17/2018
 * 
 */
using UnityEngine;
using System.Collections;

public class StarSystem {
	public static StarSystem current;
	public Planet[] planets;
	public int[] planetDist;
	public int numOfPlanets;

	public StarSystem(){
		numOfPlanets = Random.Range (1, 10);
		planets = new Planet[numOfPlanets];
		planetDist = new int[numOfPlanets];
		for (int i=0; i<numOfPlanets; i++){
			planets[i]= new Planet();
			planetDist[i]=Random.Range(i*5,(i+1)*5);
		}
	}
	public void makeCurrent(){
		current = this;
	}
}
