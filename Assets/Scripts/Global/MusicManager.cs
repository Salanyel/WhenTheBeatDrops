using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    //References
    AudioSource m_source;

    public AudioClip m_baroque_intro;
    public AudioClip m_baroque_drop1;
    public AudioClip m_retroish_intro;
    public AudioClip m_retroish_drop1;

    // Managing the current audio playing
    bool intro = false;
    bool looping = false;
    bool outro = false;

    // Use this for initialization
    void Start ()
    {
        m_source = this.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}

