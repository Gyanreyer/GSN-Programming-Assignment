using UnityEngine;
using System.Collections;

public class lightScript : MonoBehaviour {

    //Enum for state of light, whether off, half-lit, or fully on
    enum lightState
    {
        off = 0,
        half = 1,
        on = 2
    }

    //Sprite renderer for this light
    private SpriteRenderer sprRenderer;

    //Parent object which is used for animation
    private GameObject parentObj;

    //Game manager
    private gameManager GM;

    //Sprites to switch between for states
    public Sprite offSprite;
    public Sprite halfSprite;
    public Sprite onSprite;

    public lightCoords coords;//Coordinates of this light on grid    

    private lightState state;//State that light is in

    private bool highlighted;//True when bulb is highlighted

    public bool Highlighted
    {
        get { return highlighted; }
        set { highlighted = value; }
    }

	// Use this for initialization
	void Start () {
        state = lightState.off;

        sprRenderer = GetComponent<SpriteRenderer>();

        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<gameManager>();
	}
	
	// Update is called once per frame
	void Update () {
        //Set color to reflect whether highlighted or not
        if (highlighted)     
            sprRenderer.color = Color.white;
        else
            sprRenderer.color = new Color(0.85f, 0.85f, 0.85f);

        transform.parent.gameObject.GetComponent<Animator>().SetBool("Highlighted", highlighted);//Play pop animation if highlighted

        highlighted = false;//Reset highlighted so light won't be left highlighted when shouldn't
	}

    //Switch state of light
    public void switchLight()
    {
        //Cycle through states using index values of lightState enum
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

    //If user clicks on this lightbulb's collider, call LightSelected in game manager to either set start point or switch the appropriate row of lights
    void OnMouseDown()
    {
        GM.LightUpdate(coords, true);
        
             
    }

    //If user mouses over this lightbulb's collider, alter color slightly and call LightHovered to highlight all possible lightbulbs in selection
    void OnMouseOver()
    {
        if (GM.SelectionStarted)
            GM.LightUpdate(coords, false);
        else
            highlighted = true;
    }

    void OnMouseExit()
    {
        highlighted = false;
    }


}
