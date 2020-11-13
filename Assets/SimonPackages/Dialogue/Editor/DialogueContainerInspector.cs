using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueContainer))]
public class DialogueContainerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DialogueContainer asset = target as DialogueContainer;

        if (GUILayout.Button("Open"))
        {
            DialogueGraph.OpenDialogueGraphWindow();
            DialogueGraph.OpenGraph.LoadExistingFile(asset.name);
        }

        base.OnInspectorGUI();
    }
}
