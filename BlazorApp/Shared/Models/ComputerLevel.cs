using System.Collections.Generic;

namespace BlazorApp.Shared
{
    public enum ComputerLevel
    {
        None,
        Childish,
        Easy,
        Hard,
        Impossible
    }

    public static class ComputerLevelExtensions
    {
        public static IEnumerable<ComputerLevel> GetLevels()
        {
            yield return ComputerLevel.Childish;
            yield return ComputerLevel.Easy;
            yield return ComputerLevel.Hard;
            yield return ComputerLevel.Impossible;
        }
    }
}
