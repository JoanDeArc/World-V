using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleAICPU : BattleAINew
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void MakeNewDecision()
    {
        randomDir = Random.insideUnitCircle;

        agent.ResetPath();

        CombatState[] states = { CombatState.idle, CombatState.reposition, CombatState.attackRange };
        currentState = states[Random.Range(0, 3)];

        NavMeshHit hit;
        bool blocked = NavMesh.Raycast(transform.position, transform.position + Vector3.Normalize((transform.position - Target.transform.position)) / 2 + new Vector3(randomDir.x * 10, transform.position.y * 10, randomDir.y * 10), out hit, NavMesh.AllAreas);
        destination = hit.position;

        needsDecision = false;
    }

    protected override void RunStateMachine()
    {
        base.RunStateMachine();

        switch (currentState)
        {
            case CombatState.idle:
                {
                    animationControl.LookTowards(Target.transform.position);
                    if (!doingAction)
                        currentRoutine = StartCoroutine(Idle());

                    if (Target.currentState == CombatState.attackRange)
                    {
                        StopCoroutine(currentRoutine);
                        doingAction = false;
                        currentState = CombatState.defend;

                        DefenceType[] defences = { DefenceType1, DefenceType2 };
                        ActiveDefenceType = defences[Random.Range(0, defences.Length)];
                    }

                    break;
                }
            case CombatState.reposition:
                {
                    agent.SetDestination(destination);

                    if (!IsInsideBattleBounds(transform.position - (transform.position - destination).normalized * 1.5f))
                    {
                        animationControl.LookTowards(Target.transform.position);
                        needsDecision = true;
                        break;
                    }

                    if (IsAtDestination(transform.position))
                    {
                        animationControl.LookTowards(Target.transform.position);
                        needsDecision = true;
                    }

                    break;
                }
            case CombatState.chase:
                {
                    //agent.SetDestination(Target.transform.position);

                    break;
                }
            case CombatState.runAway:
                {
                    // raycast to a random point opposite of companimon, in a 200? degree arc. If valid, go there, otherwise try again, x amount of tries?
                    /*NavMeshHit hit;
                    bool blocked = NavMesh.Raycast(transform.position, transform.position + (transform.position - Target.transform.position), out hit, NavMesh.AllAreas);
                    Debug.DrawLine(transform.position, Target.transform.position, blocked ? Color.red : Color.green);

                    agent.SetDestination(hit.position);
                    */
                    break;
                }
            case CombatState.keepDistance:
                {
                    /*if (Vector3.Distance(transform.position, Target.transform.position) * 1.1f < preferedDistance)
                    {
                        NavMeshHit hit;
                        bool blocked = NavMesh.Raycast(transform.position, transform.position + Vector3.Normalize((transform.position - Target.transform.position)) / 2 + new Vector3(randomDir.x, transform.position.y, randomDir.y), out hit, NavMesh.AllAreas);

                        Debug.DrawLine(hit.position, hit.position + Vector3.up, Color.magenta);
                        Vector3 viewportCheckPoint = Camera.main.WorldToViewportPoint(hit.position);
                        if (viewportCheckPoint.x < 0.05 || viewportCheckPoint.x > 0.95 || viewportCheckPoint.y < 0.05 || viewportCheckPoint.y > 0.95 || viewportCheckPoint.z < 0
                            || Vector3.Distance(hit.position, Camera.main.transform.position) > 20)
                        {
                            Vector3 enemyDirection = -Vector3.Normalize((transform.position - Target.transform.position));
                            randomDir = Random.insideUnitCircle + new Vector2(enemyDirection.x, enemyDirection.z);
                            break;
                        }

                        agent.SetDestination(hit.position);
                    }
                    else if (Vector3.Distance(transform.position, Target.transform.position) * 0.9f > preferedDistance)
                        agent.SetDestination(Target.transform.position);
                    else
                        agent.ResetPath();*/

                    break;
                }
            case CombatState.attackRange:
                {
                    if (!doingAction)
                        StartCoroutine(RangedAttackDelay((RangedAttack)Attack1));

                    animationControl.LookTowards(Target.transform.position);

                    break;
                }
            case CombatState.defend:
                {
                    if (!doingAction)
                        StartCoroutine(DefendDelay(ActiveDefenceType));

                    break;
                }
        }
    }
}
