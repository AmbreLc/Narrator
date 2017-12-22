#if UNITY_EDITOR
using UnityEditor;
using System.IO;

namespace Narrator
{
    public static class CharactersEnum
    {
        public static void Save(NarratorBrainSO _brain)
        {
            string enumName = "Characters";
       
            string[] enumEntries = _brain.GetCharactersNames();
            string filePathAndName = "Assets/Narrator/Scripts/Enums/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                streamWriter.WriteLine("public enum " + enumName);
                streamWriter.WriteLine("{");
                for (int i = 0; i < enumEntries.Length; i++)
                {
                    string entry = enumEntries[i].Replace(' ', '_');
                    streamWriter.WriteLine("\t" + entry + ",");
                }
                streamWriter.WriteLine("}");
            }
            AssetDatabase.Refresh();
        }
    }
}
#endif
