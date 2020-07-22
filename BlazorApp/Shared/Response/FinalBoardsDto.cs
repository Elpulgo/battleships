using Core.Models;

namespace Shared
{
    public class FinalBoardsDto
    {
        public FinalBoardsDto()
        {
        }

        public FinalBoardsDto(
           GameBoardBase board,
           GameBoardBase opponentBoard)
        {
            Board = board;
            OpponentBoard = opponentBoard;
        }

        public GameBoardBase Board { get; set; }

        public GameBoardBase OpponentBoard { get; set; }
    }
}
