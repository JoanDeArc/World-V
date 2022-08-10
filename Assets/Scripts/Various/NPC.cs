using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable
{
    public bool Friendly;

    public Dialogue Dialogue;

    public Dialogue PreCombatDialogue;
    public Dialogue AfterCombatDialogue;

    public NavAgentAnimationControl animationControl;

    // Start is called before the first frame update
    void Start()
    {
        animationControl = GetComponent<NavAgentAnimationControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animationControl)
            animationControl.Animate();
    }

    public override void Trigger()
    {
        if (Friendly)
            FindObjectOfType<DialogueManager>().StartDialogue(Dialogue);
        else
        {
            if (PreCombatDialogue != null)
                StartCoroutine(TriggerBattleWithDialogue());
            else
                TriggerBattle();
        }
    }

    public IEnumerator TriggerBattleWithDialogue()
    {
        DialogueManager dManager = FindObjectOfType<DialogueManager>();
        Player player = FindObjectOfType<Player>();
        dManager.StartDialogue(PreCombatDialogue);
        while (player.IsInDialogue)
            yield return null;
        FindObjectOfType<BattleManager>().StartBattle(gameObject);
    }

    public void TriggerBattle()
    {
        FindObjectOfType<BattleManager>().StartBattle(gameObject);
    }
}
