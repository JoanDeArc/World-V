using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum CombatPersonality { aggressive, tester }
public enum CombatState { none, idle, chase, runAway, keepDistance, random, attackClose, attackRange };

public class CombatAI : MonoBehaviour
{
    public float Health;
    public float MaxHealth;

    public CombatPersonality CombatPersonality;
    [HideInInspector]
    public CombatState currentState;
    private bool didAttack;
    private bool deciding;

    private GameObject target;
    private NavMeshAgent agent;
    private Animator animator;

    public GameObject CloseAttackEffect;
    private Transform closeAttackPoint;

    public GameObject Projectile;
    public float RangedAttackAnimDelay;
    public float CloseAttackAnimDelay;
    private Transform rangedAttackPoint;

    public float preferedDistance;

    public Canvas HealthbarCanvas;
    public Image HealthbarSlideImage;

    Vector2 randomDir;

    public Attack attackSlot1;
    public float attackSlot1CDLeft;

    public Attack attackSlot2;
    public float attackSlot2CDLeft;

    bool needsDecision;

    private Attack currentAttack;

    public Text DamageText;
    private List<Text> damageTexts;
    //temp
    public Text statusText;

    // Start is called before the first frame update
    void Start()
    {
        MaxHealth = Health;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        closeAttackPoint = transform.Find("Close Point");
        rangedAttackPoint = transform.Find("Ranged Point");

        damageTexts = new List<Text>();

        Random.InitState(System.DateTime.Now.Millisecond);

    }

    public void Initiate(GameObject target)
    {
        enabled = true;
        this.target = target;

        attackSlot1CDLeft = attackSlot1.Cooldown / 2;
        attackSlot2CDLeft = attackSlot2.Cooldown / 2;
        needsDecision = true;
    }

    public void Terminate()
    {
        StopAllCoroutines();
        enabled = false;
        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (attackSlot1CDLeft > 0)
            attackSlot1CDLeft -= Time.deltaTime;
        if (attackSlot2CDLeft > 0)
            attackSlot2CDLeft -= Time.deltaTime;

        RunStateMachine();

        for (int i = 0; i < damageTexts.Count; i++)
        {
            if (damageTexts[i] != null)
                damageTexts[i].transform.Translate(new Vector3(0, 0.1f, 0) * Time.deltaTime);
            else
            {
                damageTexts.RemoveAt(i);
                i--;
            }
        }

        HealthbarCanvas.transform.LookAt(Camera.main.transform.position);
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(TakeDamageEnumerator(damage));
    }

    private IEnumerator TakeDamageEnumerator(float damage)
    {
        if (transform.tag == "Companimon")
        {
            FindObjectOfType<BattleManager>().SetDefenceTimeStamp(Time.time);
        }
        else
        {
            FindObjectOfType<BattleManager>().SetAttackTimeStamp(Time.time);
            yield return new WaitForSeconds(0.5f);

            float timeDifference = Mathf.Abs(FindObjectOfType<BattleManager>().AttackPressTime - Time.time);
            if (timeDifference < 0.5)
                damage *= (1 + timeDifference);
            //Debug.Log(timeDifference + "     " + damage);
        }

        Health -= damage;
        SpawnDamageText(damage);

        if (Health <= 0)
        {
            Health = 0;
            FindObjectOfType<BattleManager>().EndBattle();
        }
        HealthbarSlideImage.transform.localScale = new Vector3(Health / MaxHealth, 1, 1);

        yield return null;
    }

    private void SpawnDamageText(float number)
    {
        Text newText = Instantiate(DamageText, HealthbarCanvas.transform);

        newText.text = number.ToString();
        newText.enabled = true;

        damageTexts.Add(newText);

        Destroy(newText, 3);
    }


    private void RunStateMachine()
    {
        if (target == null)
            return;

        //Debug.Log(needsDecision);

        if (needsDecision)
            DecideState();
       // if (!deciding)
         //   StartCoroutine(DecideState());

        switch (currentState)
        {
            case CombatState.idle:
                {
                    agent.ResetPath();
                    break;
                }
            case CombatState.chase:
                {
                    agent.SetDestination(target.transform.position);

                    statusText.text = "Chasing";
                    break;
                }
            case CombatState.runAway:
                {
                    // raycast to a random point opposite of companimon, in a 200? degree arc. If valid, go there, otherwise try again, x amount of tries?
                    NavMeshHit hit;                   
                    bool blocked = NavMesh.Raycast(transform.position, transform.position + (transform.position - target.transform.position), out hit, NavMesh.AllAreas);            
                    Debug.DrawLine(transform.position, target.transform.position, blocked ? Color.red : Color.green);

                    agent.SetDestination(hit.position);

                    statusText.text = "Running away";
                    break;
                }
            case CombatState.keepDistance:
                {
                    statusText.text = "Keeping distance";
                    if (Vector3.Distance(transform.position, target.transform.position) * 1.1f < preferedDistance)
                    {
                        NavMeshHit hit;
                        bool blocked = NavMesh.Raycast(transform.position, transform.position + Vector3.Normalize((transform.position - target.transform.position)) / 2 + new Vector3(randomDir.x, transform.position.y, randomDir.y), out hit, NavMesh.AllAreas);

                        Debug.DrawLine(hit.position, hit.position + Vector3.up, Color.magenta);
                        Vector3 viewportCheckPoint = Camera.main.WorldToViewportPoint(hit.position);
                        if (viewportCheckPoint.x < 0.05 || viewportCheckPoint.x > 0.95 || viewportCheckPoint.y < 0.05 || viewportCheckPoint.y > 0.95 || viewportCheckPoint.z < 0
                            || Vector3.Distance(hit.position, Camera.main.transform.position) > 20)
                        {
                            Vector3 enemyDirection = -Vector3.Normalize((transform.position - target.transform.position));
                            randomDir = Random.insideUnitCircle + new Vector2(enemyDirection.x, enemyDirection.z);
                            break;
                        }

                        agent.SetDestination(hit.position);
                    }
                    else if (Vector3.Distance(transform.position, target.transform.position) * 0.9f > preferedDistance)
                        agent.SetDestination(target.transform.position);
                    else
                        agent.ResetPath();

                    break;
                }
            case CombatState.random:
                {
                    statusText.text = "Random";
                    if (agent.remainingDistance > agent.stoppingDistance)
                        break;
                    NavMeshHit hit;
                    bool blocked = NavMesh.Raycast(transform.position, transform.position + new Vector3(Random.Range(-3, 4), transform.position.y, Random.Range(-3, 4)), out hit, NavMesh.AllAreas);
                    Debug.DrawLine(transform.position, target.transform.position, blocked ? Color.red : Color.green);

                    agent.SetDestination(hit.position);
                    break;
                }
            case CombatState.attackClose:
                {
                    statusText.text = "Attack Close";
                    if (Vector3.Distance(transform.position, target.transform.position) > 2)
                    {
                        agent.SetDestination(target.transform.position);
                        break;
                    }

                    agent.ResetPath();
                    StartCoroutine(CloseAttackDelay((MeleeAttack)currentAttack));
                    currentState = CombatState.idle;
                    didAttack = true;
                    break;
                }
            case CombatState.attackRange:
                {
                    statusText.text = "Attack Range";
                    agent.ResetPath();
                    StartCoroutine(RangedAttackDelay((RangedAttack)currentAttack));
                    currentState = CombatState.idle;
                    didAttack = true;
                    break;
                }
        }
    }

