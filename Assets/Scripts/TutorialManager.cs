using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

	public Text m_text;
	public GameObject m_button;
	public float m_pause;
	public GameObject m_tileTutorial;

	//Physical Assets
	public GameObject m_controlPoint;
	public GameObject m_cave;
	public GameObject m_production;
	public GameObject m_village;

	public GameObject m_token1;
	public GameObject m_token2;
	public GameObject m_token3;
	public GameObject m_token4;

	private float m_delay = 0;
	private int m_step = 0;

	void Start()
	{
		m_controlPoint.SetActive (false);
		m_cave.SetActive (false);
		m_production.SetActive (false);
		m_village.SetActive (false);
		m_token1.SetActive (false);
		m_token2.SetActive (false);
		m_token3.SetActive (false);
		m_token4.SetActive (false);
	}

	void Update()
	{
		m_delay += Time.deltaTime;

		if (m_delay < m_pause)
			m_button.SetActive (false);
		else
			m_button.SetActive (true);

		displayTutorial ();

	}

	public void nextStep()
	{
		m_delay = 0;
		m_step++;
	}

	public void displayTutorial ()
	{
		switch (m_step)
		{
		case 1:
			m_text.text = "The goal of this game is to complete your music.";
			break;

		case 2:
			m_text.text = "Each player has a special song that he musts play to win !";
			break;

		case 3:
			m_text.text = "To play it, you have to control the holy places";
			m_controlPoint.SetActive(true);
			break;

		case 4:
			m_text.text = "Each turn, your orchestra will produce some notes.";
			m_controlPoint.SetActive(false);
			m_production.SetActive(true);
			m_token1.SetActive(true);
			break;





		}
	}

}
