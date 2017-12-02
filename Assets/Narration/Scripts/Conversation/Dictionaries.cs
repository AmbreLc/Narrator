using System;

namespace Narrator
{
    [Serializable]
    public class Dialogs : SerializableDictionary<int, Node> { }

    [Serializable]
    public class FloatDic : SerializableDictionary<string, float> { }

    [Serializable]
    public class IntDic : SerializableDictionary<string, int> { }

    [Serializable]
    public class BoolDic : SerializableDictionary<string, bool> { }
}
