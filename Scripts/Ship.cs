/* Jonathan Burtson
 * 4/17/18
 * Ship (this class) handles the player's ship's behavior
 */
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Ship : MonoBehaviour {
    public List<Item> inventory = new List<Item>();
    public ShipScanner scannerCollider;
	public float speed = 10;
    public float fuel = 0;
    public float hp = 100;

    //shooting mechanics
    public float fireDelta = 0.5F;
    private float nextFire = 0.5F;
    private float myTime = 0.0F;

    private Rigidbody rb;
	public AudioClip scanwav;
    public AudioClip thud;
    public AudioClip fire;
    public GameObject bulletPrefab;
    private AudioSource source;
	private bool isPlaying=false;
    private string scannerText="";

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		source = GetComponent<AudioSource>();
        // find the "sight" collider child object
        scannerCollider = transform.Find("Sight").GetComponent<ShipScanner>();
    }
	
	// Update is called once per frame
	void Update () {
        List<Collider> triggerList = scannerCollider.getTriggerList(); // list of things that collided with "sight", the scanner collider
        if (Input.GetAxis("Horizontal")!=0){ // turning (left/right)
			float h = Input.GetAxis("Horizontal");
			//transform.rigidbody.AddForce(Vector3.right*h*speed);
			rb.AddTorque(Vector3.up * h * Time.deltaTime * speed * .05f);
		}
		if(Input.GetAxis("Vertical")!=0 && fuel>0){ // accelleration (up/down)
            if (fuel > 0) { }
			if (isPlaying==false){ // play thrust sound
				source.Play();
				isPlaying=true;
			}
			float v = Input.GetAxis("Vertical");
			rb.AddRelativeForce(Vector3.back * v * Time.deltaTime * speed);
            fuel -= 0.1f;
		}
		if (isPlaying == true && Input.GetAxis ("Vertical") == 0){
			source.Pause ();
			isPlaying=false;
		}
		if (Input.GetAxis("Brake")>0) { // press shift to slow
			rb.velocity *= .9f;
		}
		if (Input.GetAxis ("Fire2") > 0) { // RMB to scan in front of you
			source.PlayOneShot(scanwav,.5f); // play scan noise
			StringBuilder toPrint = new StringBuilder();
			foreach (Collider c in triggerList){
				if (c!=null){
					if (c.gameObject.layer==9){ // enemy ship
                        Species scannedRace = Races.get(c.gameObject.GetComponent<EnemyShip>().race);

                        if (c.gameObject.GetComponent<EnemyShip>().type==0) toPrint.Append("Civilian "); // print type of ship
						else if (c.gameObject.GetComponent<EnemyShip>().type==1) toPrint.Append("War ");
                        else if (c.gameObject.GetComponent<EnemyShip>().type==2) toPrint.Append("Colony ");
                        else toPrint.Append("??? ");
						toPrint.Append("Alien Ship\n");
                        toPrint.Append("HP: " + (int)c.gameObject.GetComponent<EnemyShip>().hp + "\n");
                        toPrint.Append("Fuel: " + (int)c.gameObject.GetComponent<EnemyShip>().fuel + "\n");
                        toPrint.Append("Race: <color=#" + scannedRace.getColorHex() + ">" + scannedRace.getName()+ "</color>\n");
                        //toPrint.Append("" + scannedRace.toString()); // All info
                        toPrint.Append("Incoming Transmission...\n"+ scannedRace.translate("\"sphinx of black quartz judge my vow\""));
                    }
					else if (c.gameObject.layer==10){ // star
                        if (c.gameObject.GetComponent<Star>().isHome==true){
							toPrint.Append("Home ");
						}
						toPrint.Append("Star\n");
                        toPrint.Append("\n");
                        toPrint.Append("Occupied: "+c.gameObject.GetComponent<Star>().population+'\n');
						if (c.gameObject.GetComponent<Star>().race>=0){
                            Species scannedRace = Races.get(c.gameObject.GetComponent<Star>().race);
                            //toPrint.Append("Race: "+scannedRace.getName());
                            //string colorHex = ColorUtility.ToHtmlStringRGBA(scannedRace.getColor());
                            //toPrint.Append("<color=#"+colorHex+">" + scannedRace.toString() + "</color>"); // All info
                            toPrint.Append(scannedRace.toString()); // All info

                            // print race's anger
                            toPrint.Append("\nYour Infamy: "+ System.Math.Round(scannedRace.getAngerAtPlayer(), 2)); // rounded
                            int rival = c.gameObject.GetComponent<Star>().race;
                            float rivalAmount = 0f;
                            float anger;
                            for (int i=0; i<Races.size(); i++) {
                                anger = scannedRace.getAngerAtRace(i);
                                if (anger > rivalAmount) {
                                    rival = i;
                                    rivalAmount = anger;
                                }
                            }
                            toPrint.Append("\nRival: <color=#" + Races.get(rival).getColorHex() + ">" +Races.get(rival).getName()+ "</color>, at "+ System.Math.Round(rivalAmount,2));
                        }
					}
                    if (c.gameObject.layer == 9 || c.gameObject.layer == 10) toPrint.Append("\n______\n\n");
				}
			}
            scannerText = toPrint.ToString();
		}
        myTime = myTime + Time.deltaTime;
        if (Input.GetButton("Fire1") && myTime > nextFire) { // SHOOT
            nextFire = myTime + fireDelta;
            shoot();
            nextFire = nextFire - myTime;
            myTime = 0.0F;
        }
        //if (Input.GetButton("Fire1")) shoot();
        /*
		if (Input.GetAxis("Enter")>0) { // press enter near a star to enter its stellar system
			foreach (Collider c in triggerList){
				if (c!=null){
					if (c.gameObject.layer==10){ // star
						// SAVING
						//string[] path = EditorApplication.currentScene.Split(char.Parse("/"));
						//path[path.Length -1] = "AutoSave_" + path[path.Length-1];	
						//EditorApplication.SaveScene(string.Join("/",path), true);
						//Debug.Log("Saved Scene");
                        Debug.Log("Saved Scene (or would have if it wasn't commented out in loader)");
                        // DONE SAVING
                        c.gameObject.GetComponent<Star>().genSystem();
                        SceneManager.LoadScene(1);// change to stellar system scene
						break;
					}
				}
			}
		}
        */
    }
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<Rigidbody>()) { // if collided object has rigidbody
            float collisionMagnitude = (rb.velocity - other.gameObject.GetComponent<Rigidbody>().velocity).magnitude;
            // take damage equal to difference in velocities
            hp -= collisionMagnitude;
            source.PlayOneShot(thud, Mathf.Clamp(collisionMagnitude*2, .1f, 10f)); // play thud noise
        }
    }
    public void shoot() {
        source.PlayOneShot(fire, .3f); // play thud noise

        Vector3 playerPos = transform.position;
        Vector3 playerDirection = -1*transform.forward;
        Quaternion playerRotation = transform.rotation;
        float spawnDistance = 2f;
        Vector3 spawnPos = playerPos + playerDirection * spawnDistance;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        bullet.GetComponent<Bullet>().fromRace = -1; // -1 means from player "race"
        bullet.GetComponent<Rigidbody>().velocity=rb.velocity;
        //Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());
        //bullet.transform.LookAt(spawnPos);
        bullet.transform.rotation=Quaternion.LookRotation(playerDirection);
    }
    public string getScanner()
    {
        return this.scannerText;
    }
}
