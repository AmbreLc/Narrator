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
        [SerializeField] IntDic intValues;
        [SerializeField] BoolDic boolValues;

        public FloatDic FloatValues
        {
            get { return floatValues; }
        }
        public IntDic IntValues
        {
            get { return intValues; }
        }
        public BoolDic BoolValues
        {
            get { return boolValues; }
        }

        private int count;
        public int Count
        {
            get { return count; }
        }

        [SerializeField] private List<string> names;
        public List<string> Names
        {
            get { return names; }
            set { names = value; }
        }

        


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

        public void AddFloat()
        {
            string key = "NewFloat0";
            int index = 0;
            while(FloatValues.dictionary.ContainsKey(key) == true)
            {
                index++;
                key = "NewFloat" + index;
            }
            FloatValues.dictionary.Add(key, 0.0f);
            names.Add(key);
            count++;
        }
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

        public void Clear()
        {
            floatValues.dictionary.Clear();
            boolValues.dictionary.Clear();
            intValues.dictionary.Clear();
            count = 0;
            names.Clear();
        }

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

        public void SetFloat(string _key, float _value)
        {
            if(floatValues.dictionary.ContainsKey(_key))
                floatValues.dictionary[_key] = _value;
            else
                Debug.LogError("Conversation doesn't have any " + _key + " in float parameters");
        }
        public void SetInt(string _key, int _value)
        {
            if (intValues.dictionary.ContainsKey(_key))
                intValues.dictionary[_key] = _value;
            else
                Debug.LogError("Conversation doesn't have any " + _key + " in int parameters");
        }
        public void SetBool(string _key, bool _value)
        {
            if (boolValues.dictionary.ContainsKey(_key))
                boolValues.dictionary[_key] = _value;
            else
                Debug.LogError("Conversation doesn't have any " + _key + " in bool parameters");
        }


        public void SaveFloatModifications(string _exKey, string _newKey, float _newValue, bool _isDeleting)
        {
            // Delete a parameter
            if (_isDeleting && FloatValues.dictionary.ContainsKey(_exKey))
            {
                if (Names.Contains(_exKey))
                    Names.Remove(_exKey);
                FloatValues.dictionary.Remove(_exKey);
            }
            // Modifie a parameter
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
        public void SaveIntModifications(string _exKey, string _newKey, int _newValue, bool _isDeleting)
        {
            // Delete a parameter
            if (_isDeleting && IntValues.dictionary.ContainsKey(_exKey))
            {
                if (Names.Contains(_exKey))
                    Names.Remove(_exKey);
                IntValues.dictionary.Remove(_exKey);
            }
            // Modifie a parameter
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
        public void SaveBoolModifications(string _exKey, string _newKey, bool _newValue, bool _isDeleting)
        {
            // Delete a parameter
            if (_isDeleting && BoolValues.dictionary.ContainsKey(_exKey))
            {
                if (Names.Contains(_exKey))
                    Names.Remove(_exKey);

                BoolValues.dictionary.Remove(_exKey);
                
            }
            // Change a parameter
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

        public TYPE GetType(string _key)
        {
            if (floatValues.dictionary.ContainsKey(_key))
                return TYPE.f;
            else if (intValues.dictionary.ContainsKey(_key))
                return TYPE.i;
            else if(boolValues.dictionary.ContainsKey(_key))
                return TYPE.b;
            else
            return TYPE.none;
        }

     

    }
}
