using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LightingSettings;

public class ProjectSettingsEditor {

    static public void Draw() {
        EditorGUI.BeginChangeCheck ();

        LightingSettings.ProjectSettings mainProfile = Lighting2D.ProjectSettings;

        mainProfile.Profile = (LightingSettings.Profile)EditorGUILayout.ObjectField("Default Profile", mainProfile.Profile, typeof(LightingSettings.Profile), true);

        EditorGUILayout.Space();

        mainProfile.renderingMode = (RenderingMode)EditorGUILayout.EnumPopup("Rendering Mode", mainProfile.renderingMode);   

        mainProfile.coreAxis = (CoreAxis)EditorGUILayout.EnumPopup("Core Axis", mainProfile.coreAxis);
        
        EditorGUILayout.Space();

        EditorView.Draw(mainProfile);

        EditorGUILayout.Space();

        Atlas.Draw(mainProfile);

        EditorGUI.EndChangeCheck ();

        if (GUI.changed) {
            LightingManager2D.ForceUpdate();
            Lighting2D.UpdateByProfile(mainProfile.Profile);

            EditorUtility.SetDirty(mainProfile);
        }
    }

    

    public class EditorView {
        public static void Draw(LightingSettings.ProjectSettings mainProfile) {
            bool foldout = GUIFoldoutHeader.Begin("Scene View", mainProfile.sceneView);

            if (foldout == false) {
                GUIFoldoutHeader.End();
                return;
            }

            EditorGUI.indentLevel++;   

            EditorGUILayout.Space();
  
            mainProfile.sceneView.drawGizmos = EditorGUILayout.Toggle("Draw Gizmos", mainProfile.sceneView.drawGizmos);

            mainProfile.sceneView.layer =  EditorGUILayout.LayerField("Editor Layer", mainProfile.sceneView.layer);

            EditorGUI.indentLevel--;

            GUIFoldoutHeader.End();
        }
    }

    public class Atlas {
        public static void Draw(LightingSettings.ProjectSettings mainProfile) {
            bool foldout = GUIFoldoutHeader.Begin("Batching (In Development)", mainProfile.atlasSettings);

            if (foldout == false) {
                GUIFoldoutHeader.End();
                return;
            }

            EditorGUI.indentLevel++;

            EditorGUILayout.Space();

            mainProfile.atlasSettings.lightingSpriteAtlas = EditorGUILayout.Toggle("Enable", mainProfile.atlasSettings.lightingSpriteAtlas);

            //mainProfile.atlasSettings.spriteAtlasSize = (SpriteAtlasSize)EditorGUILayout.Popup("Atlas Size", (int)mainProfile.atlasSettings.spriteAtlasSize, AtlasSettings.SpriteAtlasSizeArray);
            

            /*
            mainProfile.atlasSettings.spriteAtlasPreloadFoldersCount = EditorGUILayout.IntField("Folder Count", mainProfile.atlasSettings.spriteAtlasPreloadFoldersCount);

            EditorGUI.indentLevel++;
            
            for(int i = 0; i < mainProfile.atlasSettings.spriteAtlasPreloadFoldersCount; i++) {
                if (mainProfile.atlasSettings.spriteAtlasPreloadFolders.Length <= i) {
                    System.Array.Resize(ref mainProfile.atlasSettings.spriteAtlasPreloadFolders, i + 1);
                }
                mainProfile.atlasSettings.spriteAtlasPreloadFolders[i] = EditorGUILayout.TextField("Folder (" + (i + 1) + ")", mainProfile.atlasSettings.spriteAtlasPreloadFolders[i]);
            }*/
            
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;

            GUIFoldoutHeader.End();
        }
    }
}
