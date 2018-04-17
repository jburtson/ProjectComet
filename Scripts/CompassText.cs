/* Jonathan Burtson
 * 4/17/2018
 * Generic script for changing UI text
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CompassText : MonoBehaviour
{
    private Text text;
    private GameObject player;

    // Use this for initialization
    void Awake()
    {
        text = GetComponent<Text>();
        player = GameObject.Find("Ship");
        text.text = "";
    }
    private void Update()
    {
        text.text = (int)player.transform.position.x + " " + (int)player.transform.position.z + "\n" + (int)player.transform.eulerAngles.y;
    }
}