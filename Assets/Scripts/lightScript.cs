using UnityEngine;
using System.Collections;

public class lightScript : MonoBehaviour {

    //Enum for state of light, whether off, half-lit, or fully on
    enum lightState
    {
        off,
        half,
        on
    }

    //Sprite renderer for this light
    private SpriteRenderer sprRenderer;

    //Animator on parent object used for animations
    private Animator parentAnimator;

    //Game manager
    private gameManager GM;

    //Sprites to switch between for states
    public Sprite offSprite;
    public Sprite halfSprite;
    public Sprite onSprite;

    public gridCoords coords;//Coordinates of this light on grid    

    private lightState state;//State that light is in

    private bool highlighted;//True when bulb is highlighted

    //Property for getting/setting highlighted
    public bool Highlighted
    {
        get { return highlighted; }
        set { highlighted = value; }
    }

	// Use this for initialization
	void Start () {
        state = lightState.off;

        sprRenderer = GetComponent<SpriteRenderer>();
        parentAnimator = transform.parent.gameObject.GetComponent<Animator>();

        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<gameManager>();        
	}
	
	// Update is called once per frame
	void Update () {
        //Set color to reflect whether highlighted or not
        if (highlighted)     
            sprRenderer.color = Color.white;//When highlighted color lights it up brighter
        else
            sprRenderer.color = new Color(0.85f, 0.85f, 0.85f);//When not highlighted color dims sprite

        parentAnimator.SetBool("Highlighted", highlighted);//Play pop animation if highlighted, will switch state from Idle to PopLight and then back to Idle if highlighed is false again

        highlighted = false;//Reset highlighted so light won't be left highlighted when it shouldn't, if it should it'll get set back to true next frame
	}

    //Switch state of light
    public void switchLight()
    {
        //Cycle through states using int values of lightState enum, much more compact than having to deal with switch or if statements
        state = (lightState)(((int)state + 1) % 3);


        //Change sprite based on current state
        switch(state)
        {
            case lightState.half:
                sprRenderer.sprite = halfSprite;
                break;

            case lightState.on:
                sprRenderer.sprite = onSprite;
                break;

            default:
                sprRenderer.sprite = offSprite;
                break;
        }

    }

    //If user clicks on this lightbulb's collider, call LightUpdate with clicked being true in game manager to either set start point or switch the appropriate lights
    void OnMouseDown()
    {
        GM.LightUpdate(coords, true);
    }

    //If user mouses over this lightbulb's collider
    void OnMouseOver()
    {
        //If first has been selected, update lights to highlight the rectangle between start point and this light to give a visual representation of what will change if user clicks
        if (GM.FirstSelected)
            GM.LightUpdate(coords, false);//Call LightUpdate with clicked being false since only hovering right now
        else
            highlighted = true;//If first hasn't been selected, just highlight this one
    }

    //If user's mouse leaves this light's collider, set highlighted back to false
    void OnMouseExit()
    {
        highlighted = false;
    }


}
