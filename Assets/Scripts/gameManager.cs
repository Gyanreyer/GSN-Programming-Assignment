using UnityEngine;
using System.Collections;


//Struct holds ints for x and y coords of lights that have been selected
public struct lightCoords
{
    public int x, y;

    public lightCoords(int xPos, int yPos)
    {
        x = xPos;
        y = yPos;
    }
}


public class gameManager : MonoBehaviour {

    //Prefab for lights
    public GameObject lightPrefab;

    //2D array of all lights
    private GameObject[,] lightGrid;

    //Dimensions of light grid
    public int gridWidth = 10;
    public int gridHeight = 10;


    //Selection stuff for lights
    private lightCoords startSelection;

    private bool selectionStarted;

    //Property to return whether selection started
    public bool SelectionStarted { get { return selectionStarted; } }


	// Use this for initialization
	void Start () {

        GameObject lightChild = lightPrefab.transform.GetChild(0).gameObject;

        //Get dimensions of orthographic camera in terms of world units in order to scale lights so they fit in the view
        float worldScreenHeight = Camera.main.orthographicSize * 2f;//orthographicSize = half of camera height
        Vector3 worldScreenSize = new Vector3(worldScreenHeight * Screen.width / Screen.height, worldScreenHeight, 0);

        lightChild.transform.localScale = new Vector3(1,1,1);//Make sure light's scale is 1,1,1 so that its bounds can be used to get adjusted scale

        Vector3 lightBounds = lightChild.GetComponent<SpriteRenderer>().bounds.size;//Bounds in world units for light

        //The lights are going to be square so we'll only adjust the height and then use the same value for width
        float adjustedLightHeight = worldScreenSize.y / (lightBounds.y * gridHeight);//Divide world height of screen by world height of light to get local height                                                                                                                       

        lightChild.transform.localScale = new Vector3(adjustedLightHeight, adjustedLightHeight, 1);//Set light prefab scale to adjusted value

        lightBounds = lightChild.GetComponent<SpriteRenderer>().bounds.size;//Update bounds for new adjusted size, we'll use this for spawn locations

        //Initialize light grid to specified width and height
        lightGrid = new GameObject[gridWidth, gridHeight];

        Vector3 spawnStartPos = new Vector3(-worldScreenHeight/2, -worldScreenHeight/2,0);

        //Loop through all elements of lightMatrix and spawn a light prefab
        for(int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                lightPrefab.transform.GetChild(0).position = spawnStartPos + new Vector3(x * lightBounds.x, y * lightBounds.y, 0);
                lightGrid[x, y] = (GameObject)Instantiate(lightPrefab);
                lightGrid[x, y].transform.GetChild(0).GetComponent<lightScript>().coords = new lightCoords(x,y);
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


    //Update lights if one is selected or hovered over during selection
    public void LightUpdate(lightCoords coords, bool selected)//selected is true if the light was clicked on, false if just hovered
    {
        //If selection of lights has not been started yet, store coords as initial selection and set selection state to started
        if(!selectionStarted)
        {
            startSelection = coords;
            selectionStarted = true;
        }
        //Otherwise, highlight or switch appropriate lights
        else
        {
            selectionStarted = !selected;//If second selection was made, end selection state, otherwise this will stay true

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

            GameObject currentLight;

            //Using do while so that in instances where the x or y of start and end are equal it'll still loop at least once
            //Outer loop goes through y coords
            do
            {
                x = startSelection.x;//Reset x counter

                //Inner loop through x coords
                do
                {
                    currentLight = lightGrid[x, y].transform.GetChild(0).gameObject;

                    //Switch current light
                    if (selected)
                    {
                        currentLight.GetComponent<lightScript>().switchLight();
                        currentLight.GetComponent<lightScript>().Highlighted = false;
                    }
                    else
                        currentLight.GetComponent<lightScript>().Highlighted = true;

                    x += xDirection;//Increment x counter by direction
                } while (x != coords.x + xDirection);//End loop if reached end of x selection

                y += yDirection;//Increment y counter by direction
            } while (y != coords.y + yDirection);//End loop if reached end of y selection

        }
    }

}
