/* Jonathan Burtson
 * 4/17/2018
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
    public AudioClip explosion; // audioclip that plays on creation
    public float growthRate=1.01f; // how fast the explosion radiates outwards
    public int framesToDeath = 120; // how many frames the explosion exists for
    private AudioSource source;
    private int framesAtStart;

    // Use this for initialization
    void Start()
    {
        source.PlayOneShot(explosion, .5f); // plays noise on creation
        framesAtStart = Time.frameCount;
    }

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update () {
        int timePassed = Time.frameCount - framesAtStart;
        if (timePassed % framesToDeath == 0){
            Destroy(this.gameObject);
        }
        this.transform.localScale *= growthRate;
    }
}
