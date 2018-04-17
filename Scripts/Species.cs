/* Jonathan Burtson
 * 4/17/18
 * Species (this class) stores all info about an alien race in a Species object
 */
using UnityEngine;
using System.Collections;
using System.Text;
using System;

public class Species : IComparable<Species>{
	private string raceName;
    //private char[] language = new char[26]; // cipher to simulate alien language
    private String language;
	private int aggro; // aggressiveness of species out of 10
	private int intellect; // intelligence of species out of 10
	private int pop; // aggressiveness of species out of 10
	private double importance; // importance of species out of 10 (average of aggro,intellect,pop)
    private int size; // Physical size of race out of 10
	private GameObject homeStar;
	private Color color;
    private float[] angerAtRace;
    private float angerAtPlayer;
    private int totalPopulation;

    private bool isKanji = true;

	public Species(){ // Constructor
        if (isKanji) this.raceName = nameGenKanji(); // for name using kanji sounds
        else this.raceName = nameGen(); // for name using my custom vowel/consanant method
        //this.language = newLanguage();
        this.language = newLanguage2();
        //this.language = newLanguageShifted();
        this.aggro = UnityEngine.Random.Range(0,11);
		this.intellect = UnityEngine.Random.Range(0,11);
        //this.pop = UnityEngine.Random.Range(0,11);
        this.pop = UnityEngine.Random.Range(0, 6) + UnityEngine.Random.Range(0, 6); // now most likely to get an average population
        this.importance = (aggro + intellect + pop) / 3.0;
        this.size = UnityEngine.Random.Range(0, 11);
        this.color = new Color (UnityEngine.Random.value,UnityEngine.Random.value,UnityEngine.Random.value);
        this.angerAtPlayer = 0f;
        this.totalPopulation = 0;
    }
	public Species(bool isHuman){ // Overridied constructor, will make human if parameter is bool
		this.raceName = "human";
        //this.language = new char[26]{'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'};
        this.language = "abcdefghijklmnopqrstuvwxyz";
        this.aggro = 5;
		this.intellect = 5;
		this.pop = 5;
		this.importance = 5;
        this.size = 5;
		this.color = new Color (0f,0f,1f);
        this.angerAtPlayer = 0f;
        this.totalPopulation = 0;
    }
	// Buisness Methods
	public string toString (){
        return "Race: <color=#" + getColorHex() + ">" + this.raceName + "</color> \nAgressiveness: "+this.aggro+"/10\n"+"Intelligence: "+this.intellect+"/10\n"+"Population: "+this.pop+"/10\n"+"Importance: "+this.importance+"/10\n" + "Total Population: " + this.totalPopulation + "\n" + "Size: " + this.size + "/10\n";

    }
	public int CompareTo(Species that){ // compare two species for importance
		return (int)(this.importance-that.importance);
	}
	public string translate(string text){ // use method to translate text into alien "language"
		string textTrans = "";
		for (int i=0; i<text.Length; i++){
			char c = text[i];
			if (c>=97 && c<=122){
				textTrans+=language[c-97];
			}
			else if (c>=65 && c<=90){
				textTrans+=language[c-65];
			}
			else{
				textTrans+=c;
			}
		}
		return textTrans;
	}
    public void initAnger(int raceNum) {
        this.angerAtRace = new float[raceNum];
    }
	// Getters/Setters
	public string getName(){ // returns the species name
		return this.raceName;
	}
	public int getAggro(){ 
		return this.aggro;
	}
	public int getIntellect(){
		return this.intellect;
	}
	public int getPop(){
		return this.pop;
	}
    public int getSize()
    {
        return this.size;
    }
	public GameObject getHome(){
		return homeStar;
	}
	public Color getColor(){
		return this.color;
	}
    public string getColorHex() {
        return ColorUtility.ToHtmlStringRGBA(this.color);
    }
    public float[] getAngerAtRaceArray() {
        return (float[])angerAtRace.Clone();
    }
    public float getAngerAtRace(int raceIndex) {
        return angerAtRace[raceIndex];
    }
    public float getAngerAtPlayer() {
        return angerAtPlayer;
    }
    public int getTotalPopulation() {
        return totalPopulation;
    }
    public void setHome(GameObject home){
		this.homeStar=home;
        home.GetComponent<Star>().setRace (Races.getIndex(this));
		home.GetComponent<Star>().setHome();
	}
    public void setAngerAtRace(float anger, int raceIndex) {
        angerAtRace[raceIndex] = anger;
    }
    public void setAngerAtPlayer(float anger) {
        angerAtPlayer = anger;
    }
    public int addTotalPopulation() {
        totalPopulation++;
        return totalPopulation;
    }
    public int subtractTotalPopulation() {
        totalPopulation--;
        return totalPopulation;
    }
    /* Old Language generator which used char[]
	private char[] newLanguage(){ // generate the alien's personal alphabet
        char[] tempABC = new char[26]{'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'};
        int removeIndex;
		for (int i=0; i<tempABC.Length; i++){
			removeIndex=UnityEngine.Random.Range(0, tempABC.Length - i);
			language[i]=tempABC[removeIndex];
			for (int j=removeIndex+1; j<26-i; j++){ // shift over alphabet
				tempABC[j-1]=tempABC[j];
			}
		}
		return tempABC;
	}
    */
    private String newLanguage2() { // generate the alien's personal alphabet
        StringBuilder tempABC = new StringBuilder();
        tempABC.Append("abcdefghijklmnopqrstuvwxyz");
        StringBuilder tempLanguage = new StringBuilder();
        int removeIndex;
        while(tempABC.Length>0)
        {
            removeIndex = UnityEngine.Random.Range(0, tempABC.Length);
            tempLanguage.Append(tempABC[removeIndex]);
            tempABC.Remove(removeIndex, 1);
        }
        return tempLanguage.ToString();
    }
    private String newLanguageShifted()
    { // generate the alien's personal alphabet using a shifted index
        char[] tempABC = new char[26] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        StringBuilder tempLanguage = new StringBuilder();
        int startIndex= UnityEngine.Random.Range(0, tempABC.Length - 1);
        for(int i=0; i<26; i++)
        {
            tempLanguage.Append(tempABC[(i+startIndex)%26]);
        }
        return tempLanguage.ToString();
    }
    private string nameGen(){ // generate the alien's name
		StringBuilder newWord = new StringBuilder();
		int length = UnityEngine.Random.Range(1,4);
		string[] vowels = new string[8]{"A","ah","uh","E","eh","I","oo","O"};
		string[] consanants = new string[9]{"k","m","th","f","t","k","n","l","sh"};
		int type;
		if (length == 1) { // make sure if one vowel, it's not just the vowel
			type = UnityEngine.Random.Range (0, 3); // decide placement of consanants
			if (type == 0 || type == 2) {
				newWord.Append (consanants [UnityEngine.Random.Range (0, consanants.Length)]);
			}
			newWord.Append (vowels [UnityEngine.Random.Range (0, vowels.Length)]);
			if (type == 1 || type == 2) {
				newWord.Append (consanants [UnityEngine.Random.Range (0, consanants.Length)]);
			}
		} else {
			for (int i=0; i<length; i++) {
				type = UnityEngine.Random.Range (0, 4); // decide placement of consanants
				if (type == 1 || type == 3) {
					newWord.Append (consanants [UnityEngine.Random.Range (0, consanants.Length)]);
				}
				newWord.Append (vowels [UnityEngine.Random.Range (0, vowels.Length)]);
				if (type == 2 || type == 3) {
					newWord.Append (consanants [UnityEngine.Random.Range (0, consanants.Length)]);
				}
			}
		}
		return newWord.ToString();
	}
    private string nameGenKanji(){
        StringBuilder newWord = new StringBuilder();
        //int length = UnityEngine.Random.Range(1, 4);
        int length = UnityEngine.Random.Range(3, 6);
        string[] sounds = {"uh-","ee","oo","eh-","oh","ha","hi","fu","he","ho","ka","ki","coo","ke","ko","ma","mi","mu","me","mo","sa","shi","su","seh-","so","ya","yu","yo","ta","chi","tsu","teh-","toh-","ra","ree","roo","reh-","roh-","kna","kni","noo","neh-","noh","wah-","wee","n","weh-","wo"};

        for (int i = 0; i < length; i++)
        {
            newWord.Append(sounds[UnityEngine.Random.Range(0, sounds.Length)]);
        }
        //if (newWord.charAt(newWord.length() - 1) == '-')
        ///{ // remove any last dashes
        //    newWord.Append(newWord.substring(0, newWord.length() - 1));
        //}
        return newWord.ToString();
    }
}
