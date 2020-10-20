using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using LightingSettings;

public class Lighting2DSettingsWindowEditor : EditorWindow {
	private int tab = 0;

	[MenuItem("Tools/2D Light")]
    public static void ShowWindow() {
        GetWindow<Lighting2DSettingsWindowEditor>(false, "2D Light", true);
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