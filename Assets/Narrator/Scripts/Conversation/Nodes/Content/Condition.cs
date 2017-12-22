/* NARRATOR PACKAGE
 * SpeakNode.cs
 * Created by Ambre Lacour, 12/10/2017
 * Editor script used by NarrationEditor in the NarratorWindow
 * 
 * A SpeakNode represents a line of dialogue in a conversatio, it:
 *      - is a dragable window in the Narrator Window  
 *      - is linked to other(s) SpeakNode(s) through its speak member (see the Speak class to learn more)
 *      
 */

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;


namespace Narrator
{
    [System.Serializable]
    public class Condition
    {
        public string name;
        public Parameters.TYPE type;
        public Parameters.OPERATOR test;
        public int intMarker;
        public float floatMarker;
        public bool boolMarker;


        public bool IsComplete(Parameters _params)
        {
            if (_params == null)
            {
                Debug.LogError("Cannot test condition, parameters are null object");
                return false;
            }

            switch (type)
            {
                case Parameters.TYPE.f:
                    return _params.TestFloat(name, test, floatMarker);
                case Parameters.TYPE.b:
                    return _params.TestBool(name, test, boolMarker);
                case Parameters.TYPE.i:
                    return _params.TestInt(name, test, intMarker);
                default:
                    Debug.LogError("Cannot test condition, type is unknown");
                    return false;
            }
        }

    }
}
