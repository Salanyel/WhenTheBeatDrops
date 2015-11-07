using UnityEngine;
using System.Collections;

public enum TILE_TYPE
{
	Cave, Village, ControlPoint, Neutral, Production, Block
};

public enum PLAYERS
{    
	None = -1,
	Modern = 0, 
    Classical,
    Retroish,
    Baroque
}

public enum NEUTRAL_BEATS
{
    Neutral1 = 0
};

public enum GAME_STATE
{
	PhaseBegins = 0,
	DisplayCurrentTurn,
	InitCurrentTurn,
	PlayCurrentPlayer,
	EndOfCurrentPlayer,
	EndOfTurn
};