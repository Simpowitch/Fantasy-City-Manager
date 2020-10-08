using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour {
    void Update() {
        Lighting2D.Profile.dayLightingSettings.direction -= Time.deltaTime * 20;
    }
}
