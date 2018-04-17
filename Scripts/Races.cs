/* Jonathan Burtson
 * 4/17/18
 * Races (this class) is a public class that stores every race in the universe
 *  TODO: 
 *      *   Keep track of galaxy-wide population of every race. 
 *      *   If reaches 0, make home star no longer a home star. 
 *      *   But it should also be harder to eradicate a race as it's population approaches 0 (Maybe home stars are strong? create more ships?)
 *      *   Also need to make intelligent species stronger
 *      *   And make the pathfinding adapt over time to danger, per race
 *      *   Make more dynamic ship combat. Chance of fleeing, and maybe some kind of manuvering for example
 *      *   Fix time between ship generation, so it's actually an amount of time until next generation
 *      *   Progression for player
 *      *   Player interacting with stars
 *      *   Maybe have technologies, essentially booleans which cause small bonuses. Higher intelligence races have more than others.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Races{
	private static List<Species> races;

    public static void gen(int minRaces, int maxRaces){ // generate all the species
		races = new List<Species>();
		for (int i=0; i<Random.Range (minRaces, maxRaces); i++){
			races.Add(new Species());
		}
		races.Add (new Species (true));// ADD HUMANS TO THE MIX (COMMENT THIS LINE OUT IF YOU WANT)
		races.Sort ();
        foreach (Species s in races) {
            s.initAnger(races.Count);
        }
	}
	public static int size(){
		return races.Count;
	}
	public static Species get(int index){
		if (index < races.Count) {
			return races[index];
		}
		return races[races.Count-1];
	}
	public static int getIndex(Species species){
		return races.IndexOf(species);
	}
}
