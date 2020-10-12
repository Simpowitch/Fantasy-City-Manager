﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBlink : MonoBehaviour {
    
    public Color primaryColor = Color.white;
    public Color secondaryColor = Color.black;

    private LightingSource2D lightingSource;
    
    void Start() {
        lightingSource = GetComponent<LightingSource2D>();
    }

    
    void Update() {
        float time = Time.realtimeSinceStartup;
        float step = Mathf.Cos(time);
        Color color = Color.Lerp(primaryColor, secondaryColor, Mathf.Abs(step));

        lightingSource.color = color;

        lightingSource.additiveMode.alpha = color.a * 0.5f;
    }
}