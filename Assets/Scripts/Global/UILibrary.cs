using UnityEngine;
using System.Collections;

public class UILibrary : MonoBehaviour {

	private GameMode m_gameMode;

	void Start()
	{
		m_gameMode = GameObject.FindGameObjectWithTag (Tags.m_gameMode).GetComponent<GameMode> ();
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
}
