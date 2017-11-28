using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Narrator
{
    [System.Serializable]
    public class ExitBox
    {
        public Rect rect;
        public List<int> nextNodes;

        public void Initialize()
        {
            nextNodes = new List<int>();
            rect = new Rect();
        }

        public void Initialize(Rect _rect)
        {
            nextNodes = new List<int>();
            rect = new Rect(_rect);
        }



    }

}
