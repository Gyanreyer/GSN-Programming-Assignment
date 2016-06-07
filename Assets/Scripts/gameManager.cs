using UnityEngine;
using System.Collections;


//Struct holds ints for x and y coords of lights
public struct gridCoords
{
    //x and y coordinates on grid
    public int x, y;

    //Constructor takes x and y coordinates and stores them
    public gridCoords(int xPos, int yPos)
    {
        x = xPos;
        y = yPos;
    }
}

//GameManager class handles spawning and managing grid of lights
public class gameManager : MonoBehaviour {

    //Prefab for lights
    public GameObject lightPrefab;

    //2D array of all lights
    private GameObject[,] lightGrid;

    //Dimensions of light grid
    public int gridWidth = 10;
    public int gridHeight = 10;

    //Selection stuff for lights
    private gridCoords startSelection;//Coordinates of first light bulb clicked, when a second is clicked lights will change state within the rectangle formed by the two coordinates

    private bool firstSelected;//Whether the user has selected the first light

    //Property to return whether first selected
    public bool FirstSelected { get { return firstSelected; } }


	// Use this for initialization
	void Start ()
    {
        //Get child of light prefab because it contains the sprites and visual aspects, the parent object exists so that position animations can be played that won't mess up the child
        GameObject lightChild = lightPrefab.transform.GetChild(0).gameObject;


        //Get dimensions of orthographic camera in terms of world units in order to scale lights so they fit in the view
        float worldScreenHeight = Camera.main.orthographicSize * 2f;//orthographicSize = half of camera height
        Vector3 worldScreenSize = new Vector3(worldScreenHeight * Screen.width / Screen.height, worldScreenHeight, 0);

        lightChild.transform.localScale = new Vector3(1,1,1);//Make sure light's scale is 1,1,1 so that its bounds can be used to get adjusted scale

        Vector3 lightBounds = lightChild.GetComponent<SpriteRenderer>().bounds.size;//Bounds in world units for light


        //The light grid is going to be square so we'll only adjust to fit within the screen height and use it for both width and height
        float adjustedLightDimension = worldScreenSize.y / (lightBounds.y * gridHeight);//Divide world height of screen by world height of light to get local height
                                                                                        //Also divide by height of grid so that size is proportioned to size of one spot on grid
                                                                                                                     
        lightChild.transform.localScale = new Vector3(adjustedLightDimension, adjustedLightDimension, 1);//Set light scale to adjusted value

        lightBounds = lightChild.GetComponent<SpriteRenderer>().bounds.size;//Update bounds for new adjusted size, we'll use this for spawn locations

        //Initialize light grid to specified width and height
        lightGrid = new GameObject[gridWidth, gridHeight];

        Vector3 spawnStartPos = new Vector3(-worldScreenHeight/2, -worldScreenHeight/2,0);//Start position for spawning

        //Loop through all elements of lightMatrix and spawn a light prefab
        for(int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                lightPrefab.transform.GetChild(0).position = spawnStartPos + new Vector3(x * lightBounds.x, y * lightBounds.y, 0);//Set position for child before instantiating
                lightGrid[x, y] = (GameObject)Instantiate(lightPrefab);
                lightGrid[x, y].transform.GetChild(0).GetComponent<lightScript>().coords = new gridCoords(x,y);//Set grid coordinates for new light so that it can keep track of itself in the light grid
            }
        }
	}

    void Update()
    {
        //If running as standalone application, press escape to quit
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    //Update lights if one is selected or hovered over
    public void LightUpdate(gridCoords coords, bool clicked)//clicked is true if the light was clicked on, false if just hovered
    {
        //If selection of lights has not been started yet, store coords as initial selection and set selection state to started
        if(!firstSelected)
        {
            startSelection = coords;
            firstSelected = true;
        }
        //Otherwise, highlight or switch appropriate lights
        else
        {
            firstSelected = !clicked;//If second selection was made, reset selected state, otherwise this will stay true

            //Direction that x and y will need to be incremented by in loop, will be -1, 0, or 1 depending on where end is in relation to start
            int yDirection = coords.y - startSelection.y;
            if(yDirection != 0)
                yDirection /= Mathf.Abs(yDirection);

            int xDirection = coords.x - startSelection.x;
            if(xDirection != 0)
                xDirection /= Mathf.Abs(xDirection);

            //Counter variables for loop
            int y = startSelection.y;
            int x;

            GameObject currentLight;//Will represent the current object being worked with each loop

            //Using do while so that in instances where the x or y of start and end are equal it'll still loop at least once
            //Outer loop goes through y coords
            do
            {
                x = startSelection.x;//Reset x counter

                //Inner loop goes through x coords
                do
                {
                    currentLight = lightGrid[x, y].transform.GetChild(0).gameObject;//Set current light to GO at current x and y

                    //If user clicked, switch current light's state
                    if (clicked)
                        currentLight.GetComponent<lightScript>().switchLight();
                    //Otherwise, just highlight this light
                    else
                        currentLight.GetComponent<lightScript>().Highlighted = true;

                    x += xDirection;//Increment x counter by direction
                } while (x != coords.x + xDirection);//End loop if reached end of x selection

                y += yDirection;//Increment y counter by direction
            } while (y != coords.y + yDirection);//End loop if reached end of y selection

        }
    }

}
