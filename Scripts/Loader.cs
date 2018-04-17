/* Jonathan Burtson
 * 4/17/2018
 * Saves the universe when originally opening up
 */

using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {
	bool done = false;
	// Use this for initialization
	void Start () {
		if (done == false) {
			done = true;
            //string[] path = EditorApplication.currentScene.Split (char.Parse ("/"));
            //path [path.Length - 1] = "AutoSave_" + path [path.Length - 1];	
            //EditorApplication.SaveScene (string.Join ("/", path), true);
            //Debug.Log("Saved Scene");
			Debug.Log ("Saved Scene (or would have if it wasn't commented out in loader)");
		}
		Destroy (GameObject.Find ("Loader"));
	}
}