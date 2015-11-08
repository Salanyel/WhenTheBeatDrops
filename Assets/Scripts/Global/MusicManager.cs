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
    const float TIME_JINGLE = 6;

    //References
    AudioSource m_source;

    public List<AudioClip> m_playerJingles;
    public List<AudioClip> m_neutralJingles;
    public List<AudioClip> m_neutralBeats;

    public List<AudioClip> m_beatBaroque;
    public List<AudioClip> m_beatRetroish;

    List<List<AudioClip>> m_playerBeats;
    public List<AudioClip> m_victories;

    // Managing the current audio playing
    bool m_outro = false;
    float m_fadeRate = 0.01f;

    // currentlyPlaying stuff
    PLAYERS currentPlayer;
    NEUTRAL_BEATS currentNeutral;
    int playerScore;

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

    public void startTurnPlayer(PLAYERS newBeat, int playerScore)
    {
        this.playJinglePlayer();
        this.Invoke("playTurnPlayer", TIME_JINGLE);
    }

    public void startTurnBeat(NEUTRAL_BEATS newBeat)
    {
        this.playJingleNeutral();
        this.Invoke("playTurnNeutral", TIME_JINGLE);
    }

    public void victory(PLAYERS player)
    {
        this.reset();
        m_source.clip = m_victories[(int)currentPlayer];
        m_source.Play();
    }

    // Call when the new controlling player is announced
    void playJinglePlayer()
    {
        this.reset();
        m_source.PlayOneShot(m_playerJingles[(int)currentPlayer], 1.0f);
        Debug.Log("Playing jingle : " + currentPlayer);
    }

    // Call when the turn start, to start the music of the player
    void playTurnPlayer()
    {
        this.reset();
        m_source.loop = true;
        m_source.clip = m_playerBeats[(int)currentPlayer][playerScore];
        m_source.Play();
    }

    // Call when a neutral beat is announced
    void playJingleNeutral()
    {
        this.reset();
        m_source.PlayOneShot(m_neutralJingles[(int)currentNeutral], 1.0f);
    }

    // Call when the turn start, to start the neutral music
    void playTurnNeutral()
    {
        this.reset();
        m_source.loop = true;
        m_source.clip = m_neutralBeats[(int)currentNeutral];
        m_source.Play();
    }
}

