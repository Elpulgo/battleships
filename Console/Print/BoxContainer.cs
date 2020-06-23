using Core.Models.Ships;

namespace Console.Print
{
    public class BoxContainer
    {
        public string BoxContent { get; private set; }
        public Color Color { get; private set; }

        public BoxContainer()
        {
        }

        public BoxContainer(string boxContent, Color color)
        {
            BoxContent = boxContent;
            Color = color;
        }

        public BoxContainer Empty()
        {
            BoxContent = " ";
            Color = Color.None;
            return this;
        }
    }
}

