using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public Material[] m_colors;
	public GameObject m_base;
	public Material m_neutral;

	public GameObject[] m_environments;
	public Vector3[] m_environmentPosition;
	public GameObject[] m_tokens;
	public Vector3 m_tokenPosition;

	[SerializeField]
	private PLAYERS m_player;
	[SerializeField]
	private TILE_TYPE m_tileType;
	[SerializeField]
	private int m_unitNumbers;
	[SerializeField]
	private bool m_isMoved;
	[SerializeField]
	private GameObject m_environment;
	private Vector3 m_pointFroEnvironment;
	[SerializeField]
	private GameObject m_token;

	// Use this for initialization
	void Start () {
		m_tileType = TILE_TYPE.Neutral;
		m_player = PLAYERS.None;
		m_unitNumbers = 0;
		m_isMoved = false;
		m_environment = null;
		m_token = null;
	}

	void Update()
	{
		setMaterial ();
		GameObject environment;

		if (m_tileType != TILE_TYPE.Neutral && m_environment == null)
		{

			//m_pointFroEnvironment = new Vector3(-0.283f, 0.089f, -0.265f);

			environment = m_environments[(int) m_tileType];
			m_environment = (GameObject) Instantiate(environment, environment.transform.position, environment.transform.rotation);
			m_environment.transform.parent = gameObject.transform;
			m_environment.transform.position = m_environmentPosition[(int) m_tileType] + transform.position;
		}
	}

	//Getter
	public PLAYERS getPlayer()
	{
		return m_player;
	}

	public	TILE_TYPE getTileType()
	{
		return m_tileType;
	}

	public int getUnitNumbers()
	{
		return m_unitNumbers;
	}

	public bool isMoved()
	{
		return m_isMoved;
	}

	//Setter
	public void setPlayer(PLAYERS p_player)
	{
		m_player = p_player;
		setMaterial ();

	}

	public void setTileType(TILE_TYPE p_type)
	{
		m_tileType = p_type;
	}

	public void setUnitNumbers(int p_number)
	{
		m_unitNumbers = p_number;
	}

	public void setIsMoved(bool p_moved)
	{
		m_isMoved = p_moved;
	}

	//OtherFunctions
	void setMaterial()
	{
		if (m_player != PLAYERS.None)
		{
			m_base.GetComponent<MeshRenderer> ().material =  m_colors [(int) m_player];
		}
		else
		{
			m_base.GetComponent<MeshRenderer> ().material =  m_neutral;
		}
	}

	public void updateTokens()
	{
		GameObject token;
		
		if (m_unitNumbers > 0)
		{
			if (m_token != null)
			{
				Destroy(m_token);
			}

			token = m_tokens[m_unitNumbers];
			m_token = (GameObject) Instantiate(token, token.transform.position, token.transform.rotation);
			m_token.transform.parent = gameObject.transform;
			m_token.transform.position = m_tokenPosition + transform.position;
		}
	}
}
