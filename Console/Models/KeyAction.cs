using System;

namespace Console.Models
{
    public class KeyAction
    {
        #region Properties

        public int OldStepX { get; private set; }
        public int OldStepY { get; private set; }

        public int OldPostionX { get; private set; }
        public int OldPositionY { get; private set; }
        public int NewPositionX { get; private set; }
        public int NewPositionY { get; private set; }

        #endregion

        public KeyAction()
        {
        }

        public KeyAction WithOldStep(int x, int y)
        {
            OldStepX = x;
            OldStepY = y;
            return this;
        }

        public KeyAction WithOldPostion(int x, int y)
        {
            OldPostionX = x;
            OldPositionY = y;
            return this;
        }

        public KeyAction WithNewPosition(int x, int y)
        {
            NewPositionX = x;
            NewPositionY = y;
            return this;
        }
    }
}
