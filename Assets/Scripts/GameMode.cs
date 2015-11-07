using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMode : MonoBehaviour {

	public GameObject m_tilePrefab;

	private float m_mapWidth;
	private float m_mapHeight;
	private float m_tilesUnit;

	private bool m_isFinished;
	private int m_beatsNumber;
	private int[] m_players;
	private GAME_STATE m_gameState;

	void Start () {
		if (PlayerPrefs.HasKey(PlayerPreferences.m_init))
		{
			PlayerPreferences.InitThePlayerPrefs();
		}

		setBoard ();
		setPlayers ();

		m_isFinished = false;
		setGameState (GAME_STATE.DisplayCurrentTurn);
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch (m_gameState)
		{
		case GAME_STATE.PhaseBegins :
			phaseBegins();
			break;

		case GAME_STATE.DisplayCurrentTurn :
			displayCurrentTurn();
			break;

		case GAME_STATE.InitCurrentTurn :
			initCurrentTurn();
			break;

		case GAME_STATE.PlayCurrentPlayer :
			playCurrentTurn();
			break;

		case GAME_STATE.EndOfCurrentPlayer :
						
			break;

		case GAME_STATE.EndOfTurn :
			endOfTurn();
			break;

		default:
			break;
		}

		Debug.Log ("Current state : " + m_gameState);
	}

	void phaseBegins()
	{
		if (m_isFinished)
		{
			//TODO Display écran des résultats
		}

		setGameState (GAME_STATE.DisplayCurrentTurn);
	}

	void displayCurrentTurn()
	{
		//TODO : fade sur le numéro du tour
	}

	void initCurrentTurn()
	{
		//TODO : Sort du tableau de players 
	}

	void playCurrentTurn()
	{
		//TODO : Faire jouer le player de l'index X. Afficher "Tour de machin"
		//TODO : faire pop des troupes
	}

	void endOfTurn()
	{
		//TODO : Tester si la fin du tour correspond à un nouveau test de beats. 
		//Si oui, tester qui est le meneur. S'il a gagné set m_isFinisehd.
		//Sinon, lui demander quelle musique il veut jouer.
		//Lancer le tour suivant
	}

	void setBoard()
	{
		m_mapWidth = PlayerPrefs.GetFloat (PlayerPreferences.m_mapWidth);
		m_mapHeight = PlayerPrefs.GetFloat (PlayerPreferences.m_mapHeight);
		m_tilesUnit = PlayerPrefs.GetFloat (PlayerPreferences.m_tilesUnit);

		createTheMap ();
	}

	void setPlayers()
	{
		int numberOfPlayers = PlayerPrefs.GetInt(PlayerPreferences.m_numberOfPlayers);

		m_players = new int[numberOfPlayers];

		for (int i = 0; i < numberOfPlayers; ++i)
		{
			m_players[i] = i + 1;
		}

		shufflePlayers (numberOfPlayers);
	}

	void shufflePlayers(int p_numbers)
	{

		int target;
		int tmp;

		for (int i = 0; i < p_numbers; ++i)
		{
			target = Random.Range(0, p_numbers);
			tmp = m_players[i];
			m_players[i] = m_players[target];
			m_players[target] = tmp;
		}
	}

	void setTheHexagonTMP(GameObject p_tile, float p_y, float p_x)
	{
		int p1x = 0;
		int p1y = 0;
		int p2x = 1;
		int p2y = 1;

		if (p_x == p1x && p_y == p1y)
		{
			Debug.Log ("P1 detected");
			p_tile.GetComponent<Tile>().setPlayer(PLAYERS.Baroque);
		}

		if (p_x == p2x && p_y == p2y)
		{
			p_tile.GetComponent<Tile>().setPlayer(PLAYERS.Retroish);
		}
	}

	void createTheMap()
	{

		float x;
		float y;
		float z;
		Vector3 tilePosition = Vector3.zero;
		GameObject currentTile;

		for (float height = 0; height < m_mapHeight; ++height)
		{
			for (float width = 0; width < m_mapWidth; ++width)			
			{
				x = width + (height%2 * m_tilesUnit / 2); 
				y = -height * (float) 0.75 * m_tilesUnit;
				z = 0;			

				tilePosition = new Vector3(x, y, z);

				currentTile = Instantiate(m_tilePrefab);
				currentTile.transform.position = tilePosition;
				setTheHexagonTMP(currentTile, height, width);
			}
		}
	}

	void setGameState (GAME_STATE p_state)
	{
		m_gameState = p_state;
	}

	Vector2 getPerfectHexPosition(GameObject p_target)
	{
		
		float x;
		float y;
		float currentX = p_target.transform.position.x;
		float currentY = p_target.transform.position.y;
		Vector2 result = Vector2.zero;

		y = currentY / (0.75f * m_tilesUnit);
		x = currentX - (y%2 * m_tilesUnit / 2);

		result = new Vector2 (x, y);

		return result;
	}

	List<DIRECTIONS> getBorders(GameObject p_target)
	{
		List<DIRECTIONS> directions = new List<DIRECTIONS>();

		Vector2 position = getPerfectHexPosition (p_target);

		if (position.y == 0)
		{
			addThisDirection(directions, DIRECTIONS.NORTHEAST);
			addThisDirection(directions, DIRECTIONS.NORTHWEST);
		}

		if (position.y == m_mapHeight)
		{
			addThisDirection(directions, DIRECTIONS.SOUTHEAST);
			addThisDirection(directions, DIRECTIONS.SOUTHWEST);
		}

		if (position.x == 0)
		{
			addThisDirection(directions, DIRECTIONS.WEST);

			if (position.y%2 == 0)
			{
				addThisDirection(directions, DIRECTIONS.NORTHWEST);
				addThisDirection(directions, DIRECTIONS.SOUTHWEST);
			}
		}

		if (position.x == m_mapWidth)
		{
			addThisDirection(directions, DIRECTIONS.EAST);
		
			if (position.y%2 == 0)
			{
				addThisDirection(directions, DIRECTIONS.NORTHEAST);
				addThisDirection(directions, DIRECTIONS.SOUTHEAST);
			}
		}

		return directions;

	}

	void addThisDirection(List<DIRECTIONS> p_list, DIRECTIONS p_direction)
	{
		if (!p_list.Contains(p_direction))
		{
			p_list.Add(p_direction);
		}
	}
}
