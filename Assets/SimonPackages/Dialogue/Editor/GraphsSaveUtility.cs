using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphsSaveUtility
{
    private DialogueGraphView targetGraphView;
    private DialogueContainer containerCache;

    private List<Edge> edges => targetGraphView.edges.ToList();
    private List<DialogueNode> nodes => targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

    public static GraphsSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphsSaveUtility
        {
            targetGraphView = targetGraphView
        };
    }


    public void SaveGraph(string fileName)
    {
        if (!edges.Any()) return; //if there are no edges (no connections) then return

        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

        var connectedPorts = edges.Where(x => x.input.node != null).ToArray();

        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as DialogueNode;
            var inputNode = connectedPorts[i].input.node as DialogueNode;

            dialogueContainer.nodeLinks.Add(new NodeLinkData
            {
                baseNodeGUID = outputNode.GUID,
                portName = connectedPorts[i].output.portName,
                targetNodeGUID = inputNode.GUID
            });
        }

        foreach (var dialogueNode in nodes.Where(node => !node.entryPoint))
        {

            dialogueContainer.dialogueNodeDatas.Add(new DialogueNodeData
            {
                GUID = dialogueNode.GUID,
                dialogueText = dialogueNode.dialogueText,
                position = dialogueNode.GetPosition().position
            });
        }

        DialogueNode entryNode = nodes.Find(node => node.entryPoint);
        var connectedPort = edges.Find(x => x.output.node == entryNode);
        var firstDialogueNode = connectedPort.input.node as DialogueNode;
        dialogueContainer.dialogueStart = dialogueContainer.GetNodeByID(firstDialogueNode.GUID);

        //Auto creates resources folder at root of project under assets if it does not exist yet
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string fileName)
    {
        containerCache = Resources.Load<DialogueContainer>(fileName);
        if (containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found", $"Target dialogue graph file with name '{fileName}' does not exist", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }

    private void ClearGraph()
    {
        //Set entry point guid back from the save. Discard existing guid
        nodes.Find(x => x.entryPoint).GUID = containerCache.nodeLinks[0].baseNodeGUID;

        foreach (var node in nodes)
        {
            if (node.entryPoint) continue;

            //Remove edges that connected to this node
            edges.Where(x => x.input.node == node).ToList().ForEach(edge => targetGraphView.RemoveElement(edge));

            //Then remove the node
            targetGraphView.RemoveElement(node);
        }
    }

    private void CreateNodes()
    {
        foreach (var nodeData in containerCache.dialogueNodeDatas)
        {
            //We pass in the position later on, so we use vector2.zero for now
            var tempNode = targetGraphView.CreateDialogueNode(nodeData.dialogueText, Vector2.zero);
            tempNode.GUID = nodeData.GUID;
            targetGraphView.AddElement(tempNode);

            var nodePorts = containerCache.nodeLinks.Where(x => x.baseNodeGUID == nodeData.GUID).ToList();
            nodePorts.ForEach(x => targetGraphView.AddChoicePort(tempNode, x.portName));
        }
    }

    private void ConnectNodes()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            var connections = containerCache.nodeLinks.Where(x => x.baseNodeGUID == nodes[i].GUID).ToList();
            for (int j = 0; j < connections.Count; j++)
            {
                var targetNodeGUID = connections[j].targetNodeGUID;
                var targetNode = nodes.First(x => x.GUID == targetNodeGUID);
                LinkNodes(nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

                targetNode.SetPosition(new Rect(
                    containerCache.dialogueNodeDatas.First(x => x.GUID == targetNodeGUID).position,
                    targetGraphView.defaultNodeSize
                    ));
            }
        }
    }

    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge
        {
            output = output,
            input = input
        };

        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);

        targetGraphView.Add(tempEdge);
    }
}
