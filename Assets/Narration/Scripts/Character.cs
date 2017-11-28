using UnityEngine;

namespace Narrator
{
    [System.Serializable]
    public class Character
    {
        [SerializeField] string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [SerializeField] bool isPlayable;
        public bool IsPlayable
        {
            get { return isPlayable; }
            set { isPlayable = value; }
        }



        [SerializeField][HideInInspector] private Color color;
        public Color Color
        {
            get { return color; }

            set { color = value; }
        }

        public Character()
        {
        }

        public Character(string _name, bool _isPlayable, Color _color)
        {
            name = _name;
            isPlayable = _isPlayable;
            color = _color;
        }

    }
}
