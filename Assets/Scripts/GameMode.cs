using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class GameMode : MonoBehaviour {

    // CONSTANTS
    public const int MAX_NUMBER_OF_PLAYERS = 2;
    public const int MAX_NUMBER_OF_NEUTRAL_BEATS = 1;

	//GameMode configuration
    public GameObject m_tilePrefab;
	private GAME_STATE m_gameState;

	//Map creation
	private float m_mapWidth;
	private float m_mapHeight;
	private float m_tilesUnit;
	private int m_unitsSpawnNumber;
	private int m_unitsPerTile;
	private int m_beatsFrequency;
	private int m_beatsToWin;
	private int m_foodsPerVillage;
	private int m_notesProductionReduction;

	//Victory conditions
	private int m_beatsNumber;

	//Players configuration
	private int m_numberOfPlayers;
	private PLAYERS[] m_players;
	private int m_populationLimit;
	private PLAYERS m_currentPlayer;
	private int m_currentPlayerIndex;
	private int[] m_victoryPoint;
	private int[] m_foodLimit;

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

	//Player turn
	public float m_fadingSpeedPlayerTurn;

	//Songs Menu
	public float m_fadingSpeedSongsMenu;
	private PLAYERS m_lastWinner;
	private GameObject[] m_songsButton;

    // Song Management
    // if isPlayerBeat == true, we're playing the currently selected playerBeat. Else, playing the neutral beat
    bool m_song_isPlayerBeat;
    PLAYERS m_song_playerBeat;
    NEUTRAL_BEATS m_song_neutralBeat;
    MusicManager m_musicManager;

	//Game management
	private CameraResolution m_camera;

    void Start () {
        m_musicManager = GameObject.FindGameObjectWithTag(Tags.m_musicManager).GetComponent<MusicManager>();
		m_camera = GameObject.FindGameObjectWithTag(Tags.m_mainCamera).GetComponent<CameraResolution>();

		if (PlayerPrefs.HasKey(PlayerPreferences.m_init))
		{
			PlayerPreferences.InitThePlayerPrefs();
		}
		//TODO : delete this line of init
		PlayerPreferences.InitThePlayerPrefs();

		setBoard ();
		setPlayers ();

		m_winner = PLAYERS.None;
		setGameState (GAME_STATE.InitTheMap);

		m_resultButton = GameObject.FindGameObjectWithTag(Tags.m_ui_resultButton);
		m_playerHUD = GameObject.FindGameObjectWithTag (Tags.m_ui_playerHUD);
		m_endOfTurnButton = GameObject.FindGameObjectWithTag (Tags.m_ui_endOfTheTurnButton);
		m_songsButton = GameObject.FindGameObjectsWithTag (Tags.m_ui_beatsButton);

		foreach(GameObject beatButton in m_songsButton)
		{
			beatButton.SetActive(false);
		}

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
			//Create the map and its contents
			InitTheMap();
			InitTheMapFor2Players_default();
			break;				

		case GAME_STATE.PhaseBegins :
			//Test if somebody has won the game. If not, continue the game
			phaseBegins();
			break;

		case GAME_STATE.DisplayCurrentTurn :
			//Display the "Game turn X" screen
			displayCurrentTurn();
			break;

		case GAME_STATE.InitCurrentTurn :
			//Sort the plyers tab to change the first player each turn
			initCurrentTurn();
			break;

		case GAME_STATE.PlayCurrentPlayer :
			//Display the "Turn of player X" screen & create new units
			playCurrentTurn();
			break;

		case GAME_STATE.EndOfCurrentPlayer :
			//
			updatePlayerHUD();
			break;

		case GAME_STATE.EndOfTurn :
			endOfTurn();
			break;

		case GAME_STATE.DisplaySongsMenu :
			updateUIForSongsMenu();
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
		setGameState(GAME_STATE.PlayCurrentPlayer);
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
			int currentFoodPopulation = getFoodForPlayer(m_currentPlayer);
			int spawningQuantity;
			int unitsSpawnNumber = m_unitsSpawnNumber;

			if (m_song_isPlayerBeat && m_currentPlayer == m_song_playerBeat)
			{
				unitsSpawnNumber -= m_notesProductionReduction;
			}

			foreach(GameObject hex in hexes)
			{
				//break the foreach if the population limit has been reached
				if (currentFoodPopulation >= m_foodLimit[(int) m_currentPlayer])
				{
					break;
				}

				if (unitsSpawnNumber + currentFoodPopulation > m_foodLimit[(int) m_currentPlayer])
				{
					spawningQuantity = m_foodLimit[(int) m_currentPlayer] - currentFoodPopulation;
				}
				else
				{
					spawningQuantity = unitsSpawnNumber;
				}

				tile = hex.GetComponent<Tile>();

				//Reset the possibility to move the units
				tile.setIsMoved(false);

				if (tile.getPlayer() == m_currentPlayer)
				{
					if (tile.getTileType() == TILE_TYPE.Production)
					{

						currentUnitNumbers = tile.getUnitNumbers();
						if (currentUnitNumbers + spawningQuantity > m_unitsPerTile)
						{
							nextValue = m_unitsPerTile;
						}
						else
						{
							nextValue = currentUnitNumbers + spawningQuantity;
						}

						tile.setUnitNumbers(nextValue);
					}
				}

				tile.updateTokens();

			}

			m_gameState = GAME_STATE.EndOfCurrentPlayer;
		}
	}

	//Update the HUD to display the correct amount of units, control points and food limits
	void updatePlayerHUD()
	{
		//Display the HUD
		m_playerHUD.SetActive (true);

		if (Input.GetButtonUp("EndOfTurn"))
		{
			endTheCurrentPlayerTurn();
			return;
		}

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

        lText.text = m_currentPlayer + "\nYour units number : " + units[(int)m_currentPlayer] + " / " + m_foodLimit[(int) m_currentPlayer] + "\n Your control points : " + controlPoints[(int)m_currentPlayer] + "\n Your current score : " + m_victoryPoint[(int)m_currentPlayer] + " / 3";

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
		m_lastWinner = PLAYERS.None;

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

			//Get the player with the more control points under its control
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
				m_lastWinner = PLAYERS.None;
			}
			else
			{
				m_lastWinner = beatsWinner;
			}

			m_beatsNumber++;
			setGameState(GAME_STATE.DisplaySongsMenu);
			return;
		}

		m_beatsNumber++;
		setGameState(GAME_STATE.PhaseBegins);
	}

	void updateUIForSongsMenu()
	{
		Image panel = GameObject.FindGameObjectWithTag (Tags.m_ui_4beatsWinner).GetComponent<Image> ();
		Text text = GameObject.FindGameObjectWithTag (Tags.m_ui_4beatsWinnerText).GetComponent<Text> ();
		float value;

		//Display "the panel"
		m_delay += Time.deltaTime;

		if (m_delay < m_fadingSpeedSongsMenu) {
			value = (m_delay / m_fadingSpeedSongsMenu);
			panel.color = synthethiseFadingColor (panel.color, value);
			text .color = synthethiseFadingColor (text.color, value);

			if (m_lastWinner == PLAYERS.None) {
				text.text = "Equality during the last round.\nThe next song will be random.";
			} else {
				text.text = m_lastWinner + "\nhas won the last beat.\n\nChoose the next song !"; 
			}
		} else if (m_delay < 2 * m_fadingSpeedSongsMenu && m_lastWinner == PLAYERS.None) {
			value = 1 - ((m_delay - m_fadingSpeedSongsMenu) / m_fadingSpeedSongsMenu);
			panel.color = synthethiseFadingColor (panel.color, value);
			text .color = synthethiseFadingColor (text.color, value);
		} else if (m_delay > m_fadingSpeedSongsMenu && m_lastWinner != PLAYERS.None) 
		{
			foreach (GameObject beatButton in m_songsButton) {
				beatButton.SetActive (true);
			}
			
			setGameState(GAME_STATE.WaitingForMusic);
		}
		else
		{
			m_delay = 0;
            
            // Launching a random song
			launchARandomNeutralSong();

			setGameState(GAME_STATE.DisplayCurrentTurn);
		}
	}

	void launchARandomNeutralSong()
	{
		m_song_isPlayerBeat = false;
		m_song_playerBeat = PLAYERS.None;
		m_song_neutralBeat = (NEUTRAL_BEATS) Random.Range(0, MAX_NUMBER_OF_NEUTRAL_BEATS);
		
		m_musicManager.startPeriodBeat(m_song_neutralBeat);
	}

	public void selectMySong()
	{
		m_victoryPoint [(int)m_lastWinner]++;

		m_song_isPlayerBeat = true;
		m_song_playerBeat = m_lastWinner;
        m_musicManager.startPeriodPlayer(m_lastWinner, m_victoryPoint[(int)m_lastWinner]);

		clearHUD ();

		//Test if someone as won
		for (int i = 0; i < m_numberOfPlayers; ++i)
		{
			if (m_victoryPoint[i] >= m_beatsToWin)
			{
				m_winner = (PLAYERS) i;
                m_musicManager.victory(m_winner);
			}
		}

		m_delay = 0;
		setGameState (GAME_STATE.PhaseBegins);        
    }

	void clearHUD()
	{
		Image panel = GameObject.FindGameObjectWithTag (Tags.m_ui_4beatsWinner).GetComponent<Image> ();
		Text text = GameObject.FindGameObjectWithTag (Tags.m_ui_4beatsWinnerText).GetComponent<Text> ();
		panel.color = synthethiseFadingColor (panel.color, 0);
		text .color = synthethiseFadingColor (text.color, 0);

		foreach(GameObject beatButton in m_songsButton)
		{
			beatButton.SetActive(false);
		}
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
		m_foodsPerVillage = PlayerPrefs.GetInt(PlayerPreferences.m_foodsPerVillage);
		m_notesProductionReduction = PlayerPrefs.GetInt(PlayerPreferences.m_notesProductionReduction);

		createTheMap ();
	}

	void setPlayers()
	{
		m_players = new PLAYERS[m_numberOfPlayers];
		m_victoryPoint = new int[m_numberOfPlayers];
		m_foodLimit = new int[m_numberOfPlayers];

		for (int i = 0; i < m_numberOfPlayers; ++i)
		{
			m_players[i] = (PLAYERS) (i);
			m_victoryPoint[i] = 0;
			m_foodLimit[i] = m_populationLimit;
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
			InitTheMapFor2Players_default();
			break;

		default:
			break;

		}

		m_beatsNumber = 1;
		launchARandomNeutralSong();
		setGameState (GAME_STATE.PhaseBegins);
	}


    void InitTheMapFor2Players_default()
    {        
		Vector2 p1 = new Vector2(2, 3);
        Vector2 p2 = new Vector2(7, 6);
        List<Vector2> controlPoints = new List<Vector2>();
        List<Vector2> villages = new List<Vector2>();
        List<Vector2> reprod = new List<Vector2>();
        List<Vector2> Caves = new List<Vector2>();
        GameObject[] hexes = GameObject.FindGameObjectsWithTag(Tags.m_tile);
        Tile tile;

        //Add ControlPoints
        controlPoints.Add(new Vector2(4, 0));
        controlPoints.Add(new Vector2(6, 4));
        controlPoints.Add(new Vector2(3, 5));
        controlPoints.Add(new Vector2(5, 9));

        //Add Village
        villages.Add(new Vector2(1, 5));
        villages.Add(new Vector2(4, 4));
        villages.Add(new Vector2(5, 5));
        villages.Add(new Vector2(8, 4));

        //Add Reprod
        reprod.Add(new Vector2(7, 2));
        reprod.Add(new Vector2(2, 7));

        //Add Caves
        Caves.Add(new Vector2(0, 0));
        Caves.Add(new Vector2(1, 0));
        Caves.Add(new Vector2(0, 1));
        Caves.Add(new Vector2(4, 1));
        Caves.Add(new Vector2(5, 1));
        Caves.Add(new Vector2(6, 1));
        Caves.Add(new Vector2(7, 1));
        Caves.Add(new Vector2(4, 2));
        Caves.Add(new Vector2(5, 2));
        Caves.Add(new Vector2(6, 2));
        Caves.Add(new Vector2(8, 2));
        Caves.Add(new Vector2(5, 3));
        Caves.Add(new Vector2(6, 3));
        Caves.Add(new Vector2(8, 3));

        Caves.Add(new Vector2(9, 9));
        Caves.Add(new Vector2(8, 9));
        Caves.Add(new Vector2(9, 8));
        Caves.Add(new Vector2(5, 8));
        Caves.Add(new Vector2(4, 8));
        Caves.Add(new Vector2(3, 8));
        Caves.Add(new Vector2(2, 8));
        Caves.Add(new Vector2(1, 7));
        Caves.Add(new Vector2(3, 7));
        Caves.Add(new Vector2(4, 7));
        Caves.Add(new Vector2(5, 7));
        Caves.Add(new Vector2(1, 6));
        Caves.Add(new Vector2(3, 6));
        Caves.Add(new Vector2(4, 6));

		//to test the travel around the world;

		//TODO : remove these lines
		p1 = new Vector2(0, 2);
		/*Vector2 zero = new Vector2(0, 0);
		Vector2 extemity = new Vector2(9, 9);
		p1 = zero;
		p2 = extemity;
		reprod.Add(zero);
		Caves.Remove(zero);
		reprod.Add(extemity);
		Caves.Remove(extemity);//*/

        foreach (GameObject hex in hexes)
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

	public Vector2 getPerfectHexPosition(GameObject p_target)
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

	public List<string> displayBorders(GameObject p_target)
	{
		List<string> directions = new List<string>();

		Vector2 position = getPerfectHexPosition (p_target);

		Image image;

		if (position.y == 0)
		{
			addThisDirection(directions, "North");
		}

		if (position.y == m_mapHeight - 1)
		{
			addThisDirection(directions, "South");
		}

		if (position.x == 0)
		{
			addThisDirection(directions, "West");
		}

		if (position.x == m_mapWidth - 1)
		{
			addThisDirection(directions, "East");		
		}

		return directions;
	}

	void addThisDirection(List<string> p_list, string p_direction)
	{
		if (!p_list.Contains(p_direction))
		{
			p_list.Add(p_direction);
		}
	}

	public void displacement(GameObject p_source, GameObject p_target, int p_unitsNumber)
    {
        if (p_source != p_target)
        {

			Vector2 startIdx = getPerfectHexPosition(p_source);
			Vector2 stopIdx = getPerfectHexPosition(p_target);
			string direction = "";

			//Used to go from one border of the map to an other
			if (p_target.tag == Tags.m_arrow)
			{
				Vector2 coordinate;
				float targetX = 0;
				float targetY = 0;

				GameObject arrowsTarget = null;

				switch (p_target.name)
				{
				case "North" :
					direction = "South";
					targetX = startIdx.x;
					targetY = m_mapHeight - 1;
					break;

				case "South" :
					direction = "North";
					targetX = startIdx.x;
					targetY = 0;
					break;

				case "West" :
					direction = "East";
					targetX = m_mapWidth - 1;
					targetY = startIdx.y;
					break;

				case "East" :
					direction = "West";
					targetX = 0;
					targetY = startIdx.y;
					break;

				default :
					break;
				}

				coordinate = new Vector2(targetX, targetY);
				arrowsTarget = getTileFromIntegerCoord(coordinate);

				if (arrowsTarget != null)
				{
					m_camera.moveTo(direction);
					moveUnits(p_source, arrowsTarget, p_unitsNumber);
				}
			}

            if (stopIdx.y >= startIdx.y - 1 && stopIdx.y <= startIdx.y + 1)
            {
                if(stopIdx.y == startIdx.y && stopIdx.x >= (startIdx.x - 1) && stopIdx.x <= (startIdx.x + 1)
                    || stopIdx.x >= (startIdx.x - 1 + (startIdx.y % 2)) && stopIdx.x <= (startIdx.x + (startIdx.y % 2)))
                {
                   
					moveUnits(p_source, p_target, p_unitsNumber);
                }
            }
        }
    }

	void moveUnits(GameObject p_source, GameObject p_target, int p_unitsNumber)
	{
		if (p_source.GetComponent<Tile>().getPlayer() != p_target.GetComponent<Tile>().getPlayer())
		{
			//It is an agression : we resolve it
			if (p_source.GetComponent<Tile>().getUnitNumbers() > p_target.GetComponent<Tile>().getUnitNumbers())
			{
				//Attacker wins: his units replace 
				p_target.GetComponent<Tile>().setPlayer(p_source.GetComponent<Tile>().getPlayer());
				p_target.GetComponent<Tile>().setIsMoved(true);
				
				int newNumber = p_unitsNumber - p_target.GetComponent<Tile>().getUnitNumbers();
				p_target.GetComponent<Tile>().setUnitNumbers(newNumber);
				
				p_source.GetComponent<Tile>().setUnitNumbers(p_source.GetComponent<Tile>().getUnitNumbers() - p_unitsNumber);
			}
			else
			{
				//Defensor wins: we just deduct his loss
				int newNumber = p_target.GetComponent<Tile>().getUnitNumbers() - p_unitsNumber;
				p_target.GetComponent<Tile>().setUnitNumbers(newNumber);
				
				p_source.GetComponent<Tile>().setUnitNumbers(p_source.GetComponent<Tile>().getUnitNumbers() - p_unitsNumber);
				
			}
		}
		else 
		{
			if (p_target.GetComponent<Tile>().getUnitNumbers() == 0)
			{
				p_target.GetComponent<Tile>().setIsMoved(true);
			}
			//It is a unit movement: we move up to 4 unit in the tile
			if (p_unitsNumber + p_target.GetComponent<Tile>().getUnitNumbers() <= m_unitsPerTile)
			{
				p_target.GetComponent<Tile>().setUnitNumbers(p_target.GetComponent<Tile>().getUnitNumbers() + p_unitsNumber);
				p_source.GetComponent<Tile>().setUnitNumbers(p_source.GetComponent<Tile>().getUnitNumbers() - p_unitsNumber);
			}
			else
			{
				int nbToMove = m_unitsPerTile - p_target.GetComponent<Tile>().getUnitNumbers();
				p_target.GetComponent<Tile>().setUnitNumbers(m_unitsPerTile);
				p_source.GetComponent<Tile>().setUnitNumbers(p_source.GetComponent<Tile>().getUnitNumbers() - nbToMove);
			}
		}
		
		p_source.GetComponent<Tile>().updateTokens();
		p_target.GetComponent<Tile>().updateTokens();
	}

	public PLAYERS getCurrentPlayer()
	{
		return m_currentPlayer;
	}

	public int getFoodForPlayer(PLAYERS p_player)
	{
		int result = 0;
		Tile tile;
		
		GameObject[] hexes = GameObject.FindGameObjectsWithTag(Tags.m_tile);
		foreach(GameObject hex in hexes)
		{
			tile = hex.GetComponent<Tile>();

			if (tile.getPlayer() == p_player)
			{
				result += tile.getUnitNumbers();
			}
		}
		
		return result;
		
	}

	public int getFoodLimitForPlayer(PLAYERS p_player)
{
		int result = m_populationLimit;
		Tile tile;

		GameObject[] hexes = GameObject.FindGameObjectsWithTag(Tags.m_tile);
		foreach(GameObject hex in hexes)
		{
			tile = hex.GetComponent<Tile>();

			if (tile.getTileType() == TILE_TYPE.Village)
			{
				if (tile.getPlayer() == p_player)
				{
					m_foodLimit[(int) p_player] += m_foodsPerVillage;
				}
			}
		}

		return result;

	}

	public void setFoodLimit(PLAYERS p_player, int p_delta)
	{
		m_foodLimit[(int) p_player] += p_delta;
	}

	public int getFoodPerVillage()
	{
		return m_foodsPerVillage;
	}

	public GameObject getTileFromIntegerCoord(Vector2 p_coord)
	{
		GameObject result = null;
		Vector2 comparator;
		GameObject[] hexes = GameObject.FindGameObjectsWithTag(Tags.m_tile);

		foreach(GameObject hex in hexes)
		{
			comparator = getPerfectHexPosition(hex);

			if (comparator == p_coord)
			{
				result = hex;
				return result;
			}
		}
		return result;
	}
}

