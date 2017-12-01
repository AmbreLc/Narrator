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

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }

}
