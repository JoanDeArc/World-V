using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CombatState { none, idle, reposition, chase, runAway, keepDistance, random, attackClose, attackRange, defend };
public enum DefenceType { brace, evade, block, reflect };

public class BattleAINew : MonoBehaviour
{
    public BattleAINew Target;

    public float Health = 100;
    public float Energy = 100;
    public float EnergyRechargeRate = 1;

    public float UltiCharge = 0;

    //public float EvadeEfficency = 0.5f;
    public float BraceEfficency = 0.5f;
    public float BlockEfficency = 0.5f;
    public float ReflectEfficency = 0.5f;
    public float EvadeChance = 1;
    public float BraceChance = 1;
    public float BlockChance = 1;
    public float ReflectChance = 1;

    public CombatState currentState;
    protected bool needsDecision;
    protected bool doingAction;
    protected Coroutine currentRoutine;

    public Attack Attack1, Attack2, Attack3;

    private Transform rangedAttackPoint;

    public DefenceType DefenceType1, DefenceType2;
    public DefenceType ActiveDefenceType;

    protected Vector2 randomDir;
    protected Vector3 destination;

    protected NavMeshAgent agent;
    private Animator animator;
    protected NavAgentAnimationControl animationControl;
    public float preferedDistance;

    private float rangedAttackAnimDelay;
    private float braceAnimDelay;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        animationControl = GetComponent<NavAgentAnimationControl>();

        rangedAttackPoint = transform.Find("Ranged Attack Point");

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "Armature|AttackRange")
                rangedAttackAnimDelay = clip.length;
            else if (clip.name == "Armature|DefendTwo")
                braceAnimDelay = clip.length;
        }

        needsDecision = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        RunStateMachine();

        animationControl.Animate();

        if (Energy < 100)
            Energy = Mathf.Min(Energy + (EnergyRechargeRate * Time.deltaTime), 100);
    }

    public void TakeDamage(float damage)
    {
        if (currentState == CombatState.defend)
        {
            switch (ActiveDefenceType)
            {
                case DefenceType.evade:
                    Health -= damage;
                    EvadeChance += 0.03f;
                    break;
                case DefenceType.brace:
                    Health -= damage * (1 - BraceEfficency);
                    BraceChance += 0.05f;
                    break;
                case DefenceType.block:
                    Health -= damage * (1 - BlockEfficency);
                    BlockChance += 0.1f;
                    break;
                case DefenceType.reflect:
                    Health -= damage * (1 - ReflectEfficency);
                    ReflectChance += 0.08f;
                    break;
            }
            UltiCharge += 0.2f;
        }
        else
        {
            Health -= damage;
            UltiCharge += 0.1f;
        }
    }

    public float GetDefenceChance(DefenceType type)
    {
        switch (type)
        {
            case DefenceType.evade:
                return EvadeChance;
            case DefenceType.brace:
                return BraceChance;
            case DefenceType.block:
                return BlockChance;
            case DefenceType.reflect:
                return ReflectChance;
        }
        return 0;
    }

    protected bool IsInsideBattleBounds(Vector3 point)
    {
        Vector3 viewportCheckPoint = Camera.main.WorldToViewportPoint(point);
        if (viewportCheckPoint.x < 0.05 || viewportCheckPoint.x > 0.95 || viewportCheckPoint.y < 0.05 || viewportCheckPoint.y > 0.95 || viewportCheckPoint.z < 0
            || Vector3.Distance(point, Camera.main.transform.position) > 20)
            return false;
        return true;
    }

    protected bool IsAtDestination(Vector3 position)
    {
        // need to also take view size into consideration?
        if (Vector3.Distance(new Vector3(position.x, 0, position.z), new Vector3(destination.x, 0, destination.z)) <= preferedDistance)
            return true;
        return false;
    }

    protected virtual void MakeNewDecision(){}

    protected virtual void RunStateMachine()
    {
        if (needsDecision)
            MakeNewDecision();
    }

    protected IEnumerator Idle()
    {
        doingAction = true;
        yield return new WaitForSeconds(Random.Range(1, 5));

        doingAction = false;
        needsDecision = true;
    }

    protected IEnumerator RangedAttackDelay(RangedAttack rAttack)
    {
        doingAction = true;
        animator.SetTrigger("attackRange");
        yield return new WaitForSeconds(rangedAttackAnimDelay);

        GameObject projectile = Instantiate(rAttack.Projectile, rangedAttackPoint.position, Quaternion.Euler(transform.position - Target.transform.position));
        projectile.GetComponent<Projectile>().SetValues(rAttack.Damage, rAttack.Speed, rAttack.Range);
        projectile.transform.forward = transform.forward;
        Energy -= 15;

        doingAction = false;
        needsDecision = true;
    }
    protected IEnumerator DefendDelay(DefenceType type)
    {
        doingAction = true;
        animationControl.LookTowards(Target.transform.position);

        animator.SetTrigger(type.ToString());

        switch (ActiveDefenceType)
        {
            case DefenceType.evade:
                EvadeChance -= 0.2f;
                yield return new WaitForSeconds(braceAnimDelay);
                break;
            case DefenceType.block:
                BlockChance -= 0.2f;
                yield return new WaitForSeconds(braceAnimDelay);
                break;
            case DefenceType.brace:
                BraceChance -= 0.2f;
                yield return new WaitForSeconds(braceAnimDelay);
                break;
            case DefenceType.reflect:
                ReflectChance -= 0.2f;
                yield return new WaitForSeconds(braceAnimDelay);
                break;
        }

        doingAction = false;
        needsDecision = true;
    }
}