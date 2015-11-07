using UnityEngine;
using System.Collections;

public class GameMode : MonoBehaviour {

	public GameObject m_tilePrefab;

	private float m_mapWidth;
	private float m_mapHeight;
	private float m_tilesUnit;

	void Start () {
		if (PlayerPrefs.HasKey(PlayerPreferences.m_init))
		{
			PlayerPreferences.InitThePlayerPrefs();
		}

		m_mapWidth = PlayerPrefs.GetFloat (PlayerPreferences.m_mapWidth);
		m_mapHeight = PlayerPrefs.GetFloat (PlayerPreferences.m_mapHeight);
		m_tilesUnit = PlayerPrefs.GetFloat (PlayerPreferences.m_tilesUnit);

		Debug.Log ("Tiles unit : " + m_tilesUnit);

		createTheMap ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void createTheMap()
	{

		float x;
		float y;
		float z;
		Vector3 tilePosition = Vector3.zero;
		GameObject currentTile;

		for (int height = 0; height < m_mapHeight; ++height)
		{
			for (float width = 0; width < m_mapWidth; ++width)			
			{
				x = width + (height%2 * m_tilesUnit / 2); 
				y = -height * (float) 0.75 * m_tilesUnit;
				z = 0;			

				tilePosition = new Vector3(x, y, z);

				Debug.Log(tilePosition);

				currentTile = Instantiate(m_tilePrefab);
				currentTile.transform.position = tilePosition;
			}
		}
	}
}
