using System.Drawing;

namespace NeaProject.Classes
{
    public class Sprite
    {
        private Color _colour;
        public string Name;
        public Sprite(Uri uriSprite, string name)
        {
            _colour = Color.White;
            Name = name;
        }

        public Sprite(string textSprite, string name) 
        {
            _colour = Color.Black;
            Name = name;
        }

        public Sprite(Color colour, string name)
        {
            _colour = colour;
            Name = name;
        }

        public Color GetColourAt(int x, int y) 
        {
            return _colour;
        }
    }
}
