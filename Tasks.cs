using System;

namespace RaceTo21_Blazor
{
    //value cannot be changed and added in enum
    //it's a simple way to save status
	public enum Tasks
	{
        SaveSetting,
        ShowConfirming,
        PlayerTurn,
        CheckForEnd,
        NextRound,
        GameOver,
    }
}

