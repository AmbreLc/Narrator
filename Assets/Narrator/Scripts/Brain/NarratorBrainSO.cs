#if UNITY_EDITOR
using UnityEditor;
#endif
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

        [SerializeField] private List<string> languages;
        public List<string> Languages
        {
            get { return languages; }
        }

        private int currentLanguageIndex;
        public int CurrentLanguageIndex
        {
            set { currentLanguageIndex = value; }
            get { return currentLanguageIndex; }
        }
        public string CurrentLanguage
        {
            set { if (languages.Contains(value)) currentLanguageIndex = languages.IndexOf(value); else Debug.LogError(value + " langage doesn't exist"); }
            get { return languages[currentLanguageIndex]; }
        }


        public void AddCharacter(Character _character)
        {
            if (_character.IsPlayable == true && PCs.Contains(_character) == false)
                PCs.Add(_character);
            else if (_character.IsPlayable == false && NPCs.Contains(_character) == false)
                NPCs.Add(_character);
            SaveCharacterList();
        }
        public void DeleteCharacter(Character _character)
        {
            if (_character.IsPlayable == true && PCs.Contains(_character) == true)
                PCs.Remove(_character);
            else if (_character.IsPlayable == false && NPCs.Contains(_character) == true)
                NPCs.Remove(_character);
            SaveCharacterList();
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
        public void SaveCharacterList()
        {
            CharactersEnum.Save(this);
        }

        public void CreateBrain()
        {
            npcs = new List<Character>();
            pcs = new List<Character>();
            Color play1 = new Color(0.8f, 0.8f, 1.0f);
            pcs.Add(new Character("Player1", true, play1));

            parameters = new Parameters();

            languages = new List<string>();
            languages.Add("English");
            currentLanguageIndex = 0;

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

        public void AddLanguage(string name = "New language")
        {
            languages.Add(name);
            Save();
        }

        public void DeleteCurrentLanguage()
        {
            Debug.Assert(languages.Contains(CurrentLanguage), "Error: brain trying to delete a non-existing language");
            languages.Remove(CurrentLanguage);
            Save();
        }

        public void RenameCurrentLanguage(string name)
        {
            Debug.Assert(languages.Contains(CurrentLanguage), "Error: brain trying to rename a non-existing language");
            languages.Remove(CurrentLanguage);
            languages.Add(name);
            Save();
        }

        public string[] GetLanguagesArray()
        {
            string[] returnArray = new string[languages.Count];
            for (int i = 0; i < languages.Count; i++)
            {
                returnArray[i] = languages[i];
            }

            return returnArray;
        }


        private void Save()
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }

}
