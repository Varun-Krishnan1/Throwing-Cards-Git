 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;


public class CardWeapon : Weapon
{
    public Transform firePoint;
    public GameObject cardPrefab;
    public GameObject cardInHand;
    public GameObject currentCard; 
    public GameObject player;
    public Light2D weaponLighting;

    private float fireRateTime;          // max fire rate time (set by weapons manager) 
    private float fireRate = 0f;        // current fire rate counter 
    public float offset;
    public Sprite[] cardSprites;

    private Animator animator; 


    Dictionary<string, CardStruct> cardDict = new Dictionary<string, CardStruct>();
    private CardStruct nextCard; 



    // Start is called before the first frame update
    void Start()
    {

        animator = player.GetComponent<Animator>();
        
        createCardDict();
        
        setRandomCard();

        //float shootTime = animator.runtimeAnimatorController.animationClips.First(x => x.name == "CardThrowing").length;
        //fireRateTime = shootTime;
    }

    // Update is called once per frame
    public void Update()
    {
        fireRate -= Time.deltaTime; 
   
    }

    // -- called only once at beggining of game 
    void createCardDict()
    {
        // -- process sprite name 
        foreach(Sprite sprite in cardSprites)
        {
            string name = sprite.name;
            string valueString = "";
            string suit = "";

            int separationIndex = name.IndexOf("Suit"); // -- keyword that value and suit are separated by 

            valueString = name.Substring(0, separationIndex);
            suit = name.Substring(separationIndex); 

            cardDict.Add(name, new CardStruct(valueString, suit, sprite));
        }
        
    }

    // -- called to start shoot animation 
    public override bool startShootAnimation()
    {
        // -- only start shoot animation if fire rate cooldown time is over 
        if (fireRate <= 0)
        {

            animator.SetBool("isShooting", true);
            fireRate = fireRateTime;
            return true;
        }
        return false; 
    }


    // -- called by shoot animation midway through (DO NOT CALL MANUALLY) 
    public override void Shoot()
    {
        GameObject newCard = Instantiate(cardPrefab, firePoint.position, firePoint.rotation);

        // set sprite of card thrown to next card's sprite stored globally 
        newCard.GetComponent<SpriteRenderer>().sprite = nextCard.sprite;            

        // -- set attributes of the card using nextcard struct stored from setrandomcard() 
        newCard.GetComponent<CardController>().value = nextCard.value;
        newCard.GetComponent<CardController>().suit = nextCard.suit;

        setRandomCard(); 
    }

    // -- TO BE USED BY START MENU CHARACTER ONLY! 
    public void ShootStartMenu(GameObject fancyCard)
    {
        Instantiate(fancyCard, firePoint.position, firePoint.rotation);

        // set sprite of card thrown to next card's sprite stored globally 
        fancyCard.GetComponent<SpriteRenderer>().sprite = nextCard.sprite;

        // -- set attributes of the card using nextcard struct stored from setrandomcard() 
        fancyCard.GetComponent<CardController>().value = nextCard.value;
        fancyCard.GetComponent<CardController>().suit = nextCard.suit;

        setRandomCard();
    }

    public override float getFireRateTime()
    {
        return this.fireRateTime; 
    }

    public override void setFireRateTime(float value)
    {
        this.fireRateTime = value; 
    }

    public void setRandomCard()
    {
        // -- get a random number to select sprite from sprite array 
        int randNumber = Random.Range(0, cardSprites.Length - 1);
        Sprite randSprite = cardSprites[randNumber]; 
        string randSpriteName = randSprite.name;

        // -- set card in hand and card over head to next card's sprite 
        cardInHand.GetComponent<SpriteRenderer>().sprite = randSprite;
        currentCard.GetComponent<UnityEngine.UI.Image>().sprite = randSprite;

        // -- store info about next card globally for shoot method to access 
        // -- use the sprite name as a key for the card dictionary to get the card struct 
        nextCard = cardDict[randSpriteName]; 

        // -- lighting 
        if(cardDict[randSpriteName].suit == "Suit1" || cardDict[randSpriteName].suit == "Suit2")
        {
            weaponLighting.color = Color.red; 
        }
        else
        {
            weaponLighting.color = Color.white;
        }
    }

    public override void setWeaponImgObject(bool activeState)
    {
        cardInHand.SetActive(activeState);
        currentCard.SetActive(activeState);

        // -- then set lighting 

    }

}

// -- let's use struct for card object because it's stored on stack not heap so access to it is very fast 
// -- you can't change struct variables once they are passed but since we don't really need to change the base 
// -- attributes of the card we should be fine 

// --Structs are generally a good idea for small data structures that are meant to just hold groups of data. 
// -- You don't need to deal with the overhead of "objects" since the struct stores the actual values, not references to the data.

struct CardStruct
{
    //Variable declaration
    //Note: I'm explicitly declaring them as public, but they are public by default. You can use private if you choose.
    public int value;
    public string suit;
    public Sprite sprite; 

    //Constructor (not necessary, but helpful)
    public CardStruct(string valueString, string suit, Sprite cardSprite)
    {
        this.suit = suit;
        this.value = 0;
        this.sprite = cardSprite; 

        this.value = getValueFromString(valueString);
    }

    private int getValueFromString(string valueString)
    {
        if(valueString == "A")
        {
            return 1; 
        }
        else
        {
            return System.Int32.Parse(valueString);
        }
    }
}