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

        Layers.Draw(mainProfile);

        EditorGUILayout.Space();
        
        LightSettings.Draw(mainProfile);

        EditorGUILayout.Space();

        Atlas.Draw(mainProfile);

        EditorGUILayout.Space();

        EditorView.Draw(mainProfile);

        EditorGUILayout.Space();
        
        mainProfile.triangulation = (PolygonTriangulator2D.Triangulation)EditorGUILayout.EnumPopup("Triangulation", mainProfile.triangulation);

        EditorGUI.EndChangeCheck ();

        if (GUI.changed) {
            LightingManager2D.ForceUpdate();
            Lighting2D.UpdateByProfile(mainProfile.Profile);

            EditorUtility.SetDirty(mainProfile);
        }
    }

    public class Layers {

        public static void Draw(LightingSettings.ProjectSettings mainProfile) {
            bool foldout = GUIFoldoutHeader.Begin("Layers", mainProfile.layers);
    
            if (foldout == false) {
                GUIFoldoutHeader.End();
                return;
            }

            EditorGUI.indentLevel++;

                EditorGUILayout.Space();

                DrawList(mainProfile.layers.lightLayers, "Light Layers", "Light Layer");

                EditorGUILayout.Space();

                DrawList(mainProfile.layers.nightLayers, "Night Layers", "Night Layer");

                EditorGUILayout.Space();

                DrawList(mainProfile.layers.dayLayers, "Day Layers", "Day Layer");

            EditorGUI.indentLevel--;

            GUIFoldoutHeader.End();
        }

        public static void  DrawList(LightingSettings.LayersList layerList, string name, string singular) {
            bool foldout = GUIFoldout.Draw(name, layerList);

            if (foldout == false) {
                return;
            }
            
            EditorGUI.indentLevel++;

            int lightLayerCount = EditorGUILayout.IntSlider ("Count", layerList.names.Length, 1, 10);

            if (lightLayerCount != layerList.names.Length) {
                int oldCount = layerList.names.Length;

                System.Array.Resize(ref layerList.names, lightLayerCount);

                for(int i = oldCount; i < lightLayerCount; i++) {
                    layerList.names[i] = singular + " " + (i + 1);
                }

            }

            for(int i = 0; i < lightLayerCount; i++) {
                layerList.names[i] = EditorGUILayout.TextField(" ", layerList.names[i]);
            }

            EditorGUI.indentLevel--;
        }
    }

    public class LightSettings {
        public static void Draw(LightingSettings.ProjectSettings mainProfile) {
            bool foldout = GUIFoldoutHeader.Begin("Light Settings", mainProfile.lightingBufferSettings);

            if (foldout == false) {
                GUIFoldoutHeader.End();
                return;
            }

            EditorGUI.indentLevel++;

            EditorGUILayout.Space();

            mainProfile.lightingBufferSettings.fixedLightTextureSize = (LightingSourceTextureSize)EditorGUILayout.Popup("Resolution", (int)mainProfile.lightingBufferSettings.fixedLightTextureSize, LightingSourceSettings.LightingSourceTextureSizeArray);

            EditorGUI.BeginDisabledGroup(Lighting2D.lightingBufferSettings.fixedLightTextureSize == LightingSettings.LightingSourceTextureSize.Custom);
                
            mainProfile.lightingBufferSettings.lightingBufferPreloadCount = (int)EditorGUILayout.Slider("Pre-Load Buffers", mainProfile.lightingBufferSettings.lightingBufferPreloadCount, 0, 50);

            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel--;

            GUIFoldoutHeader.End();
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
            bool foldout = GUIFoldoutHeader.Begin("Atlas", mainProfile.atlasSettings);

            if (foldout == false) {
                GUIFoldoutHeader.End();
                return;
            }

            EditorGUI.indentLevel++;

            EditorGUILayout.Space();

            mainProfile.atlasSettings.lightingSpriteAtlas = EditorGUILayout.Toggle("Enable", mainProfile.atlasSettings.lightingSpriteAtlas);

            //mainProfile.atlasSettings.spriteAtlasSize = (SpriteAtlasSize)EditorGUILayout.Popup("Atlas Size", (int)mainProfile.atlasSettings.spriteAtlasSize, AtlasSettings.SpriteAtlasSizeArray);
            
            mainProfile.atlasSettings.spriteAtlasPreloadFoldersCount = EditorGUILayout.IntField("Folder Count", mainProfile.atlasSettings.spriteAtlasPreloadFoldersCount);

            EditorGUI.indentLevel++;
            
            for(int i = 0; i < mainProfile.atlasSettings.spriteAtlasPreloadFoldersCount; i++) {
                if (mainProfile.atlasSettings.spriteAtlasPreloadFolders.Length <= i) {
                    System.Array.Resize(ref mainProfile.atlasSettings.spriteAtlasPreloadFolders, i + 1);
                }
                mainProfile.atlasSettings.spriteAtlasPreloadFolders[i] = EditorGUILayout.TextField("Folder (" + (i + 1) + ")", mainProfile.atlasSettings.spriteAtlasPreloadFolders[i]);
            }
            
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;

            GUIFoldoutHeader.End();
        }
    }
}
