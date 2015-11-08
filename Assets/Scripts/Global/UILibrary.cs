using UnityEngine;
using System.Collections;

public class UILibrary : MonoBehaviour {

	private GameMode m_gameMode;

	void Start()
	{
		try
		{
			m_gameMode = GameObject.FindGameObjectWithTag (Tags.m_gameMode).GetComponent<GameMode> ();
		}
		catch (UnityException e)
		{

		}
	}

	public void returnToTheMainMenu()
	{
		Application.LoadLevel (0);
	}

	public void currentPlayerEndOfTurn()
	{
		m_gameMode.endTheCurrentPlayerTurn ();
	}

	public void mySong()
	{
		m_gameMode.selectMySong ();
	}

	public void loadTutorial()
	{
		Application.LoadLevel (1);
	}

	public void loadANewGame()
	{
		Application.LoadLevel (2);
	}

	public void quitTheApplication()
	{
		Application.Quit ();
	}
}
