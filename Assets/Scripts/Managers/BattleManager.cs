using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{

    public GameObject Player;
    public GameObject Companimon;



    public Animator AnimatorBattle;
    public Animator AnimatorWon;
    public Transform BlackoutPanel;
    bool battleActive;

    private GameObject enimon;

    private CinemachineVirtualCamera currentCamera;

    public GameObject CanvasBattle;
    public Text TextSlot1;
    public Text TextSlot2;

    public float DefencePressTime;
    public float AttackPressTime;

    private bool defencePressBlocked;
    private bool attackPressBlocked;

    private float blockTime = 1;
    private float defenceBlockCooldown;
    private float attackBlockCooldown;

    public float DefenceTimeStamp;
    public float AttackTimeStamp;

    public float MegaAttackCharge;
    public Slider MegaAttackSlider;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //TextSlot1.text = ((int)(Companimon.GetComponent<CombatAI>().attackSlot1CDLeft)).ToString();
        //TextSlot2.text = ((int)(Companimon.GetComponent<CombatAI>().attackSlot2CDLeft)).ToString();


        HandleUserInput();
    }

    private void HandleUserInput()
    {
        if (Input.GetKeyDown(KeyCode.R) && !attackPressBlocked)
        {
            AttackPressTime = Time.time;
            attackPressBlocked = true;
            attackBlockCooldown = blockTime;

        }
        if (Input.GetKeyDown(KeyCode.T) && !defencePressBlocked)
        {
            DefencePressTime = Time.time;
            defencePressBlocked = true;
            defenceBlockCooldown = blockTime;

            StopCoroutine(MissDefence());
            ChargeMegaAttack(Mathf.Abs(DefencePressTime - DefenceTimeStamp));
        }

        if (attackBlockCooldown > 0)
        {
            attackBlockCooldown -= Time.deltaTime;
            if (attackBlockCooldown <= 0)
                attackPressBlocked = false;
        }
        if (defenceBlockCooldown > 0)
        {
            defenceBlockCooldown -= Time.deltaTime;
            if (defenceBlockCooldown <= 0)
                defencePressBlocked = false;
        }

        
    }

    public void StartBattle(GameObject enimon, BattleSpace battleSpace = null)
    {
        battleActive = true;
        this.enimon = enimon;

        MegaAttackCharge = 0;
        StartCoroutine(BattlePrelude());
    }

    private IEnumerator BattlePrelude()
    {
        currentCamera = CinemachineCore.Instance.GetVirtualCamera(0).GetComponent<CinemachineVirtualCamera>();
        currentCamera.enabled = false;

        Player.GetComponent<Player>().MovementDisabled = true;


        // Run to closest? edge of screen
        Player.GetComponent<NavMeshAgent>().enabled = true;
        Player.GetComponent<NavMeshAgent>().SetDestination(Player.transform.position + new Vector3(10, 0, 0));

        Companimon.GetComponent<Companimon>().followPlayer = false;


        while (Player.GetComponent<NavMeshAgent>().pathPending || Player.GetComponent<NavMeshAgent>().remainingDistance > 0)
            yield return null;
        yield return new WaitForSeconds(0.3f);


         // Look between mons
        Player.transform.LookAt((Companimon.transform.position + enimon.transform.position) / 2);


        AnimatorBattle.SetBool("IsShown", true);
        yield return new WaitForSeconds(2);
        AnimatorBattle.SetBool("IsShown", false);


        Companimon.transform.Find("Canvas Healthbar").gameObject.SetActive(true);
        enimon.transform.Find("Canvas Healthbar").gameObject.SetActive(true);
        CanvasBattle.SetActive(true);



        Companimon.GetComponent<CombatAI>().Initiate(enimon);
        enimon.GetComponent<CombatAI>().Initiate(Companimon);
    }

  
    public void EndBattle()
    {
        StartCoroutine(EndBattleCo());
    }

    public void SetDefenceTimeStamp(float time)
    {
        DefenceTimeStamp = time;

        ChargeMegaAttack(Mathf.Abs(DefencePressTime - DefenceTimeStamp));

        StopCoroutine(MissDefence());
        StartCoroutine(MissDefence());
    }

    public void SetAttackTimeStamp(float time)
    {
        AttackTimeStamp = time;
    }

    private IEnumerator MissDefence()
    {
        yield return new WaitForSeconds(0.5f);
        if (Mathf.Abs(DefencePressTime - Time.time) > 0.5f)
            ChargeMegaAttack(0.07f);
        DefenceTimeStamp = 0;
    }

    private void ChargeMegaAttack(float difference)
    {
        if (difference > 0.5f)
            return;
        else
        {
            //Debug.Log("charge: " + difference + "    " + Time.time);
            if (difference < 0.07f)
                difference = 0.07f;
            MegaAttackCharge += difference;
            MegaAttackSlider.value = MegaAttackCharge;
        }
        DefenceTimeStamp = 0;
    }

    private IEnumerator EndBattleCo()
    {
        battleActive = false;


        Companimon.GetComponent<CombatAI>().Terminate();
        enimon.GetComponent<CombatAI>().Terminate();

        AnimatorWon.SetBool("IsShown", true);
        yield return new WaitForSeconds(2);
        AnimatorWon.SetBool("IsShown", false);

        Companimon.transform.Find("Canvas Healthbar").gameObject.SetActive(false);
        enimon.transform.Find("Canvas Healthbar").gameObject.SetActive(false);
        CanvasBattle.SetActive(false);


        Player.GetComponent<NavMeshAgent>().enabled = false;
        Player.GetComponent<Player>().MovementDisabled = false;

        //enimon.GetComponent<NPC>().TriggerDialogue(enimon.GetComponent<NPC>().AfterCombatDialogue);

        while (Player.GetComponent<Player>().IsInDialogue)
            yield return null;
        currentCamera.enabled = true;

        Companimon.GetComponent<NavMeshAgent>().enabled = true;
        Companimon.GetComponent<Companimon>().followPlayer = true;
    }
}
