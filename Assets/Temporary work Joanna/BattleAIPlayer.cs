using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleAIPlayer : BattleAINew
{
    private BattleCanvasHandler bch;
    private bool DPADDown;
    private int attackChoiceIndex;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        bch = FindObjectOfType<BattleCanvasHandler>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if ((!doingAction || currentState == CombatState.idle || currentState == CombatState.reposition) && currentState != CombatState.attackRange && currentState != CombatState.defend)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                if (!bch.AttackBar.activeSelf)
                    bch.ToggleAttackBar();
                else
                {
                    bch.ToggleAttackBar();
                    if (currentRoutine != null)
                        StopCoroutine(currentRoutine);
                    doingAction = false;
                    agent.ResetPath();
                    currentState = CombatState.attackRange;
                }
            }
            else if (Input.GetAxis("DPAD LeftRight") != 0 && !DPADDown)
            {
                DPADDown = true;
                if (bch.AttackBar.activeSelf)
                {
                    if (Input.GetAxis("DPAD LeftRight") > 0)
                        attackChoiceIndex += 1;
                    else
                        attackChoiceIndex -= 1;
                }
                if (attackChoiceIndex < 0)
                    attackChoiceIndex = 2;
                else if (attackChoiceIndex > 2)
                    attackChoiceIndex = 0;

                bch.SelectAttack(attackChoiceIndex);
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                if (bch.AttackBar.activeSelf)
                {
                    bch.ToggleAttackBar();
                }
                else if (GetDefenceChance(DefenceType1) > 0)
                {
                    if (currentRoutine != null)
                        StopCoroutine(currentRoutine);
                    doingAction = false;
                    agent.ResetPath();
                    ActiveDefenceType = DefenceType1;
                    currentState = CombatState.defend;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button2) && GetDefenceChance(DefenceType2) > 0 && !bch.AttackBar.activeSelf)
            {
                if (currentRoutine != null)
                    StopCoroutine(currentRoutine);
                doingAction = false;
                agent.ResetPath();
                ActiveDefenceType = DefenceType2;
                currentState = CombatState.defend;
            }
        }
        if (Input.GetAxis("DPAD LeftRight") == 0)
        {
            DPADDown = false;
        }
    }

    protected override void MakeNewDecision()
    {
        randomDir = Random.insideUnitCircle;

        agent.ResetPath();

        CombatState[] states = { CombatState.idle, CombatState.reposition };
        currentState = states[Random.Range(0, 2)];

        NavMeshHit hit;
        bool blocked = NavMesh.Raycast(transform.position, transform.position + Vector3.Normalize((transform.position - Target.transform.position)) / 2 + new Vector3(randomDir.x * 10, transform.position.y * 10, randomDir.y * 10), out hit, NavMesh.AllAreas);
        destination = hit.position;

        needsDecision = false;
    }

    // State machine that is influenced by player input
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

                    break;
                }
            case CombatState.reposition:
                {
                    doingAction = true;
                    agent.SetDestination(destination);

                    if (!IsInsideBattleBounds(transform.position - (transform.position - destination).normalized * 1.5f))
                    {
                        animationControl.LookTowards(Target.transform.position);
                        needsDecision = true;
                        doingAction = false;
                        break;
                    }

                    if (IsAtDestination(transform.position))
                    {
                        animationControl.LookTowards(Target.transform.position);
                        needsDecision = true;
                        doingAction = false;
                    }

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