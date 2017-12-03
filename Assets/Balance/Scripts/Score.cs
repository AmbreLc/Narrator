using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    Text text;


    void Start ()
    {
        text = GetComponent<Text>();
	}


    void Update ()
    {
        text.text = "You have been stable during " + GameManager.score + " months";
	}
}
