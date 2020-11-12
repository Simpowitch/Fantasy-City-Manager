using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<NodeLinkData> nodeLinks = new List<NodeLinkData>();
    public List<DialogueNodeData> dialogueNodeDatas = new List<DialogueNodeData>();

    public DialogueNodeData GetNodeByLinkTargetID(string targetNodeGUID)
    {
        foreach (var node in dialogueNodeDatas)
        {
            if (node.GUID == targetNodeGUID) return node;
        }
        Debug.LogError($"ID: {targetNodeGUID}, not found");
        return null;
    }

    public List<DialogueNodeData> GetNextNodesFromNodeID(string nodeGUID)
    {
        List<DialogueNodeData> dialogueNodes = new List<DialogueNodeData>();
        GetLinksByNodeID(nodeGUID).ForEach(link => dialogueNodes.Add(GetNodeByLinkTargetID(link.targetNodeGUID)));
        return dialogueNodes;
    }

    public List<NodeLinkData> GetLinksByNodeID(string nodeGUID)
    {
        List<NodeLinkData> relatedNodes = new List<NodeLinkData>();
        nodeLinks.Where(x => x.baseNodeGUID == nodeGUID).ToList().ForEach(foundLink => relatedNodes.Add(foundLink));
        return relatedNodes;
    }
}
