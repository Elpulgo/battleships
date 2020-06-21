using Core.Utilities;

namespace Console.Models
{
    public class PositionSate
    {
        #region Fields

        public int Position_X { get; private set; }
        public int Position_Y { get; private set; }

        public int OldPosition_X { get; private set; }
        public int OldPosition_Y { get; private set; }
        public int Step_X { get; private set; } = 1;
        public int Step_Y { get; private set; } = 10;
        public int OldStep_X { get; private set; } = 1;
        public int OldStep_Y { get; private set; } = 10;
        #endregion

        public PositionSate(int position_X, int position_Y)
        {
            Position_X = position_X;
            Position_Y = position_Y;
            OldPosition_X = Position_X;
            OldPosition_Y = Position_Y;
        }


        public void Validate()
        {
            if (Position_X > System.Console.BufferWidth || Position_Y > System.Console.BufferHeight)
            {
                System.Console.Clear();
                System.Console.WriteLine("Coords are greater than buffer!");
            }
        }
        
        public void Increment_Y()
        {
            if (Step_Y < GameConstants.MaxRowCount)
            {
                Position_Y = Position_Y + 2;
                Step_Y++;
            }
        }

        public void Decrement_Y()
        {
            if (Step_Y > 1 && Position_Y > 1)
            {
                Position_Y = Position_Y - 2;
                Step_Y--;
            }
        }

        public void Increment_X()
        {
            if (Step_X < GameConstants.MaxColumnCount)
            {
                Position_X = Position_X + 4;
                Step_X++;
            }
        }

        public void Decrement_X()
        {
            if (Step_X > 1 && Position_X > 3)
            {
                Position_X = Position_X - 4;
                Step_X--;
            }
        }

        public void CacheCurrentState()
        {
            OldStep_X = Step_X;
            OldStep_Y = Step_Y;
            OldPosition_X = Position_X;
            OldPosition_Y = Position_Y;
        }
    }
}
