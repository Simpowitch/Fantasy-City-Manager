using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;

    DialogueContainer dialogue = null;
    DialogueNodeData dialogueNode = null;
    [SerializeField] TextMeshProUGUI eventText = null;
    [SerializeField] GameObject mainPanel = null;
    [SerializeField] GameObject[] choiceButtons = null;
    [SerializeField] GameObject exitButton = null;
    TextMeshProUGUI[] choiceButtonTexts = null;
    public bool dialogueOpen = false;

    private void Awake()
    {
        #region Singleton
        if (DialogueSystem.instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Another instance of: " + this + " , was tried to be instantiated, but was destroyed! This instance was tried to be instantiated on: " + this.gameObject);
            Destroy(this);
            return;
        }
        #endregion

        choiceButtonTexts = new TextMeshProUGUI[choiceButtons.Length];
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtonTexts[i] = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        }
        EndDialogue();
    }

    public void StartDialogue(DialogueContainer dialogue)
    {
        dialogueOpen = true;
        this.dialogue = dialogue;
        dialogueNode = dialogue.dialogueStart;
        DisplayGUI(dialogueNode);
    }

    public void MakeChoice(int index)
    {
        DialogueNodeData nextDialogue = dialogue.GetNextNodesFromPreviousNodeID(dialogueNode.GUID)[index];

        dialogueNode = nextDialogue;
        DisplayGUI(nextDialogue);
    }


    private void DisplayGUI(DialogueNodeData input)
    {
        mainPanel.SetActive(true);
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].SetActive(false);
        }

        List<string> choiceTexts = dialogue.GetChoiceTextsFromNode(input.GUID); 

        for (int i = 0; i < choiceTexts.Count; i++)
        {
            DrawChoice(choiceTexts[i], i);
        }
        exitButton.SetActive(choiceTexts.Count == 0);

        eventText.text = input.dialogueText;
    }

    private void DrawChoice(string choice, int choiceIndex)
    {
        bool isAllowed = true;

        if (isAllowed)
        {
            choiceButtons[choiceIndex].GetComponent<Button>().interactable = true;
            choiceButtons[choiceIndex].SetActive(true);
            choiceButtonTexts[choiceIndex].text = choice;
        }
    }

    public void EndDialogue()
    {
        dialogueNode = null;
        dialogue = null;
        mainPanel.SetActive(false);
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].SetActive(false);
        }
        exitButton.SetActive(false);
        dialogueOpen = false;
    }
}
