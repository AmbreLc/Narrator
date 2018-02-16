/* NARRATOR PACKAGE : Dictionaries.cs
 * Created by Ambre Lacour
 * 
 * Classes which enable parameters dictionaries serialization
 * 
 */

using System;

namespace Narrator
{
    [Serializable]
    public class FloatDic : SerializableDictionary<string, float> { }

    [Serializable]
    public class IntDic : SerializableDictionary<string, int> { }

    [Serializable]
    public class BoolDic : SerializableDictionary<string, bool> { }
}
