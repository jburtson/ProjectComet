/* Jonathan Burtson
 * 4/17/2018
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScale : MonoBehaviour {
    public float timeScale;
	// Use this for initialization
	void Start () {
        Time.timeScale = timeScale;
    }
}
