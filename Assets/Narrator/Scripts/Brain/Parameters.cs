/* NARRATOR PACKAGE : Parameter.cs
 * Created by Ambre Lacour
 * 
 * Parameters : values modified by the conversation
 * 
 * A parameter:
 *      - has a name and a type (bool, float, int)
 *      - is stocked in the brain
 *      - can be tested has a condition or modified by a conversation's link
 *      
 * You can create/edit/delete a parameter using the narrator window
 * You can get/set a parameter value during game
 * 
 */

using System.Collections.Generic;
using UnityEngine;

namespace Narrator
{
    [System.Serializable]
    public class Parameters
    {
        [System.Serializable]
        public enum TYPE
        {
            f,
            b,
            i,
            none
        };

        [System.Serializable]
        public enum OPERATOR
        {
            less,
            greater,
            equals,
        }

        [SerializeField] FloatDic floatValues;
        /// <summary>
        /// All float parameters of the brain
        /// </summary>
        public FloatDic FloatValues
        {
            get { return floatValues; }
        }


        [SerializeField] IntDic intValues;
        /// <summary>
        /// All int parameters of the brain
        /// </summary>
        public IntDic IntValues
        {
            get { return intValues; }
        }


        [SerializeField] BoolDic boolValues;
        /// <summary>
        /// All bool parameters of the brain
        /// </summary>
        public BoolDic BoolValues
        {
            get { return boolValues; }
        }

        private int count;
        /// <summary>
        /// How many parameters the brain contains (total)
        /// </summary>
        public int Count
        {
            get { return count; }
        }

        [SerializeField] private List<string> names;
        /// <summary>
        /// All the parameters' names
        /// </summary>
        public List<string> Names
        {
            get { return names; }
            set { names = value; }
        }

        
        //______GET/SET PARAMETERS_____//

        /// <summary>
        /// Get a float value by name
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        public float GetFloat(string _key)
        {
            if (floatValues.dictionary.ContainsKey(_key))
                return floatValues.dictionary[_key];
            else
            {
                Debug.LogError("Conversation doesn't have any " + _key + " in float parameters");
                return -1.0f;
            }
        }

        /// <summary>
        /// Get an int value by name
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int GetInt(string _key)
        {
            if (intValues.dictionary.ContainsKey(_key))
                return intValues.dictionary[_key];
            else
            {
                Debug.LogError("Conversation doesn't have any " + _key + " in int parameters");
                return -1;
            }
        }

        /// <summary>
        /// Get a bool value by name
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool GetBool(string _key)
        {
            if (boolValues.dictionary.ContainsKey(_key))
                return boolValues.dictionary[_key];
            else
            {
                Debug.LogError("Conversation doesn't have any " + _key + " in bool parameters");
                return false;
            }
        }

        /// <summary>
        /// Update a float value by name
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_value"></param>
        public void SetFloat(string _key, float _value)
        {
            if(floatValues.dictionary.ContainsKey(_key))
                floatValues.dictionary[_key] = _value;
            else
                Debug.LogError("Conversation doesn't have any " + _key + " in float parameters");
        }

        /// <summary>
        /// Update an int value by name
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_value"></param>
        public void SetInt(string _key, int _value)
        {
            if (intValues.dictionary.ContainsKey(_key))
                intValues.dictionary[_key] = _value;
            else
                Debug.LogError("Conversation doesn't have any " + _key + " in int parameters");
        }

        /// <summary>
        /// Update a bool value by name
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_value"></param>
        public void SetBool(string _key, bool _value)
        {
            if (boolValues.dictionary.ContainsKey(_key))
                boolValues.dictionary[_key] = _value;
            else
                Debug.LogError("Conversation doesn't have any " + _key + " in bool parameters");
        }


        //______CONDITION TESTERS______//

