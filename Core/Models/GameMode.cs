using System;

namespace Core.Models
{
    public enum GameMode
    {
        CreatingPlayer,
        WaitingForPlayerToJoin,
        Setup,
        WaitingForPlayerSetup,
        GamePlay,
        WaitingForOpponent,
        GameEnded,
        Exit
    }
}
