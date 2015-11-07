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
    public List<List<AudioClip>> m_playerBeats;
    public List<AudioClip> m_neutralBeats;

    // Managing the current audio playing
    bool m_outro = false;
    float m_fadeRate = 0.05f;

    // Use this for initialization
    void Start ()
    {
        m_source = this.GetComponent<AudioSource>();
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

    // Call that at the end of a turn
    void endTurn()
    {
        this.m_outro = true;
    }

    // Call when the new controlling player is announced
    void playJinglePlayer(PLAYERS newBeat)
    {
        m_source.PlayOneShot(m_playerJingles[(int)newBeat], 1.0f);
    }

    // Call when the turn start, to start the music of the player
    void playTurnPlayer(PLAYERS newBeat, int playerScore)
    {
        m_source.loop = true;
        m_source.clip = m_playerBeats[(int)newBeat][playerScore];
    }

    // Call when a neutral beat is announced
    void playJingleNeutral(NEUTRAL_BEATS newBeat)
    {
        m_source.PlayOneShot(m_neutralJingles[(int)newBeat], 1.0f);
    }

    // Call when the turn start, to start the neutral music
    void playTurnNeutral(NEUTRAL_BEATS newBeat)
    {
        m_source.loop = true;
        m_source.clip = m_neutralBeats[(int)newBeat];
    }
}

