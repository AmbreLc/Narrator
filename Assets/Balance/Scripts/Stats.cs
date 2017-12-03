using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Narrator;


public class Stats : MonoBehaviour
{
    [SerializeField] private NarratorBrainSO brain;
    [SerializeField] private string statName;

    [Header("Slider")]
    [SerializeField][Range(-1.0f, 1.0f)]private float value;
    [SerializeField] private Image fillAmount;
    [SerializeField] private Image middleImage;
    [SerializeField] private Color goodColor;
    [SerializeField] private Color badColor;

    float offsetMax = 0.0f;
    float offsetMin = 0.0f;
    
    // Use this for initialization
    void Start ()
    {
        //Debug.Log("max: " + fillAmount.rectTransform.offsetMax.x + " min: " + fillAmount.rectTransform.offsetMin.x);

        offsetMax = fillAmount.rectTransform.offsetMax.x;
        offsetMin = fillAmount.rectTransform.offsetMin.x;      
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(statName + ": " + value + " max: " + fillAmount.rectTransform.offsetMax.x + " min: " + fillAmount.rectTransform.offsetMin.x);

        if (value > 0)
        {
            fillAmount.rectTransform.offsetMax = new Vector2(Mathf.Lerp(offsetMax, 0.0f, Mathf.Abs(value * 2)), fillAmount.rectTransform.offsetMax.y);
            fillAmount.rectTransform.offsetMin = new Vector2(offsetMin, fillAmount.rectTransform.offsetMin.y);
        }
        else
        {
            fillAmount.rectTransform.offsetMax = new Vector2(offsetMax, fillAmount.rectTransform.offsetMax.y);
            fillAmount.rectTransform.offsetMin = new Vector2(Mathf.Lerp(offsetMin, 0.0f, Mathf.Abs(value * 2)), fillAmount.rectTransform.offsetMin.y);
        }

        value = (brain.Parameters.GetInt(statName) - 50) * 0.01f;
        fillAmount.color = Color.Lerp(goodColor, badColor, Mathf.Abs(value * 1.5f));
        middleImage.color = Color.Lerp(goodColor, badColor, Mathf.Abs(value * 1.5f));
    }
}
