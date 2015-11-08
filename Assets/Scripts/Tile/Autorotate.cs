using UnityEngine;
using System.Collections;

public class Autorotate : MonoBehaviour {

	public float m_rotateSpeed;
	
	void Update()
	{
		Vector3 angle = new Vector3 (0, 1 * m_rotateSpeed, 0);
		gameObject.transform.Rotate (angle);
	}
}
