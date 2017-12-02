using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Narrator;


[RequireComponent(typeof(Slider))]
public class Stats : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private NarratorBrainSO brain;
    [SerializeField] private string statName;
	// Use this for initialization
	void Start ()
    {
        slider = GetComponent<Slider>();

        brain.Parameters.SetInt(statName, 50);
	}
	
	// Update is called once per frame
	void Update ()
    {
        slider.value = brain.Parameters.GetInt(statName);

    }
}
