/* Jonathan Burtson
 * 4/17/2018
 * Generic script for changing UI text
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScannerText : MonoBehaviour
{
    private Text text;
    private Ship player;

    // Use this for initialization
    void Awake()
    {
        text = GetComponent<Text>();
        player = GameObject.Find("Ship").GetComponent<Ship>();
        text.text = "";
    }
    private void Update()
    {
        text.text = player.getScanner();
    }
}