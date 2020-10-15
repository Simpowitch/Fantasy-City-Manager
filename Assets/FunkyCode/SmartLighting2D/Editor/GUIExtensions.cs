using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using LightingSettings;
using UnityEditorInternal;
using System.Reflection;
using System;


public class GUIFoldout {
	static Dictionary<object, bool> dictionary = new Dictionary<object, bool>();

	static public bool GetValue(object Object) {
		bool value = false;
		bool exist = dictionary.TryGetValue(Object, out value);

		if (exist == false) {
			dictionary.Add(Object, value);
		}

		return(value);
	}

	static public void SetValue(object Object, bool value) {
		bool resultVal;
		bool exist = dictionary.TryGetValue(Object, out resultVal);

		if (exist) {
			dictionary.Remove(Object);
			dictionary.Add(Object, value);
		}
	}

	static public bool Draw(string name, object Object) {
		bool value = EditorGUILayout.Foldout(GetValue(Object), name );
		SetValue(Object, value);
		return(value);
	}
}

public class GUIFoldoutHeader {
	static Dictionary<object, bool> dictionary = new Dictionary<object, bool>();

	static public bool GetValue(object Object) {
		bool value = false;
		bool exist = dictionary.TryGetValue(Object, out value);

		if (exist == false) {
			dictionary.Add(Object, value);
		}

		return(value);
	}

	static public void SetValue(object Object, bool value) {
		bool resultVal;
		bool exist = dictionary.TryGetValue(Object, out resultVal);

		if (exist) {
			dictionary.Remove(Object);
			dictionary.Add(Object, value);
		}
	}

	static public bool Begin(string name, object Object) {

		#if UNITY_2019_1_OR_NEWER
			bool value = EditorGUILayout.BeginFoldoutHeaderGroup(GetValue(Object), name);
		#else
			bool value = EditorGUILayout.Foldout(GetValue(Object), name);
		#endif

        SetValue(Object, value);
		return(value);
	}

	static public void End() {
		
		#if UNITY_2019_1_OR_NEWER
			EditorGUILayout.EndFoldoutHeaderGroup();
		#endif
	}
}

public class GUISortingLayer {
	
	static public string[] GetSortingLayerNames() {
         System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
         PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
         return (string[])sortingLayersProperty.GetValue(null, new object[0]);
     }
 
     static public int[] GetSortingLayerUniqueIDs() {
         System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
         PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
         return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
     }

	 static public void Draw(LightingSettings.SortingLayer sortingLayer) {
		bool value = GUIFoldout.Draw("Sorting Layer", sortingLayer);
        
		if (value == false) {
			return;
		}

		EditorGUI.indentLevel++;

			string[] sortingLayerNames = GetSortingLayerNames();
			int id = Array.IndexOf(sortingLayerNames, sortingLayer.Name);
			int newId = EditorGUILayout.Popup("Name", id, sortingLayerNames);

            if (newId > -1 && newId < sortingLayerNames.Length) {
                string newName = sortingLayerNames[newId];

                if (newName != sortingLayer.Name)
                {
                    sortingLayer.Name = newName;
                }

            }
			
			sortingLayer.Order = EditorGUILayout.IntField("Order", sortingLayer.Order);

		EditorGUI.indentLevel--;
	 }
}

public class GUIMeshMode {

    public static void Draw(MeshMode meshMode) {
		bool value = GUIFoldout.Draw("Mesh Mode", meshMode);
        
		if (value == false) {
			return;
		}

		EditorGUI.indentLevel++;

		meshMode.enable = EditorGUILayout.Toggle("Enable", meshMode.enable);

		meshMode.alpha = EditorGUILayout.Slider("Alpha", meshMode.alpha, 0, 1);

		meshMode.shader = (MeshMode.MeshModeShader)EditorGUILayout.EnumPopup("Shader", meshMode.shader);

		if (meshMode.shader == MeshMode.MeshModeShader.Custom) {
			meshMode.material = (Material)EditorGUILayout.ObjectField(meshMode.material, typeof(Material), true);
		}

		GUISortingLayer.Draw(meshMode.sortingLayer);

		EditorGUI.indentLevel--;
    }
}

public class GUIBumpMapMode {
	static public void Draw (NormalMapMode bumpMapMode) {
		bool value = GUIFoldout.Draw("Mask Normal Map", bumpMapMode);
        
		if (value == false) {
			return;
		}

		EditorGUI.indentLevel++;

		bumpMapMode.type = (NormalMapType)EditorGUILayout.EnumPopup("Type", bumpMapMode.type);

		bumpMapMode.textureType = (NormalMapTextureType)EditorGUILayout.EnumPopup("Texture Type", bumpMapMode.textureType);

		switch(bumpMapMode.textureType) {
			case NormalMapTextureType.Texture:
				bumpMapMode.texture = (Texture)EditorGUILayout.ObjectField("Texture", bumpMapMode.texture, typeof(Texture), true);

			break;

			case NormalMapTextureType.Sprite:
				bumpMapMode.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", bumpMapMode.sprite, typeof(Sprite), true);

			break;
		}

		EditorGUI.indentLevel--;
	}

	static public void DrawDay (DayNormalMapMode bumpMapMode) {
		bool value = GUIFoldout.Draw("Mask Normal Map", bumpMapMode);
        
		if (value == false) {
			return;
		}

		EditorGUI.indentLevel++;

		bumpMapMode.textureType = (NormalMapTextureType)EditorGUILayout.EnumPopup("Texture Type", bumpMapMode.textureType);

		switch(bumpMapMode.textureType) {
			case NormalMapTextureType.Texture:
				bumpMapMode.texture = (Texture)EditorGUILayout.ObjectField("Texture", bumpMapMode.texture, typeof(Texture), true);

			break;

			case NormalMapTextureType.Sprite:
				bumpMapMode.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", bumpMapMode.sprite, typeof(Sprite), true);

			break;
		}

		EditorGUI.indentLevel--;
	}
}

public class GUIGlowMode {
	static public void Draw(GlowMode glowMode) {
		bool value = GUIFoldout.Draw("Glow Mode", glowMode);
        
		if (value == false) {
			return;
		}

        EditorGUI.indentLevel++;

        glowMode.enable = EditorGUILayout.Toggle("Enable", glowMode.enable);

        glowMode.glowSize = EditorGUILayout.IntSlider("Glow Size", glowMode.glowSize, 1, 10);

        glowMode.glowIterations = EditorGUILayout.IntSlider("Glow Iterations", glowMode.glowIterations, 1, 10);

        EditorGUI.indentLevel--;
	}
}