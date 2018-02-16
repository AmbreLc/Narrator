/* NARRATOR PACKAGE : Character.cs
 * Created by Ambre Lacour
 * 
 * A playable or non-playable character
 * 
 * A character:
 *      - has a name
 *      - is stocked in the brain
 *      - is playable or non-playable
 *      - has a color (to display his/her nodes)
 *      
 * You can create/edit/delete a character using the narrator window
 */

using UnityEngine;

namespace Narrator
{
    [System.Serializable]
    public class Character
    {
        [SerializeField] string name;
        /// <summary>
        /// Character's name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        [SerializeField] bool isPlayable;
        /// <summary>
        /// Is the character a PC or an NPC ?
        /// </summary>
        public bool IsPlayable
        {
            get { return isPlayable; }
            set { isPlayable = value; }
        }


        [SerializeField][HideInInspector] private Color color;
        /// <summary>
        /// Character color (define his/her nodes color in narrator window)
        /// </summary>
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
