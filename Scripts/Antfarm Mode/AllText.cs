/* Jonathan Burtson
 * 4/17/2018
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class AllText : MonoBehaviour {
    private Text text;
    private Generator gen;
    // Use this for initialization
    void Awake() {
        text = GetComponent<Text>();
        gen = GameObject.Find("Generator").GetComponent<Generator>();
        text.text = "";
    }
    private void Update() {
        StringBuilder toPrint = new StringBuilder();
        //toPrint.Append("HP: " + (int)player.hp + "\n");
        for (int i=0; i<Races.size(); i++) {
            //string colorHex = ColorUtility.ToHtmlStringRGBA(Races.get(i).getColor());
            //toPrint.Append("<color=#" + colorHex + ">" + Races.get(i).toString() + "</color>\n"); // All info
            toPrint.Append(Races.get(i).toString() + "\n"); // All info
        }
        for (int i = 0; i < gen.stars.Count; i++) {
            Star thisStar = gen.stars[i].GetComponent<Star>();
            if (thisStar.race >= 0) {
                toPrint.Append("<color=#" + Races.get(thisStar.race).getColorHex() + ">" + thisStar.toString() + "</color>\n"); // All info
            }
            else toPrint.Append(thisStar.toString() + "\n");
        }
        text.text = toPrint.ToString();
    }
}
