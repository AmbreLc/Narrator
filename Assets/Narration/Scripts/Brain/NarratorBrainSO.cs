using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Narrator
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "ConversationBrain.asset", menuName = "Narrator/Conversation Brain")]
#endif

    [System.Serializable]
    public class NarratorBrainSO : ScriptableObject
    {
        [SerializeField] private List<Character> npcs;
        public List<Character> NPCs
        {
            get { return npcs; }
        }

        [SerializeField] private List<Character> pcs;
        public List<Character> PCs
        {
            get { return pcs; }
        }


        [SerializeField] private Parameters parameters;
        public Parameters Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        [SerializeField] private List<string> langages;
        public List<string> Langages
        {
            get { return langages; }
        }

        private int currentLangageIndex;
        public int CurrentLangageIndex
        {
            set { currentLangageIndex = value; }
            get { return currentLangageIndex; }
        }
        public string CurrentLangage
        {
            set { if (langages.Contains(value)) currentLangageIndex = langages.IndexOf(value); else Debug.LogError(value + " langage doesn't exist"); }
            get { return langages[currentLangageIndex]; }
        }


        public void AddCharacter(Character _character)
        {
            if (_character.IsPlayable == true && PCs.Contains(_character) == false)
                PCs.Add(_character);
            else if (_character.IsPlayable == false && NPCs.Contains(_character) == false)
                NPCs.Add(_character);
        }
        public void DeleteCharacter(Character _character)
        {
            if (_character.IsPlayable == true && PCs.Contains(_character) == true)
                PCs.Remove(_character);
            else if (_character.IsPlayable == false && NPCs.Contains(_character) == true)
                NPCs.Remove(_character);

        }


        public void CreateBrain()
        {
            npcs = new List<Character>();
            pcs = new List<Character>();
            Color play1 = new Color(0.8f, 0.8f, 1.0f);
            pcs.Add(new Character("Player1", true, play1));

            parameters = new Parameters();

            langages = new List<string>();
            langages.Add("English");
            currentLangageIndex = 0;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        public void ApplyImpact(Impact _impact)
        {
            switch(_impact.type)
            {
                case Parameters.TYPE.i:
                    if(parameters.IntValues.dictionary.ContainsKey(_impact.name) == true)
                    {
                        parameters.SetInt(_impact.name, parameters.IntValues.dictionary[_impact.name] + _impact.intModifier);
                    }
                    break;
                case Parameters.TYPE.f:
                    if (parameters.FloatValues.dictionary.ContainsKey(_impact.name) == true)
                    {
                        parameters.SetFloat(_impact.name, parameters.FloatValues.dictionary[_impact.name] + _impact.floatModifier);
                    }
                    break;
                case Parameters.TYPE.b:
                    if (parameters.BoolValues.dictionary.ContainsKey(_impact.name) == true)
                    {
                        parameters.SetBool(_impact.name, _impact.boolModifier);
                    }
                    break;
                default:
                    Debug.LogError("Can't apply impact : unknown type");
                    break;
            }
        }

        public void AddLangage(string name = "New langage")
        {
            langages.Add(name);
        }

        public string[] GetLangagesArray()
        {
            string[] returnArray = new string[langages.Count];
            for (int i = 0; i <langages.Count; i++)
            {
                returnArray[i] = langages[i];
            }

            return returnArray;
        }


    }



  

}
