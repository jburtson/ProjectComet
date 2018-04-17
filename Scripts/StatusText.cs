/* Jonathan Burtson
 * 4/17/2018
 * Generic script for changing UI text
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class StatusText : MonoBehaviour {
    private Text text;
    private Ship player;

    // Use this for initialization
    void Awake() {
        text = GetComponent<Text>();
        player = GameObject.Find("Ship").GetComponent<Ship>();
        text.text = "";
    }
    private void Update() {
        StringBuilder toPrint = new StringBuilder();
        toPrint.Append("HP: " + (int)player.hp + "\n");
        toPrint.Append("Fuel: " + (int)player.fuel + "\n");
        foreach (Item i in player.inventory) {
            toPrint.Append(i.name + " . . . . . \u00a7" + i.value + "\n");
        }
        text.text = toPrint.ToString();
    }
}