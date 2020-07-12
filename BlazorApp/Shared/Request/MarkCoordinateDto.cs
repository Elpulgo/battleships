using Core.Models;
using static Core.Models.CoordinatesHelper;

namespace Shared
{
    public class MarkCoordinateDto
    {
        public MarkCoordinateDto()
        {
        }

        public MarkCoordinateDto(
           Column column,
           int row)
        {
            Column = column;
            Row = row;
        }

        public Column Column { get; set; }

        public int Row { get; set; }
    }
}