    private void DecideState()
    {

        deciding = true;
        // Create a new random direction for the new decision
        randomDir = Random.insideUnitCircle;
        switch (CombatPersonality)
        {
            case CombatPersonality.aggressive:
                {
                    if (didAttack || attackSlot1CDLeft > 0)
                    {
                        currentState = CombatState.keepDistance;
                        //yield return new WaitForSeconds(Random.RandomRange(2, 5));
                        didAttack = false;
                    }
                    //else if (Vector3.Distance(transform.position, target.transform.position) > 2f)
                    //{
                    //    currentState = CombatState.chase;
                    //}
                    else if (attackSlot1CDLeft <= 0)
                    {
                        attackSlot1CDLeft = attackSlot1.Cooldown;
                        currentState = CombatState.attackRange;
                        //yield return new WaitForSeconds(CloseAttackAnimDelay);
                    }
                    else
                    {

                    }
                    break;
                }
            case CombatPersonality.tester:
                {
                    if (attackSlot1CDLeft > 0 && attackSlot2CDLeft > 0)
                    {
                        currentState = CombatState.keepDistance;
                        needsDecision = false;
                        StartCoroutine(QueueDecisionIn(Random.Range(2, 5)));
                    }
                    else
                    {
                        Attack();
                        needsDecision = false;
                    }

                    break;
                }
        }
        //needsDecision = false;
        deciding = false;
        //yield return null;
    }

    private void Attack()
    {
        if (attackSlot1CDLeft <= 0 && attackSlot2CDLeft <= 0)
            currentAttack = Random.Range(0, 1) > 0.5f ? attackSlot1 : attackSlot2;
        else if (attackSlot1CDLeft <= 0)
            currentAttack = attackSlot1;
        else if (attackSlot2CDLeft <= 0)
            currentAttack = attackSlot2;
        

        if (currentAttack.IsRanged())
            currentState = CombatState.attackRange;
        else
            currentState = CombatState.attackClose;
    }

    private void ActivateGlobalCooldown()
    {
        if (currentAttack == attackSlot1)
            attackSlot1CDLeft = attackSlot1.Cooldown;
        else if (attackSlot1CDLeft <= 0)
            attackSlot1CDLeft = 1;

        if (currentAttack == attackSlot2)
            attackSlot2CDLeft = attackSlot2.Cooldown;
        else if (attackSlot2CDLeft <= 0)
            attackSlot2CDLeft = 1;
    }

    private IEnumerator QueueDecisionIn(float time)
    {

        yield return new WaitForSeconds(time);
        needsDecision = true;
    }

    private IEnumerator CloseAttackDelay(MeleeAttack attack)
    {
        GetComponent<NavAgentAnimationControl>().LookTowards(target.transform.position);
        animator.SetTrigger("AttackClose");
        yield return new WaitForSeconds(CloseAttackAnimDelay);

        if (target.GetComponent<Collider>().bounds.Contains(closeAttackPoint.position))
        {
            Instantiate(attack.VisualEffect, closeAttackPoint.position, Quaternion.identity);
            target.GetComponent<CombatAI>().TakeDamage(attack.Damage);
        }
        
        ActivateGlobalCooldown();

        needsDecision = true;
    }

    private IEnumerator RangedAttackDelay(RangedAttack attack)
    {
        GetComponent<NavAgentAnimationControl>().LookTowards(target.transform.position);
        animator.SetTrigger("AttackRange");
        yield return new WaitForSeconds(RangedAttackAnimDelay);

        GameObject projectile = Instantiate(attack.Projectile, rangedAttackPoint.position, transform.rotation);
        projectile.GetComponent<Projectile>().SetValues(attack.Damage, attack.Speed, attack.Range);
        projectile.transform.forward = transform.forward;

        ActivateGlobalCooldown();

        needsDecision = true;
    }
}
