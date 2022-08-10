using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EventManager : MonoBehaviour
{
    [SerializeField]
    public Event[] events;

    private Event currentEvent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExecuteEvent(int id)
    {
        currentEvent = events[id];
        StartCoroutine(ParseEvent(currentEvent.Commands));
    }

    private IEnumerator ParseEvent(string commands)
    {
        foreach (string command in commands.Split('\n'))
        {
            yield return ParseCommand(command);
        }
    }

    private IEnumerator ParseCommand(string command)
    {
        if (command.Contains("disable"))
            FindObjectOfType<Player>().MovementDisabled = true;
        else if (command.Contains("enable"))
            FindObjectOfType<Player>().MovementDisabled = false;
        else if (command.Contains("setactive"))
            yield return StartCoroutine(SetActive(GetActor(command.Split(' ')[1]), command.Split(' ')[2] == "true"));
        else if (command.Contains("cameratarget"))
            yield return StartCoroutine(SetCameraTarget(GetActor(command.Split(' ')[1])));
        else if (command.Contains("turnto")) // turnto actor target
            yield return StartCoroutine(TurnTo(GetActor(command.Split(' ')[1]), GetActor(command.Split(' ')[2])));
        else if (command.Contains("walkto")) // walkto actor target
            yield return StartCoroutine(WalkTo(GetActor(command.Split(' ')[1]), GetActor(command.Split(' ')[2])));
        else if (command.Contains("startdialogue")) // startdialogue actor dialogueIndex
            yield return StartCoroutine(StartDialogue(GetActor(command.Split(' ')[1]), int.Parse(command.Split(' ')[2])));
        else if (command.Contains("startbattle")) // startbattle enimon
            yield return StartCoroutine(StartBattle(GetActor(command.Split(' ')[1])));
        else if (command.Contains("playanimation")) // usage: playanimation actor clipName optionalSpeed
        {
            string[] c = command.Split(' ');
            if (c.Length == 3)
                yield return StartCoroutine(PlayAnimation(GetActor(command.Split(' ')[1]), command.Split(' ')[2]));
            if (c.Length == 4)
                yield return StartCoroutine(PlayAnimation(GetActor(command.Split(' ')[1]), command.Split(' ')[2], float.Parse(command.Split(' ')[3])));
        }


        yield return null;
    }

    private GameObject GetActor(string name)
    {
        foreach (GameObject actor in currentEvent.Characters)
            if (actor.name == name)
                return actor;
        return null;
    }

    private IEnumerator SetActive(GameObject actor, bool active)
    {
        actor.SetActive(active);
        yield return null;
    }

    private IEnumerator SetCameraTarget(GameObject actor)
    {
        CinemachineCore.Instance.GetVirtualCamera(0).GetComponent<CinemachineVirtualCamera>().LookAt = actor.transform;
        yield return null;
    }

    private IEnumerator TurnTo(GameObject actor, GameObject target)
    {
        actor.transform.LookAt(new Vector3(target.transform.position.x, actor.transform.position.y, target.transform.position.z));
        yield return null;
    }

    private IEnumerator WalkTo(GameObject actor, GameObject target)
    {
        actor.GetComponent<NavMeshAgent>().SetDestination(target.transform.position);
        while (Vector3.Distance(actor.transform.position, target.transform.position) > actor.GetComponent<NavMeshAgent>().stoppingDistance) yield return null;
        FindObjectOfType<Player>().MovementDisabled = false;
    }

    private IEnumerator StartDialogue(GameObject actor, int dialogueIndex)
    {
        actor.GetComponent<Interactable>().TriggerDialogue();//.dialogues[dialogueIndex]);
        while (FindObjectOfType<Player>().IsInDialogue) yield return null;
    }

    private IEnumerator StartBattle(GameObject target)
    {
        FindObjectOfType<BattleManager>().StartBattle(target);
        yield return null;
    }

    private IEnumerator PlayAnimation(GameObject actor, string animationName)
    {
        Animator anim = actor.GetComponent<Animator>();
        anim.Play(animationName);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length * 1.5f);
    }

    private IEnumerator PlayAnimation(GameObject actor, string animationName, float speed)
    {
        Animator anim = actor.GetComponent<Animator>();
        anim.SetFloat("Direction", speed);
        anim.Play(animationName, -1, 1f);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length * 1.5f);
    }
}
