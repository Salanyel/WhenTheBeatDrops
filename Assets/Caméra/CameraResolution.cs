using UnityEngine;
using System.Collections;

public class CameraResolution : MonoBehaviour {

	public int m_baseWidth;
	public int m_baseHeight;
    public float m_speed;

	private float m_baseRatio;
	private float m_ratio;
	private Camera m_camera;

	/*	-----
		Return :
		Parameters :
		Function behavior : Function called at the creation of the game. Init the basic ratio of the camera and update the camera aspect
	*/
	void Start () {

		m_camera = GameObject.FindGameObjectWithTag (Tags.m_mainCamera).GetComponent<Camera>();
		m_baseRatio = calculateRatio(m_baseWidth, m_baseHeight);

		updateCameraViewport ();
	}
	
	/*	-----
		Return :
		Parameters :
		Function behavior : Function called every frame. Reset the camera aspect ratio
	*/
	void Update () {
        transform.Translate(Input.GetAxis("Horizontal") * m_speed * Time.deltaTime, Input.GetAxis("Vertical") * m_speed * Time.deltaTime, 0);
		updateCameraViewport ();
	}

	/*	-----
		Return :
		Parameters :
		Function behavior : Calculate the ratio of the camera's height rect in order to modify it based on the screen resolution
	*/
	void updateCameraViewport()
	{
		float resolutionRatio;

		resolutionRatio = calcultateViewportHeight();
		setViewportHeight (resolutionRatio);
	}

	/*	-----
		Return :
			float : Ratio calculated by dividing a width by a height
		Parameters :
			p_width (int) : Width of the screen
			p_height (int) : Height of the screen
		Function behavior : Return the ratio of the width / height
	*/
	float calculateRatio(int p_width, int p_height)
	{
		return (float) p_width / (float) p_height;
	}	

	/*	-----
		Return :
			float : Ratio calculated by dividing the base ratio by the screen ratio to modify the camera rectangle vision
		Parameters :
		Function behavior : Return the ratio of the current ratio / base ratio
	*/
	float calcultateViewportHeight()
	{
		float resolutionRatio;

		m_ratio = calculateRatio (Screen.width, Screen.height);

		resolutionRatio = m_ratio / m_baseRatio;

		return resolutionRatio;
	}

	/*	-----
		Return :
		Parameters :
			p_height (float) : Value of the rect camera height
		Function behavior : Set the height of the camera rect height
	*/
	void setViewportHeight(float p_height)
	{
		Rect myRect = m_camera.rect;

		myRect.Set (myRect.x, myRect.y, myRect.width, p_height);

		m_camera.rect = myRect;
	}
}
