using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameMode : MonoBehaviour {

	public GameObject m_tilePrefab;

	private float m_mapWidth;
	private float m_mapHeight;
	private float m_tilesUnit;
	
	private int m_beatsNumber;
	private PLAYERS[] m_players;
	private GAME_STATE m_gameState;
	private int m_populationLimit;
	private int m_numberOfPlayers;
	private PLAYERS m_currentPlayer;
	private int m_currentPlayerIndex;
	private int m_unitsSpawnNumber;
	private int m_unitsPerTile;
	private int m_beatsFrequency;
	private int m_beatsToWin;
	private int[] m_victoryPoint;

	//Result screen
	private PLAYERS m_winner;
	public float m_fadingSpeed;
	private float m_delay = 0f;
	private GameObject m_resultButton;

	//Turn screen
	public float m_fadingSpeedTurn;

	//HUD of the players
	GameObject m_playerHUD;
	GameObject m_endOfTurnButton;

	//Player tunr
	public float m_fadingSpeedPlayerTurn;

	void Start () {
		/*if (PlayerPrefs.HasKey(PlayerPreferences.m_init))
		{
			PlayerPreferences.InitThePlayerPrefs();
		}*/
		PlayerPreferences.InitThePlayerPrefs();

		setBoard ();
		setPlayers ();

		m_winner = PLAYERS.None;
		setGameState (GAME_STATE.InitTheMap);

		m_resultButton = GameObject.FindGameObjectWithTag(Tags.m_ui_resultButton);
		m_playerHUD = GameObject.FindGameObjectWithTag (Tags.m_ui_playerHUD);
		m_endOfTurnButton = GameObject.FindGameObjectWithTag (Tags.m_ui_endOfTheTurnButton);

		m_endOfTurnButton.SetActive (false);
		m_playerHUD.SetActive (false);
		m_resultButton.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () 
	{

		switch (m_gameState)
		{
		case GAME_STATE.InitTheMap :
			InitTheMap();
			InitTheMapFor2Players();
			break;

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
			updatePlayerHUD();
			break;

		case GAME_STATE.EndOfTurn :
			endOfTurn();
			break;

		default:
			break;
		}
	}

	Color synthethiseFadingColor(Color p_color, float p_alpha)
	{
		Color result = new Color (p_color.r, p_color.g, p_color.b, p_alpha);
		return result;
	}

	//Test if somebody has won the game
	void phaseBegins()
	{

		if (m_winner != PLAYERS.None)
		{
			Image panel = GameObject.FindGameObjectWithTag (Tags.m_ui_resultLog).GetComponent<Image> ();
			Text text = GameObject.FindGameObjectWithTag (Tags.m_ui_resultText).GetComponent<Text> ();
			float value;

			text.text = "The game is finished.\n" + m_winner + " is the real beat !";
			m_delay += Time.deltaTime;
			value = (m_delay / m_fadingSpeed);
			panel.color = synthethiseFadingColor(panel.color, value);
			text .color = synthethiseFadingColor(text.color, value);

			if (m_delay > m_fadingSpeed)
			{
				m_resultButton.SetActive (true);
			}
		}
		else
		{
			m_delay = 0;
			setGameState (GAME_STATE.DisplayCurrentTurn);
		}
	}

	//Display current turn
	void displayCurrentTurn()
	{
		Image panel = GameObject.FindGameObjectWithTag (Tags.m_ui_turn).GetComponent<Image> ();
		Text text = GameObject.FindGameObjectWithTag (Tags.m_ui_turnText).GetComponent<Text> ();
		float value;

		m_delay += Time.deltaTime;

		if (m_delay < m_fadingSpeedTurn)
		{
			value = (m_delay / m_fadingSpeedTurn);
			panel.color = synthethiseFadingColor(panel.color, value);
			text .color = synthethiseFadingColor(text.color, value);
			text.text = "Turn " + m_beatsNumber;
		}
		else if (m_delay < 2 * m_fadingSpeedTurn)
		{
			value = 1 - ((m_delay - m_fadingSpeedTurn) / m_fadingSpeedTurn);
			panel.color = synthethiseFadingColor(panel.color, value);
			text .color = synthethiseFadingColor(text.color, value);
		}
		else
		{
			m_delay = 0;
			m_gameState = GAME_STATE.InitCurrentTurn;
		}
	}

	//Sort the m_players Tab
	void initCurrentTurn()
	{
		m_gameState = GAME_STATE.PlayCurrentPlayer;
		m_currentPlayerIndex = 0;

		if (m_numberOfPlayers < 3)
		{
			m_currentPlayer = m_players[0];
			return;
		}			

		PLAYERS playerTmp = m_players[0];

		for (int i = 1; i < m_numberOfPlayers; ++i)
		{
			m_players[i-1] = m_players[i];
		}

		m_currentPlayer = m_players[0];
		m_players [m_numberOfPlayers] = playerTmp;
	}

	//Play all players
	void playCurrentTurn()
	{
		Image panel = GameObject.FindGameObjectWithTag (Tags.m_ui_player).GetComponent<Image> ();
		Text text = GameObject.FindGameObjectWithTag (Tags.m_ui_playerText).GetComponent<Text> ();
		float value;

		//Display the HUD
		m_playerHUD.SetActive (true);
		//updatePlayerHUD ();

		//Display "Turn of ..."
		m_delay += Time.deltaTime;

		if (m_delay < m_fadingSpeedPlayerTurn)
		{
			value = (m_delay / m_fadingSpeedPlayerTurn);
			panel.color = synthethiseFadingColor(panel.color, value);
			text .color = synthethiseFadingColor(text.color, value);
			text.text = "Turn of " + (PLAYERS) m_currentPlayer;
		}
		else if (m_delay < 2 * m_fadingSpeedPlayerTurn)
		{
			value = 1 - ((m_delay - m_fadingSpeedPlayerTurn) / m_fadingSpeedPlayerTurn);
			panel.color = synthethiseFadingColor(panel.color, value);
			text .color = synthethiseFadingColor(text.color, value);
		}
		else
		{
			//Display the end of turn button
			m_endOfTurnButton.SetActive(true);

			//Create new units
			GameObject[] hexes = GameObject.FindGameObjectsWithTag(Tags.m_tile);
			Tile tile;
			int currentUnitNumbers;
			int nextValue;

			foreach(GameObject hex in hexes)
			{
				tile = hex.GetComponent<Tile>();

				//Reset the possibility to move the units
				tile.setIsMoved(false);

				if (tile.getPlayer() == m_currentPlayer)
				{
					if (tile.getTileType() == TILE_TYPE.Production)
					{
						currentUnitNumbers = tile.getUnitNumbers();
						if (currentUnitNumbers + m_unitsSpawnNumber > m_unitsPerTile)
						{
							nextValue = m_unitsPerTile;
						}
						else
						{
							nextValue = currentUnitNumbers + m_unitsSpawnNumber;
						}

						tile.setUnitNumbers(nextValue);
					}
				}

				tile.updateTokens();

			}

			m_gameState = GAME_STATE.EndOfCurrentPlayer;
		}
	}

	//Update the HUD to display the correct amount of units
	void updatePlayerHUD()
	{
        // Text references
        Text lText = GameObject.FindGameObjectWithTag(Tags.m_ui_yourInfo).GetComponent<Text>();
        Text rText = GameObject.FindGameObjectWithTag(Tags.m_ui_opponentsInfo).GetComponent<Text>();

        // For each tile
        GameObject[] hexes = GameObject.FindGameObjectsWithTag(Tags.m_tile);
        Tile tile;

        List<int> controlPoints = new List<int>();
        List<int> units = new List<int>();

        for (int i = 0; i < m_numberOfPlayers; ++i)
        {
            controlPoints.Add(0);
            units.Add(0);
        }

        foreach (GameObject hex in hexes)
        {
            tile = hex.GetComponent<Tile>();
            // It's a cave, and its ours : increment ours
            if (tile.getTileType() == TILE_TYPE.Cave && tile.getPlayer() == this.m_currentPlayer)
            {
                units[(int)tile.getPlayer()] += tile.getUnitNumbers();
            }
            else if (tile.getPlayer() != PLAYERS.None)
            {
                // If its a control point, add a point to the corresponding player
                if (tile.getTileType() == TILE_TYPE.ControlPoint)
                {
                    controlPoints[(int)tile.getPlayer()] += 1;
                }

                //  If there are units on it, add to counter
                units[(int)tile.getPlayer()] += tile.getUnitNumbers();
            }
        }

        lText.text = "Your units number : " + units[(int)m_currentPlayer] + "\n Your control points : " + controlPoints[(int)m_currentPlayer] + "\n Your current score : " + m_victoryPoint[(int)m_currentPlayer] + " / 3";

		string tempText = "Player : Units / Control // Current score\n";
        
        for (int i = 0; i < m_numberOfPlayers; ++i)
        {
            tempText += "Player " + (i + 1) + " : " + units[i] + " / " + controlPoints[i] + " // " + m_victoryPoint[i] + "\n";
        }
        rText.text = tempText;
    }

	//Function triggered by the "EndOfTurn" button
	public void endTheCurrentPlayerTurn()
	{
		//Hide the end of turn button
		m_endOfTurnButton.SetActive(false);

		m_delay = 0;
		++m_currentPlayerIndex;

		if (m_currentPlayerIndex < m_numberOfPlayers)
		{
			m_currentPlayer = m_players[m_currentPlayerIndex];
			setGameState(GAME_STATE.PlayCurrentPlayer);
		}
		else
		{
			setGameState(GAME_STATE.EndOfTurn);
		}
	}

	void endOfTurn()
	{
		m_delay = 0;
		m_playerHUD.SetActive (false);
		int[] controlPoints = new int[m_numberOfPlayers];

		Debug.Log ("Current turn : " + m_beatsNumber + " / " + m_beatsFrequency);

		//Test if its time to score
		if (m_beatsNumber % m_beatsFrequency == 0)
		{
			PLAYERS beatsWinner;

			for (int i = 0; i < m_numberOfPlayers; ++i)
			{
				controlPoints[i] = 0;
			}

			GameObject[] hexes = GameObject.FindGameObjectsWithTag(Tags.m_tile);
			Tile tile;

			foreach(GameObject hex in hexes)
			{
				tile = hex.GetComponent<Tile>();
				if (tile.getTileType() == TILE_TYPE.ControlPoint && tile.getPlayer() != PLAYERS.None)
				{
					controlPoints[(int) tile.getPlayer()] ++;
				}
			}

			//TODO : Get the player with the more control points under its control
			beatsWinner = (PLAYERS) 0;
			for (int i = 1; i < m_numberOfPlayers; ++i)
			{
				if (controlPoints[i] > controlPoints[i-1])
				{
					beatsWinner = (PLAYERS) i;
				}
			}

			bool equality = false;
			for (int i = 0; i < m_numberOfPlayers; ++i)
			{
				if ((PLAYERS) i != beatsWinner)
				{
					if (controlPoints[i] == controlPoints[(int) beatsWinner])
					{
						equality = true;
					}
				}
			}

			if (equality)
			{
				//TODO : Play a random neutral song
			}

			//TODO : if there is no winner, the last more control point choose the next song

			//Test if someOne as won
			for (int i = 0; i < m_numberOfPlayers; ++i)
			{
				if (m_victoryPoint[i] >= m_beatsToWin)
				{
					m_winner = (PLAYERS) i;
					//TODO Laucnh the win music
				}
			}
		}

		m_beatsNumber++;
		setGameState(GAME_STATE.PhaseBegins);
	}

	void setBoard()
	{
		m_mapWidth = PlayerPrefs.GetFloat (PlayerPreferences.m_mapWidth);
		m_mapHeight = PlayerPrefs.GetFloat (PlayerPreferences.m_mapHeight);
		m_tilesUnit = PlayerPrefs.GetFloat (PlayerPreferences.m_tilesUnit);
		m_populationLimit = PlayerPrefs.GetInt (PlayerPreferences.m_populationLimit);
		m_numberOfPlayers = PlayerPrefs.GetInt (PlayerPreferences.m_numberOfPlayers);
		m_unitsSpawnNumber = PlayerPrefs.GetInt (PlayerPreferences.m_unitsSpawn);
		m_unitsPerTile = PlayerPrefs.GetInt(PlayerPreferences.m_unitsPerTile);
		m_beatsFrequency = PlayerPrefs.GetInt(PlayerPreferences.m_beatsFrequency);
		m_beatsToWin = PlayerPrefs.GetInt(PlayerPreferences.m_beatsToWin);

		createTheMap ();
	}

	void setPlayers()
	{
		m_players = new PLAYERS[m_numberOfPlayers];
		m_victoryPoint = new int[m_numberOfPlayers];

		for (int i = 0; i < m_numberOfPlayers; ++i)
		{
			m_players[i] = (PLAYERS) (i);
			m_victoryPoint[i] = 0;
		}

		shufflePlayers (m_numberOfPlayers);
	}

	void shufflePlayers(int p_numbers)
	{

		int target;
		PLAYERS tmp;

		for (int i = 0; i < p_numbers; ++i)
		{
			target = Random.Range(0, p_numbers);
			tmp = m_players[i];
			m_players[i] = m_players[target];
			m_players[target] = tmp;
		}
	}

	void InitTheMap()
	{

		switch(m_numberOfPlayers)
		{
		case 2 :
			InitTheMapFor2Players();
			break;

		default:
			break;

		}

		m_beatsNumber = 1;
		setGameState (GAME_STATE.PhaseBegins);
	}

	void InitTheMapFor2Players()
	{
		Vector2 p1 = new Vector2 (3, 2);
		Vector2 p2 = new Vector2 (7, 6);
		List<Vector2> controlPoints = new List<Vector2> ();
		List<Vector2> villages = new List<Vector2> ();
		List<Vector2> reprod = new List<Vector2> ();
		List<Vector2> Caves = new List<Vector2> ();
		GameObject[] hexes = GameObject.FindGameObjectsWithTag (Tags.m_tile);
		Tile tile;

		//Add ControlPoints
		controlPoints.Add (new Vector2 (2, 2));
		controlPoints.Add (new Vector2 (2, 4));
		controlPoints.Add (new Vector2 (7, 2));
		controlPoints.Add (new Vector2 (7, 9));
		controlPoints.Add (new Vector2 (5, 9));

		//Add Village
		villages.Add (new Vector2 (6, 0));
		villages.Add (new Vector2 (6, 2));
		villages.Add (new Vector2 (4, 6));

		//Add Reprod
		reprod.Add (new Vector2 (1, 0));
		reprod.Add (new Vector2 (1, 2));
		reprod.Add (new Vector2 (9, 4));
		reprod.Add (new Vector2 (9, 6));

		//Add Caves
		Caves.Add(new Vector2 (1, 1));
		Caves.Add(new Vector2 (1, 3));
		Caves.Add(new Vector2 (2, 4));
		Caves.Add(new Vector2 (5, 8));
		Caves.Add(new Vector2 (7, 8));
		Caves.Add(new Vector2 (8, 9));

		foreach(GameObject hex in hexes)
		{
			if (getPerfectHexPosition(hex) == p1)
			{
				hex.name = "Player1Spawn";
				tile = hex.GetComponent<Tile>();
				tile.setPlayer(PLAYERS.Baroque);
				tile.setTileType(TILE_TYPE.Production);
			}

			if (getPerfectHexPosition(hex) == p2)
			{
				hex.name = "Player2Spawn";
				tile = hex.GetComponent<Tile>();
				tile.setPlayer(PLAYERS.Retroish);
				tile.setTileType(TILE_TYPE.Production);
			}

			if (controlPoints.Contains(getPerfectHexPosition(hex)))
			{
				hex.name = "ControlPoints";
				tile = hex.GetComponent<Tile>();
				tile.setTileType(TILE_TYPE.ControlPoint);
			}

			if (reprod.Contains(getPerfectHexPosition(hex)))
			{
				hex.name = "Production";
				tile = hex.GetComponent<Tile>();
				tile.setTileType(TILE_TYPE.Production);
			}

			if (villages.Contains(getPerfectHexPosition(hex)))
			{
				hex.name = "Village";
				tile = hex.GetComponent<Tile>();
				tile.setTileType(TILE_TYPE.Village);
			}

			if (Caves.Contains(getPerfectHexPosition(hex)))
			{
				hex.name = "Caves";
				tile = hex.GetComponent<Tile>();
				tile.setTileType(TILE_TYPE.Cave);
			}
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

		y = - currentY / (0.75f * m_tilesUnit);
		x = currentX - (y%2 * m_tilesUnit / 2);

		result = new Vector2 (x, y);

		return result;
	}

	public void displayBorders(GameObject p_target)
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
	}

	void addThisDirection(List<DIRECTIONS> p_list, DIRECTIONS p_direction)
	{
		if (!p_list.Contains(p_direction))
		{
			p_list.Add(p_direction);
		}
	}

	public void displacement(GameObject p_source, GameObject p_target)
	{
        if(p_source != p_target)
        {
            Vector2 startIdx = getPerfectHexPosition(p_source);
            Vector2 stopIdx = getPerfectHexPosition(p_target);

            if (stopIdx.y >= startIdx.y -1 && stopIdx.y <= startIdx.y +1)
            {
                if (stopIdx.x >= (startIdx.x -1 + (stopIdx.y % 2)) && stopIdx.x <= (startIdx.x + (stopIdx.y % 2)))
                {
                   if(p_source.GetComponent<Tile>().getPlayer() != p_target.GetComponent<Tile>().getPlayer())
                   { 
                      //It is an agression : we resolve it
                        if(p_source.GetComponent<Tile>().getUnitNumbers() > p_target.GetComponent<Tile>().getUnitNumbers())
                        {
                           //Attacker wins: his units replace 
                            p_target.GetComponent<Tile>().setPlayer(p_source.GetComponent<Tile>().getPlayer());
                            p_target.GetComponent<Tile>().setIsMoved(true);

                            int newNumber = p_source.GetComponent<Tile>().getUnitNumbers() - p_target.GetComponent<Tile>().getUnitNumbers();
                            p_target.GetComponent<Tile>().setUnitNumbers(newNumber);

                            p_source.GetComponent<Tile>().setUnitNumbers(0);
                        }
                        else
                        {
                           //Defensor wins: we just deduct his loss
                            int newNumber = p_target.GetComponent<Tile>().getUnitNumbers() - p_source.GetComponent<Tile>().getUnitNumbers();
                            p_target.GetComponent<Tile>().setUnitNumbers(newNumber);

                            p_source.GetComponent<Tile>().setUnitNumbers(0);

                        }
                    }
                    else
                    {
                        //It is a unit movement: we move up to 4 unit in the tile
                        if(p_source.GetComponent<Tile>().getUnitNumbers() + p_target.GetComponent<Tile>().getUnitNumbers() <=4)
                        {
                            p_target.GetComponent<Tile>().setUnitNumbers(p_source.GetComponent<Tile>().getUnitNumbers() + p_target.GetComponent<Tile>().getUnitNumbers());
                            p_source.GetComponent<Tile>().setUnitNumbers(0);
                        }
                        else
                        {
                            int nbToMove = 4 - p_target.GetComponent<Tile>().getUnitNumbers();
                            p_target.GetComponent<Tile>().setUnitNumbers(4);
                            p_source.GetComponent<Tile>().setUnitNumbers(p_source.GetComponent<Tile>().getUnitNumbers() - nbToMove);
                        }
					p_source.GetComponent<Tile>().updateTokens();
					p_target.GetComponent<Tile>().updateTokens();
                }
            }
        }
	}

	public PLAYERS getCurrentPlayer()
	{
		return m_currentPlayer;
	}
}

