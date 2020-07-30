using BlazorApp.Shared;
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
            bool playVsComputer = false,
            ComputerLevel computerLevel = ComputerLevel.None)
        {
            Type = type;
            Name = name;
            PlayVsComputer = playVsComputer;
            ComputerLevel = computerLevel;
        }

        public PlayerType Type { get; set; }

        public string Name { get; set; }

        public bool PlayVsComputer { get; set; }
        public ComputerLevel ComputerLevel { get; set; } = ComputerLevel.None;
    }
}
