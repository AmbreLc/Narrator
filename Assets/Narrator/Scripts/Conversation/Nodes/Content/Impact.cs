/* NARRATOR PACKAGE : Impact.cs
 * Created by Ambre Lacour
 * 
 * An impact modify parameters' values when entering a link
 * 
 * An impact :
 *      - is on a conversation link
 *      - contains a parameter's name & type
 *      - contains a modifier according the parameter's type 
 *
 * When the conversation reaches a link, all its impacts are applied
 * You can create/edit/delete impacts on links in the narrator window
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
        //__________VARIABLES_________//

        /// <summary>
        /// Parameter's name
        /// </summary>
        public string name;

        /// <summary>
        /// Parameter's type
        /// </summary>
        public Parameters.TYPE type;

        /// <summary>
        /// Parameter's modifier (if int type)
        /// </summary>
        public int intModifier;

        /// <summary>
        /// Parameter's modifier (if float type)
        /// </summary>
        public float floatModifier;

        /// <summary>
        /// Parameter's modifier (if bool type)
        /// </summary>
        public bool boolModifier;



        //___________METHODS______//

        /// <summary>
        /// Apply the impact (modify parameter value according to the modifier value)
        /// </summary>
        /// <param name="_params"></param>
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
