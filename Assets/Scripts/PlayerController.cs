using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    private bool dragging = false;
    private Tile startingTile;
    private Tile finishTile;

    private GameMode gameMode;
	private int m_numberOfUnitsToDisplace = 0;

    // Use this for initialization
    void Start()
    {
        gameMode = GameObject.FindGameObjectWithTag("GameMode").GetComponent<GameMode>();

    }

    // Update is called once per frame
    void Update()
    {

		if (Input.GetButton("RageQuit"))
		{
			Application.LoadLevel(0);
		}

        //If the button is pressed, we initiate the drag and drop
		if (Input.GetButtonDown("Select unit"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Tile")
                {
                    startingTile = hit.collider.gameObject.GetComponent<Tile>();

					if (startingTile.getPlayer() != gameMode.getCurrentPlayer())
						return;

                    if (!startingTile.isMoved() && startingTile.getUnitNumbers() > 0)
                    {
                        gameMode.displayBorders(startingTile.gameObject);

						if (Input.GetButton("SplitUnits"))
						{
							m_numberOfUnitsToDisplace = 1;
							Debug.Log ("Shitf used");
						}
						else
						{
							m_numberOfUnitsToDisplace = startingTile.getUnitNumbers();
						}
                        dragging = true;
                    }
                }
            }
        }
		//If we split one of our troops from our army
        //If we release the button while dragging, we drop the units on the tile
        else if (dragging && Input.GetButtonUp("Select unit"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Tile")
                {
                    finishTile = hit.collider.gameObject.GetComponent<Tile>();
					gameMode.displacement(startingTile.gameObject, finishTile.gameObject, m_numberOfUnitsToDisplace);
					m_numberOfUnitsToDisplace = 0;
                }
            }

            dragging = false;
        }
    }
}