/* Jonathan Burtson
 * 4/17/2018
 * 
 */
using UnityEngine;
using System.Collections;

public class Planet{
	public int size;
	public bool isGas;
	private static int minGas=5;
	private static int maxGas=10;
	private static int minTerra=1;
	private static int maxTerra=3;

	public Planet(){
		if (Random.Range (0, 2) == 0) {
			isGas = true;
			size = Random.Range(minGas,maxGas+1);
		} else {
			isGas = false;
			size = Random.Range(minTerra,maxTerra+1);
		}
	}
}
