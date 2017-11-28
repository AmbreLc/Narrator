using System.Collections.Generic;
using UnityEngine;

namespace Narrator
{
    [System.Serializable]
    public class Parameters
    {
        public enum TYPE
        {
            f,
            b,
            i
        };

        public enum CONDITION
        {
            equal,
            inferior,
            superior
        }

        [SerializeField] Dictionary<string, float> floatValues;
        [SerializeField] Dictionary<string, int> intValues;
        [SerializeField] Dictionary<string, bool> boolValues;

        public Dictionary<string, float> FloatValues
        {
            get { return floatValues; }
        }
        public Dictionary<string, int> IntValues
        {
            get { return intValues; }
        }
        public Dictionary<string, bool> BoolValues
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
            floatValues = new Dictionary<string, float>();
            intValues = new Dictionary<string, int>();
            boolValues = new Dictionary<string, bool>();

            count = 0;
        }

        public void AddFloat()
        {
            string key = "NewFloat0";
            int index = 0;
            while(FloatValues.ContainsKey(key) == true)
            {
                index++;
                key = "NewFloat" + index;
            }
            FloatValues.Add(key, 0.0f);
            count++;
        }
        public void AddInt()
        {
            string key = "NewInt0";
            int index = 0;
            while (IntValues.ContainsKey(key) == true)
            {
                index++;
                key = "NewInt" + index;
            }
            IntValues.Add(key, 0);
            count++;
        }
        public void AddBool()
        {
            string key = "NewBool0";
            int index = 0;
            while (BoolValues.ContainsKey(key) == true)
            {
                index++;
                key = "NewBool" + index;
            }
            BoolValues.Add(key, false);
            count++;
        }

        public void Clear()
        {
            floatValues.Clear();
            boolValues.Clear();
            intValues.Clear();
            count = 0;
        }

        public float GetFloat(string _key)
        {
            if (floatValues.ContainsKey(_key))
                return floatValues[_key];
            else
            {
                Debug.LogError("Conversation doesn't have any " + _key + " in float parameters");
                return -1.0f;
            }
        }
        public int GetInt(string _key)
        {
            if (intValues.ContainsKey(_key))
                return intValues[_key];
            else
            {
                Debug.LogError("Conversation doesn't have any " + _key + " in int parameters");
                return -1;
            }
        }
        public bool GetBool(string _key)
        {
            if (boolValues.ContainsKey(_key))
                return boolValues[_key];
            else
            {
                Debug.LogError("Conversation doesn't have any " + _key + " in bool parameters");
                return false;
            }
        }

        public void SetFloat(string _key, float _value)
        {
            if(floatValues.ContainsKey(_key))
                floatValues[_key] = _value;
            else
                Debug.LogError("Conversation doesn't have any " + _key + " in float parameters");
        }
        public void SetInt(string _key, int _value)
        {
            if (intValues.ContainsKey(_key))
                intValues[_key] = _value;
            else
                Debug.LogError("Conversation doesn't have any " + _key + " in int parameters");
        }
        public void SetBool(string _key, bool _value)
        {
            if (boolValues.ContainsKey(_key))
                boolValues[_key] = _value;
            else
                Debug.LogError("Conversation doesn't have any " + _key + " in bool parameters");
        }
    }
}
