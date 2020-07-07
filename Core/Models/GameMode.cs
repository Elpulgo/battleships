using System;

namespace Core.Models
{
    public enum GameMode
    {
        CreatingPlayer,
        WaitingForPlayerToJoin,
        Setup,
        GamePlay,
        WaitingForOpponent,
        GameEnded,
        Exit
    }
}
