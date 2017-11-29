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
            i
        };

        [System.Serializable]
        public enum CONDITION
        {
            equal,
            inferior,
            superior
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

        public Parameters()
        {
            floatValues = new FloatDic();
            floatValues.dictionary = new Dictionary<string, float>();

            intValues = new IntDic();
            intValues.dictionary = new Dictionary<string, int>();

            boolValues = new BoolDic();
            boolValues.dictionary = new Dictionary<string, bool>();
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
            count++;
        }

        public void Clear()
        {
            floatValues.dictionary.Clear();
            boolValues.dictionary.Clear();
            intValues.dictionary.Clear();
            count = 0;
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


        public bool TestFloat(string _key, CONDITION _condition, float _marker)
        {
            if(floatValues.dictionary.ContainsKey(_key) == false)
            {
                Debug.LogError("Float key not found");
                return false;
            }
            float value = floatValues.dictionary[_key];

            switch(_condition)
            {
                case CONDITION.inferior:
                    return value < _marker;
                case CONDITION.superior:
                    return value > _marker;
                default:
                    Debug.LogError("The current condition cannot be tested on a float value");
                    return false;
            }
        }
        public bool TestInt(string _key, CONDITION _condition, int _marker)
        {
            if (intValues.dictionary.ContainsKey(_key) == false)
            {
                Debug.LogError("Int key not found");
                return false;
            }
            int value = intValues.dictionary[_key];

            switch (_condition)
            {
                case CONDITION.inferior:
                    return value < _marker;
                case CONDITION.superior:
                    return value > _marker;
                case CONDITION.equal:
                    return value == _marker;
                default:
                    Debug.LogError("The current condition cannot be tested on an integer value");
                    return false;
            }
        }
        public bool TestBool(string _key, CONDITION _condition, bool _marker)
        {
            if (boolValues.dictionary.ContainsKey(_key) == false)
            {
                Debug.LogError("Bool key not found");
                return false;
            }
            bool value = boolValues.dictionary[_key];

            switch (_condition)
            {
                case CONDITION.equal:
                    return value == _marker;
                default:
                    Debug.LogError("The current condition cannot be tested on a boolean value");
                    return false;
            }
        }

    }
}
