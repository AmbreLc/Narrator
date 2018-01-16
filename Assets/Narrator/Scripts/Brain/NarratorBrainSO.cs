/*
 * Narrator package : NarratorBrainSO
 * 
 * Infos : the brain centralizes all the data used in conversations (characters, parameters, langages).
        * There is only 1 brain
        * The brain is saved and loaded from a scriptable object
        * If your Asset folder doesn't contains any brain, a brain is automatically generated when opening the Narrator window.
 * 
 */
#if UNITY_EDITOR
#endif

using System.Collections.Generic;
using UnityEngine;


namespace Narrator
{
    [System.Serializable]
    public class NarratorBrainSO : ScriptableObject
    {
        [SerializeField, HideInInspector] private List<Character> npcs;
        /// <summary>
        /// All non playable characters
        /// </summary>
        public List<Character> NPCs
        {
            get { return npcs; }
        }

        [SerializeField, HideInInspector] private List<Character> pcs;
        /// <summary>
        /// All playable characters
        /// </summary>
        public List<Character> PCs
        {
            get { return pcs; }
        }


        [SerializeField, HideInInspector] private Parameters parameters;
        /// <summary>
        /// All parameters
        /// </summary>
        public Parameters Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        [SerializeField, HideInInspector] private List<string> langages;
        /// <summary>
        /// All langages
        /// </summary>
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

        /// <summary>
        /// [EDITOR ONLY] 
        /// </summary>
        /// <param name="_character"></param>
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
        public string[] GetCharactersNames()
        {
            string[] names = new string[NPCs.Count + PCs.Count];

            for(int i = 0; i < names.Length; i++)
            {
                if (i < PCs.Count)
                    names[i] = PCs[i].Name;
                else
                    names[i] = NPCs[i - PCs.Count].Name;
            }
            return names;
        }

        /// <summary>
        /// [EDITOR ONLY]
        /// </summary>
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

        /// <summary>
        /// [IN GAME] Apply impact on parameters value
        /// </summary>
        /// <param name="_impact"></param>
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

        /// <summary>
        /// [EDITOR ONLY] Add new langage to the brain (the modification is saved)
        /// </summary>
        /// <param name="name"></param>
        public void AddLangage(string name = "New language")
        {
            langages.Add(name);
            Save();
        }

        /// <summary>
        /// [EDITOR ONLY] Delete current langage (the modification is saved)
        /// </summary>
        public void DeleteCurrentLangage()
        {
            Debug.Assert(langages.Contains(CurrentLangage), "Error: brain trying to delete a non-existing language");
            langages.Remove(CurrentLangage);
            Save();
        }

        /// <summary>
        /// [EDITOR ONLY] Update current langage name (the modification is saved)
        /// </summary>
        /// <param name="name"></param> 
        public void RenameCurrentLangage(string name)
        {
            Debug.Assert(langages.Contains(CurrentLangage), "Error: brain trying to rename a non-existing language");
            langages.Remove(CurrentLangage);
            langages.Add(name);
            Save();
        }

        /// <summary>
        /// Return brain langages as a string array
        /// </summary>
        /// <returns></returns>
        public string[] GetLangagesArray()
        {
            string[] returnArray = new string[langages.Count];
            for (int i = 0; i < langages.Count; i++)
            {
                returnArray[i] = langages[i];
            }

            return returnArray;
        }

        /// <summary>
        /// [EDITOR ONLY]
        /// </summary>
        private void Save()
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }

}
