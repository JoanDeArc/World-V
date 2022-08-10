using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    //public Dialogue Dialogue;

    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Trigger()
    {
        //TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        //if (Dialogue == null)
          //  return;
        //FindObjectOfType<DialogueManager>().StartDialogue(Dialogue);
    }

    public void TriggerDialogue(Dialogue dialogue)
    {
       // if (dialogue == null)
        //    return;
        //FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
