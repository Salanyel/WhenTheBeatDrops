using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{

    private bool dragging = false;
    private Tile startingTile;
    private Tile finishTile;

    private GameMode gameMode;
	private int m_numberOfUnitsToMove = 0;

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
                if (hit.collider.tag == Tags.m_tile)
                {

                    startingTile = hit.collider.gameObject.GetComponent<Tile>();

					if (startingTile.getPlayer() != gameMode.getCurrentPlayer())
						return;

                    if (!startingTile.isMoved() && startingTile.getUnitNumbers() > 0)
                    {

						startingTile.displayArrow(gameMode.displayBorders(startingTile.gameObject));
						
						if (Input.GetButton("SplitUnits"))
						{
							m_numberOfUnitsToMove = 1;
						}
						else
						{
							m_numberOfUnitsToMove = startingTile.getUnitNumbers();
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
                if (hit.collider.tag == Tags.m_tile)
                {
                    finishTile = hit.collider.gameObject.GetComponent<Tile>();
					gameMode.displacement(startingTile.gameObject, finishTile.gameObject, m_numberOfUnitsToMove);
					m_numberOfUnitsToMove = 0;
                }

				if (hit.collider.tag == Tags.m_arrow)
				{
					gameMode.displacement(startingTile.gameObject, hit.collider.gameObject, m_numberOfUnitsToMove);
					m_numberOfUnitsToMove = 0;
				}
            }

			startingTile.hideArrows();
            dragging = false;
        }
    }
}