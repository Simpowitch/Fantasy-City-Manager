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
    public DialogueNodeData dialogueStart;


    public DialogueNodeData GetNodeByID(string targetNodeID)
    {
        foreach (var node in dialogueNodeDatas)
        {
            if (node.GUID == targetNodeID) return node;
        }
        Debug.LogError($"ID: {targetNodeID}, not found");
        return null;
    }

    public List<DialogueNodeData> GetNextNodesFromPreviousNodeID(string previousNodeID)
    {
        List<DialogueNodeData> dialogueNodes = new List<DialogueNodeData>();
        GetLinksOutFromNodeID(previousNodeID).ForEach(link => dialogueNodes.Add(GetNodeByID(link.targetNodeGUID)));
        return dialogueNodes;
    }

    public List<NodeLinkData> GetLinksOutFromNodeID(string nodeID)
    {
        List<NodeLinkData> relatedNodes = new List<NodeLinkData>();
        nodeLinks.Where(x => x.baseNodeGUID == nodeID).ToList().ForEach(foundLink => relatedNodes.Add(foundLink));
        return relatedNodes;
    }

    public List<string> GetChoiceTextsFromNode(string nodeID)
    {
        List<string> choiceTexts = new List<string>();
        GetLinksOutFromNodeID(nodeID).ForEach(foundLink => choiceTexts.Add(foundLink.portName));
        return choiceTexts;
    }
}
