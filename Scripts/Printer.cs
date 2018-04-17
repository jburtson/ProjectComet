/* Jonathan Burtson
 * 4/17/2018
 * Generic script for changing UI text
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Printer : MonoBehaviour
{
    private Text text;
    public string read;

    // Use this for initialization
    public void print(string str)
    {
        text.text = str;
    }
    void Awake()
    {
        text = GetComponent<Text>();
        text.text = "";
    }
}