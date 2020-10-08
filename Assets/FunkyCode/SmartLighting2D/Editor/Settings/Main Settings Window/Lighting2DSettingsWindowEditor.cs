using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using LightingSettings;

public class Lighting2DSettingsWindowEditor : EditorWindow {
	private int tab = 0;

	[MenuItem("Tools/Lighting 2D")]
    public static void ShowWindow() {
        GetWindow<Lighting2DSettingsWindowEditor>(false, "Lighting 2D", true);
    }

	void OnGUI() {
		tab = GUILayout.Toolbar (tab, new string[] { "Profile Settings", "Project Settings"});

		switch (tab) {
			case 0:
				ProfileEditor.Draw();
				break;

			case 1:
				ProjectSettingsEditor.Draw();
				break;
		}
    }
}