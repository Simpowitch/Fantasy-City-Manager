using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;

[InitializeOnLoad]
class Lighting2DStartup {
    static Lighting2DStartup () {
        bool icon_light = UnityEngine.Windows.File.Exists("Assets/Gizmos/light.png");

        if (icon_light == false) {

            try {
                FileUtil.CopyFileOrDirectory("Assets/FunkyCode/SmartLighting2D/Resources/Gizmos", "Assets/Gizmos");
            } catch {

            }
 
            
        }
    }
}
