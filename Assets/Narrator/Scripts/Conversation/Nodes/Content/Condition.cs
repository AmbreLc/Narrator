/* NARRATOR PACKAGE : Condition.cs
 * Created by Ambre Lacour
 * 
 * A condition enable or disable the transition to another dialog node
 * 
 * A condition :
 *      - is on a conversation link (between two dialog nodes)
 *      - has a parameter name & type (which parameter is tested ?)
 *      - has an operator (how is the parameter tested ?)
 *      - has a marker (which value is the parameter compared with)
 * 
 * You can create/edit/delete conditions on links in the narrator window
 * 
 */

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;


namespace Narrator
{
    [System.Serializable]
    public class Condition
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
        /// Condition's operator (less, greater, equals)
        /// </summary>
        public Parameters.OPERATOR test;

        /// <summary>
        /// Which value is the parameter compared with (if int type)
        /// </summary>
        public int intMarker;

        /// <summary>
        /// Which value is the parameter compared with (if float type)
        /// </summary>
        public float floatMarker;

        /// <summary>
        /// Which value is the parameter compared with (if bool type)
        /// </summary>
        public bool boolMarker;


        //___________METHODS______//

        /// <summary>
        /// Currently, is the condition complete according to the brain's parameters
        /// </summary>
        /// <param name="_params"></param>
        /// <returns></returns>
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