        /// <summary>
        /// Test if a float parameter fulfils a condition
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_condition"></param>
        /// <param name="_marker"></param>
        /// <returns></returns>
        public bool TestFloat(string _key, OPERATOR _condition, float _marker)
        {
            if(floatValues.dictionary.ContainsKey(_key) == false)
            {
                Debug.LogError("Float key not found");
                return false;
            }
            float value = floatValues.dictionary[_key];

            switch(_condition)
            {
                case OPERATOR.less:
                    return value < _marker;
                case OPERATOR.greater:
                    return value > _marker;
                default:
                    Debug.LogError("The current condition cannot be tested on a float value");
                    return false;
            }
        }

        /// <summary>
        /// Test if an int parameter fulfils a condition
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_condition"></param>
        /// <param name="_marker"></param>
        /// <returns></returns>
        public bool TestInt(string _key, OPERATOR _condition, int _marker)
        {
            if (intValues.dictionary.ContainsKey(_key) == false)
            {
                Debug.LogError("Int key not found");
                return false;
            }
            int value = intValues.dictionary[_key];

            switch (_condition)
            {
                case OPERATOR.less:
                    return value < _marker;
                case OPERATOR.greater:
                    return value > _marker;
                case OPERATOR.equals:
                    return value == _marker;
                default:
                    Debug.LogError("The current condition cannot be tested on an integer value");
                    return false;
            }
        }

        /// <summary>
        /// Test if a bool parameter fulfils a condition
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_condition"></param>
        /// <param name="_marker"></param>
        /// <returns></returns>
        public bool TestBool(string _key, OPERATOR _condition, bool _marker)
        {
            if (boolValues.dictionary.ContainsKey(_key) == false)
            {
                Debug.LogError("Bool key not found");
                return false;
            }
            bool value = boolValues.dictionary[_key];
            return value == _marker;
        }

        /// <summary>
        /// Get the type of a given parameter (return none if parameter doesn't exit)
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        public TYPE GetType(string _key)
        {
            if (floatValues.dictionary.ContainsKey(_key))
                return TYPE.f;
            else if (intValues.dictionary.ContainsKey(_key))
                return TYPE.i;
            else if (boolValues.dictionary.ContainsKey(_key))
                return TYPE.b;
            else
                return TYPE.none;
        }


        //____EDITOR TOOLS____//

        /// <summary>
        /// Constructor
        /// </summary>
        public Parameters()
        {
            floatValues = new FloatDic();
            floatValues.dictionary = new Dictionary<string, float>();

            intValues = new IntDic();
            intValues.dictionary = new Dictionary<string, int>();

            boolValues = new BoolDic();
            boolValues.dictionary = new Dictionary<string, bool>();

            names = new List<string>();
            count = 0;
        }
        
        /// <summary>
        /// [EDITOR ONLY] Delete all parameters
        /// </summary>
        public void Clear()
        {
            floatValues.dictionary.Clear();
            boolValues.dictionary.Clear();
            intValues.dictionary.Clear();
            count = 0;
            names.Clear();
        }


        /// <summary>
        /// Add a new float parameter to the brain
        /// </summary>
        public void AddFloat()
        {
            string key = "NewFloat0";
            int index = 0;
            while (FloatValues.dictionary.ContainsKey(key) == true)
            {
                index++;
                key = "NewFloat" + index;
            }
            FloatValues.dictionary.Add(key, 0.0f);
            names.Add(key);
            count++;
        }

        /// <summary>
        /// Add a new int parameter to the brain
        /// </summary>
        public void AddInt()
        {
            string key = "NewInt0";
            int index = 0;
            while (IntValues.dictionary.ContainsKey(key) == true)
            {
                index++;
                key = "NewInt" + index;
            }
            IntValues.dictionary.Add(key, 0);
            names.Add(key);
            count++;
        }

        /// <summary>
        /// Add a new bool parameter to the brain
        /// </summary>
        public void AddBool()
        {
            string key = "NewBool0";
            int index = 0;
            while (BoolValues.dictionary.ContainsKey(key) == true)
            {
                index++;
                key = "NewBool" + index;
            }
            BoolValues.dictionary.Add(key, false);
            names.Add(key);
            count++;
        }

