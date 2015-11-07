using UnityEngine;
using System.Collections.Generic;

/*
 LE GROS TODO DU BONHEUR POUR PROKO : 
  - Commencer par appeler playJinglePlayer() ou playJingleNeutral() : joue le jingle du player (ou de la musique neutre correspondante)
  - Ensuite, appeller startTurnPlayer() ou startTurnNeutral quand le tour commence : joue la musique correspondante
  - Enfin, appeller endTurn a la fin du tour : le fade out
    */

public class MusicManager : MonoBehaviour
{
    //References
    AudioSource m_source;

    public List<AudioClip> m_playerJingles;
    public List<AudioClip> m_neutralJingles;
    public List<AudioClip> m_neutralBeats;

    public List<AudioClip> m_beatBaroque;
    public List<AudioClip> m_beatRetroish;

    List<List<AudioClip>> m_playerBeats;

    // Managing the current audio playing
    bool m_outro = false;
    float m_fadeRate = 0.01f;

    // Use this for initialization
    void Start ()
    {
        m_source = this.GetComponent<AudioSource>();

        // Initializing beats
        this.m_playerBeats = new List<List<AudioClip>>();
        this.m_playerBeats.Add(m_beatBaroque);
        this.m_playerBeats.Add(m_beatRetroish);
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (m_outro)
        {
            this.m_source.volume -= m_fadeRate;
            if (this.m_source.volume <= 0.0f)
            {
                m_source.Stop();
                m_outro = false;
                m_source.volume = 1.0f;
            }
        }
	}

    // Reset everything
    public void reset()
    {
        this.m_outro = false;
        this.m_source.Stop();
        this.m_source.loop = false;
        this.m_source.volume = 1;
    }


    // Call that at the end of a turn
    public void endTurn()
    {
        this.m_outro = true;
    }

    // Call when the new controlling player is announced
     public void playJinglePlayer(PLAYERS newBeat)
    {
        this.reset();
        m_source.PlayOneShot(m_playerJingles[(int)newBeat], 1.0f);
        Debug.Log("Playing jingle : " + newBeat);
    }

    // Call when the turn start, to start the music of the player
    public void playTurnPlayer(PLAYERS newBeat, int playerScore)
    {
        this.reset();
        m_source.loop = true;
        m_source.clip = m_playerBeats[(int)newBeat][playerScore];
        m_source.Play();
    }

    // Call when a neutral beat is announced
    public void playJingleNeutral(NEUTRAL_BEATS newBeat)
    {
        this.reset();
        m_source.PlayOneShot(m_neutralJingles[(int)newBeat], 1.0f);
    }

    // Call when the turn start, to start the neutral music
    public void playTurnNeutral(NEUTRAL_BEATS newBeat)
    {
        this.reset();
        m_source.loop = true;
        m_source.clip = m_neutralBeats[(int)newBeat];
        m_source.Play();
    }
}

