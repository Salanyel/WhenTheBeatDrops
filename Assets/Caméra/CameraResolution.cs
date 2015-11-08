using UnityEngine;
using System.Collections;

public class CameraResolution : MonoBehaviour {

	public int m_baseWidth;
	public int m_baseHeight;
    public float m_speed;

	private float m_baseRatio;
	private float m_ratio;
	private Camera m_camera;

    //limits of the camera motion
    private Vector2 topLeft;
    private Vector2 topRight;
    private Vector2 bottomLeft;
    private Vector2 bottomRight;

    /*	-----
		Return :
		Parameters :
		Function behavior : Function called at the creation of the game. Init the basic ratio of the camera and update the camera aspect
	*/
    void Start () {

		m_camera = GameObject.FindGameObjectWithTag (Tags.m_mainCamera).GetComponent<Camera>();
		m_baseRatio = calculateRatio(m_baseWidth, m_baseHeight);

        float mapWidth = PlayerPrefs.GetFloat(PlayerPreferences.m_mapWidth);
        float mapHeight = PlayerPrefs.GetFloat(PlayerPreferences.m_mapHeight);
        float tilesUnit = PlayerPrefs.GetFloat(PlayerPreferences.m_tilesUnit);

        topLeft = new Vector2(2.5f, -3.5f);
        topRight = new Vector2(6.5f, -3.5f);
        bottomLeft = new Vector2(2.5f, -8);
        bottomRight = new Vector2(6.5f, -8);

        updateCameraViewport ();
	}
	
	/*	-----
		Return :
		Parameters :
		Function behavior : Function called every frame. Reset the camera aspect ratio
	*/
	void Update () {
        
        // Horizontal move
        if (Input.mousePosition.x < (Screen.width * 0.10) || Input.GetAxis("Horizontal") < 0)
        {
            //the mouse is in the first third of the screen: camera moves left
            transform.Translate(-m_speed * Time.deltaTime, 0, 0);
            transform.position = new Vector3(Mathf.Max(transform.position.x, topLeft.x),transform.position.y,-4);
        }
        else if ((Input.mousePosition.x > (Screen.width * 0.90) && Input.mousePosition.y > (Screen.height * 0.10) ) || Input.GetAxis("Horizontal") > 0)
        {
            //the mouse is in the last third of the screen: camera moves right
            transform.Translate(m_speed * Time.deltaTime, 0, 0);
            transform.position = new Vector3(Mathf.Min(transform.position.x, topRight.x), transform.position.y, -4);
        }

        //Vertical move by mouse
        if (Input.mousePosition.y < (Screen.height * 0.10) || Input.GetAxis("Vertical") < 0)
        {
            //the mouse is in the bottom third of the screen: camera moves down
            transform.Translate(0, -m_speed * Time.deltaTime, 0);
            transform.position = new Vector3(transform.position.x, Mathf.Max(transform.position.y,bottomLeft.y), -4);
        }
        else if ((Input.mousePosition.y > (Screen.height * 0.90) && Input.mousePosition.x < (Screen.width * 0.90)) || Input.GetAxis("Vertical") > 0)
        {
            //the mouse is in the top third of the screen: camera moves up
            transform.Translate(0, m_speed * Time.deltaTime, 0);
            transform.position = new Vector3(transform.position.x, Mathf.Min(transform.position.y, topLeft.y), -4);

        }
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
