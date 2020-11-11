using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView graphView;
    private string fileName = "New Narrative";

    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
        GenerateMinimap();
    }
   

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    private void ConstructGraphView()
    {
        graphView = new DialogueGraphView(this)
        {
            name = "Dialogue Graph"
        };

        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        var fileNameTextField = new TextField("File Name: ");
        fileNameTextField.SetValueWithoutNotify(fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });
        toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });

        rootVisualElement.Add(toolbar);
    }

    private void GenerateMinimap()
    {
        var minimap = new MiniMap { anchored = true };
        var coords = graphView.contentViewContainer.WorldToLocal(new Vector2(10, 30));
        minimap.SetPosition(new Rect(coords.x, coords.y, 200, 140));
        graphView.Add(minimap);
    }

    private void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name", "OK");
            return;
        }

        var saveUtility = GraphsSaveUtility.GetInstance(graphView);

        if (save)
            saveUtility.SaveGraph(fileName);
        else
            saveUtility.LoadGraph(fileName);
    }
}
