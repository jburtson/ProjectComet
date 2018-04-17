/* Jonathan Burtson
 * 4/17/18
 * Item (this class) is a public class that that defines items which can be stored as cargo and bought/sold
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Item
{
    public int value;
    public string name;

    public Item()
    {
        value = 0;
        name = "rubbish";
    }
    public Item(int value, string name)
    {
        this.value = value;
        this.name = name;
    }
    override public string ToString()
    {
        return name+" ---- "+value+" credits";
    }
}
