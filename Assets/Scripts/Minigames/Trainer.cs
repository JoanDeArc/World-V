using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainer : Interactable
{
    public enum Element { Fire, Water, Air, Earth };
    public Element TargetElement;

    private DialogueWithAnswer dialogue;
    private DialogueWithAnswer dialogueTutorial;

    public TrainingGame Game;
    public int DifficultyLevel;

    // Start is called before the first frame update
    void Start()
    {
        dialogue = new DialogueWithAnswer();
        dialogue.Sentences = new SentenceWrapper[1];
        dialogue.Sentences[0] = new SentenceWrapper(("", "Start training?"));
        dialogue.Answers = new string[] { "Start", "Info", "Exit" };

        dialogueTutorial = new DialogueWithAnswer();
        dialogueTutorial.Sentences = new SentenceWrapper[1];
        dialogueTutorial.Sentences[0] = new SentenceWrapper(("", Game.InfoString));
        dialogueTutorial.Answers = new string[] { "Back" };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Trigger()
    {
        Train();
    }

    private void Train()
    {
        StartCoroutine(FindObjectOfType<DialogueManager>().StartDialogueAnswer(dialogue, answer => 
        {
            if (answer == 0)
            {
                Game.gameObject.SetActive(true);
                StartCoroutine(Game.StartGame(DifficultyLevel));
            }
            else if (answer == 1)
            {
                StartCoroutine(FindObjectOfType<DialogueManager>().StartDialogueAnswer(dialogueTutorial, answer =>
                {
                    Train();
                }));
            }
        }));
    }

    private void IncreaseElement()
    {
        Companimon companimon = FindObjectOfType<Companimon>();
        switch (TargetElement)
        {
            case Element.Fire:
                companimon.AttunementFire += 5;
                break;
            case Element.Water:
                companimon.AttunementWater += 5;
                break;
            case Element.Air:
                companimon.AttunementAir += 5;
                break;
            case Element.Earth:
                companimon.AttunementEarth += 5;
                break;
        }
    }
}
