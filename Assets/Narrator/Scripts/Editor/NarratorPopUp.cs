using UnityEditor;
using UnityEngine;

namespace Narrator
{
    public class NarratorPopUp : EditorWindow
    {
        static string content;
        static NarratorPopUp window;

        public delegate void PopupDelegate();
        static public PopupDelegate popupDel;

        public static void Init(string _content, Vector3 _pos)
        {
            content = _content;

            window = GetWindow<NarratorPopUp>();
            window.titleContent = GUIContent.none;
            window.position = new Rect(_pos.x, _pos.y, 50.0f, 10.0f);
            window.ShowPopup();
        }

        private void OnGUI()
        {
            content = EditorGUILayout.TextField(content);

            if (Event.current.keyCode == KeyCode.Return)
                popupDel();
        }

        public static string GetContent()
        {
            return content;
        }

        public static void IsOkay()
        {
        }

        public static void ClosePopUp()
        {
            window.Close();
        }

    }
}
