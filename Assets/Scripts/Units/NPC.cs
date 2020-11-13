using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] DialogueContainer dialogueContainer = null;



    private void OnMouseDown()
    {
        if (dialogueContainer)
            DialogueSystem.instance.StartDialogue(dialogueContainer);
    }
}
