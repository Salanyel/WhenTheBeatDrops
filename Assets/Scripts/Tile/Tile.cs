using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public Material[] m_colors;
	public GameObject m_base;
	public Material m_neutral;

	[SerializeField]
	private PLAYERS m_player;
	[SerializeField]
	private TILE_TYPE m_tileType;
	[SerializeField]
	private int m_unitNumbers;
	[SerializeField]
	private bool m_isMoved;

	// Use this for initialization
	void Start () {
		m_tileType = TILE_TYPE.Neutral;
		m_player = PLAYERS.None;
		m_unitNumbers = 0;
		m_isMoved = false;
	}

	void Update()
	{
		setMaterial ();
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
}
