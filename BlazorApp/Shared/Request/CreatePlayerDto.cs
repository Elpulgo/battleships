using Core.Models;

namespace Shared
{
    public class CreatePlayerDto
    {
        public CreatePlayerDto()
        {
        }

        public CreatePlayerDto(
            PlayerType type,
            string name,
            bool playVsComputer)
        {
            Type = type;
            Name = name;
            PlayVsComputer = playVsComputer;
        }

        public PlayerType Type { get; set; }

        public string Name { get; set; }

        public bool PlayVsComputer { get; set; }
    }
}
