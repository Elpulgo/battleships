using Core.Models;

namespace Shared
{
    public class CreatePlayerDto
    {
        public CreatePlayerDto()
        {
        }

        public CreatePlayerDto(PlayerType type, string name)
        {
            Type = type;
            Name = name;
        }

        public PlayerType Type { get; set; }

        public string Name { get; set; }
    }
}
