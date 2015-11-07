using UnityEngine;
using System.Collections;

public class PlayerPreferences : MonoBehaviour {

	public static string m_init = "Init";
	public static string m_mapWidth = "MapWidth";
	public static string m_mapHeight = "MapHeight";
	public static string m_tilesUnit = "TilesUnit";

	public static  void InitThePlayerPrefs()
	{
		PlayerPrefs.SetFloat (m_init, 1);
		PlayerPrefs.SetFloat (m_tilesUnit, 1);
		PlayerPrefs.SetFloat (m_mapWidth, 10);
		PlayerPrefs.SetFloat (m_mapHeight, 10);
	}
}