        /// <summary>
        /// [EDITOR ONLY] Save a parameter modification made via the narrator window
        /// </summary>
        /// <param name="_exKey"></param>
        /// <param name="_newKey"></param>
        /// <param name="_newValue"></param>
        /// <param name="_isDeleting"></param>
        public void SaveFloatModifications(string _exKey, string _newKey, float _newValue, bool _isDeleting)
        {
            // Delete a parameter
            if (_isDeleting && FloatValues.dictionary.ContainsKey(_exKey))
            {
                if (Names.Contains(_exKey))
                    Names.Remove(_exKey);
                FloatValues.dictionary.Remove(_exKey);
            }
            // Modify a parameter
            else
            {
                // Rename
                if (_exKey != string.Empty && FloatValues.dictionary.ContainsKey(_newKey) == false)
                {
                    if (Names.Contains(_exKey))
                        Names.Remove(_exKey);
                    Names.Add(_newKey);

                    FloatValues.dictionary.Remove(_exKey);
                    FloatValues.dictionary.Add(_newKey, _newValue);
                }
                // Change value
                else if (FloatValues.dictionary.ContainsKey(_newKey) && _newValue != FloatValues.dictionary[_newKey])
                {
                    FloatValues.dictionary[_newKey] = _newValue;
                }
            }
        }

        /// <summary>
        /// [EDITOR ONLY] Save a parameter modification made via the narrator window
        /// </summary>
        /// <param name="_exKey"></param>
        /// <param name="_newKey"></param>
        /// <param name="_newValue"></param>
        /// <param name="_isDeleting"></param>
        public void SaveIntModifications(string _exKey, string _newKey, int _newValue, bool _isDeleting)
        {
            // Delete a parameter
            if (_isDeleting && IntValues.dictionary.ContainsKey(_exKey))
            {
                if (Names.Contains(_exKey))
                    Names.Remove(_exKey);
                IntValues.dictionary.Remove(_exKey);
            }
            // Modify a parameter
            else
            {
                // Rename
                if (_exKey != string.Empty && IntValues.dictionary.ContainsKey(_newKey) == false)
                {
                    if (Names.Contains(_exKey))
                        Names.Remove(_exKey);
                    Names.Add(_newKey);

                    IntValues.dictionary.Remove(_exKey);
                    IntValues.dictionary.Add(_newKey, _newValue);
                }
                // Change value
                else if (IntValues.dictionary.ContainsKey(_newKey) && _newValue != IntValues.dictionary[_newKey])
                {
                    IntValues.dictionary[_newKey] = _newValue;
                }
            }
        }

        /// <summary>
        /// [EDITOR ONLY] Save a parameter modification made via the narrator window
        /// </summary>
        /// <param name="_exKey"></param>
        /// <param name="_newKey"></param>
        /// <param name="_newValue"></param>
        /// <param name="_isDeleting"></param>
        public void SaveBoolModifications(string _exKey, string _newKey, bool _newValue, bool _isDeleting)
        {
            // Delete a parameter
            if (_isDeleting && BoolValues.dictionary.ContainsKey(_exKey))
            {
                if (Names.Contains(_exKey))
                    Names.Remove(_exKey);

                BoolValues.dictionary.Remove(_exKey);

            }
            // Modify a parameter
            else
            {
                if (_exKey != string.Empty && BoolValues.dictionary.ContainsKey(_newKey) == false)
                {
                    if (Names.Contains(_exKey))
                        Names.Remove(_exKey);
                    Names.Add(_newKey);

                    BoolValues.dictionary.Remove(_exKey);
                    BoolValues.dictionary.Add(_newKey, _newValue);
                }
                else if (BoolValues.dictionary.ContainsKey(_newKey) && _newValue != BoolValues.dictionary[_newKey])
                {
                    BoolValues.dictionary[_newKey] = _newValue;
                }
            }
        }
    }
}
