using System;

namespace Core.Models
{
    public enum GameMode
    {
        CreatingPlayer,
        Setup,
        WaitingForPlayer,
        GamePlay,
        GameEnded,
        Exit
    }
}
