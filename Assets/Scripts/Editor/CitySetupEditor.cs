using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CitySetup))]
public class CitySetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CitySetup citySetup = (CitySetup)target;

        if (GUILayout.Button("Create City"))
        {
            citySetup.SetupMap();
        }
    }
}
