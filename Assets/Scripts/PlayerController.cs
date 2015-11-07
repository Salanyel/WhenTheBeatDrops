using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    private bool dragging = false;
    private Tile startingTile;
    private Tile finishTile;

    private GameMode gameMode;

    // Use this for initialization
    void Start()
    {
        gameMode = GameObject.FindGameObjectWithTag("GameMode").GetComponent<GameMode>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Select unit"))
        {
            Debug.Log("getting input");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Tile")
                {
                    startingTile = hit.collider.gameObject.GetComponent<Tile>();
                    if (startingTile.getUnitNumbers() > 0)
                    {
                        dragging = true;

                    }
                }
            }

        }
        else if (Input.GetButtonUp("Select unit"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Tile")
                {
                    finishTile = hit.collider.gameObject.GetComponent<Tile>();
                }
            }

            dragging = false;
        }
    }
}