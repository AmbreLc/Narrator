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


namespace Narrator
{
    [System.Serializable]
    public class Impact
    {
        public string name;
        public Parameters.TYPE type;
        public int intModifier;
        public float floatModifier;
        public bool boolModifier;

        public void ApplyImpact(Parameters _params)
        {
            switch (type)
            {
                case Parameters.TYPE.f:
                    float fValue = _params.GetFloat(name);
                    _params.SetFloat(name, fValue + floatModifier);
                    break;
                case Parameters.TYPE.b:
                    _params.SetBool(name, boolModifier);
                    break;
                case Parameters.TYPE.i:
                    int iValue = _params.GetInt(name);
                    _params.SetInt(name, iValue + intModifier);
                    break;
                default:
                    Debug.LogError("Cannot test condition, type is unknown");
                    break;
            }
        }
    }

}
