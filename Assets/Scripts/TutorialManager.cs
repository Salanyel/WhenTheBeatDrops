using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

	public Text m_text;
	public GameObject m_button;
	public float m_pause;
	public GameObject m_secondTileTutorial;

	//Physical Assets
	public GameObject m_controlPoint;
	public GameObject m_cave;
	public GameObject m_production;
	public GameObject m_village;

	public GameObject m_token1;
	public GameObject m_token2;
	public GameObject m_token3;
	public GameObject m_token4;

	public GameObject m_base;
	public GameObject m_secondaryBase;

	public Material m_red;
	public Material m_blue;

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
		m_secondTileTutorial.SetActive (false);
	}

	void Update()
	{

		if (Input.GetButton("RageQuit"))
		{
			Application.LoadLevel(0);
		}

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
			m_text.text = "But take care ! When you're near the victory, you'll produce less notes";
			m_controlPoint.SetActive(true);
			break;

		case 5:
			m_text.text = "Each turn, your orchestra will produce some notes.";
			m_controlPoint.SetActive(false);
			m_production.SetActive(true);
			m_token3.SetActive(true);
			break;

		case 6 :
			m_text.text = "These notes can only move to 1 adjacent hex.(drag & drop with mouse click)\nBut they can be split ! (hold shift during the drag & drop) ";
			m_token3.SetActive(false);
			m_token2.SetActive(true);
			m_secondTileTutorial.SetActive(true);
			break;

		case 7 :
			m_text.text = "By moving your notes on a case, you'll control it.\nYou can also use its bonus.";
			m_base.GetComponent<MeshRenderer>().material = m_red;
			m_secondaryBase.GetComponent<MeshRenderer>().material = m_blue;
			break;

		case 8:
			m_text.text = "Sometimes, ennemies notes are presents. \nIf you want to go on the targeted case, the quantity will make you win the fight.";
			m_secondaryBase.GetComponent<MeshRenderer>().material = m_red;
			m_token2.SetActive(false);
			break;

		case 9:
			m_text.text = "You can also find some other interesting buildings. The trees will hide your notes from the ennemy";
			m_production.SetActive(false);
			m_secondaryBase.GetComponent<MeshRenderer>().material = m_red;
			m_token2.SetActive(false);
			m_secondTileTutorial.SetActive(false);
			m_cave.SetActive(true);
			break;

		case 10 :
			m_text.text = "The villages will augment your troops capacities. \nBut remember. A hex can only contain 4 notes at once.";
			break;

		case 11 :
			m_text.text = "You can return to the main menu and launch a game !";
			break;

		case 12 :
			Application.LoadLevel(0);
			break;
		}
	}

}
