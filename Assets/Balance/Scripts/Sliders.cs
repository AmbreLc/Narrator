using UnityEngine;
using UnityEngine.UI;

public class Sliders : MonoBehaviour
{
    float transparency;
    [SerializeField] private Narrator.NarratorBrainSO brain;


	// Use this for initialization
	void Start ()
    {
        transparency = 0.0f;
        foreach(Text t in GetComponentsInChildren<Text>())
        {
            t.color = new Color(t.color.r, t.color.g, t.color.b, transparency);
        }
        foreach (Image i in GetComponentsInChildren<Image>())
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, transparency);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (transparency < 1.0f)
        {
            transparency += Time.deltaTime;
            foreach (Text t in GetComponentsInChildren<Text>())
            {
                t.color = new Color(t.color.r, t.color.g, t.color.b, transparency);
            }
            foreach (Image i in GetComponentsInChildren<Image>())
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, transparency);
            }
        }
        else
            transparency = 1.0f;
	}

}
