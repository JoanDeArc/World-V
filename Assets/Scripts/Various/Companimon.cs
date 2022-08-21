using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Companimon : MonoBehaviour
{
    public Transform Player;

    public float MovementSpeed = 7;
    public bool followPlayer;

    public int HP;
    public int MP;
    public int Attack;
    public int Defence;
    public int Speed;
    public int AttunementFire, AttunementWater, AttunementAir, AttunementEarth;

    public float Hunger = 100;
    public float Metabolism = 1;
    public Text TemporaryHungerMeter;

    private NavMeshAgent agent;
    private NavAgentAnimationControl animationControl;

    public static Companimon instance;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        agent = GetComponent<NavMeshAgent>();
        animationControl = GetComponent<NavAgentAnimationControl>();

        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.enabled)
        {
            animationControl.Animate();
        }
        if (followPlayer)
        {
            agent.SetDestination(Player.position);

            Quaternion rotation = Quaternion.LookRotation(Player.transform.position - transform.position);
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
            transform.rotation = rotation;
        }

        Hunger -= Metabolism * Time.deltaTime;
        TemporaryHungerMeter.text = "Hunger: " + Hunger.ToString("0");
    }

    public bool Feed(ItemData item)
    {    
        if (item == null || item.Tag != ItemTag.edible)
            return false;

        // Play eat animation

        Hunger += item.FillAmount;
        return true;
    }

    public void Warp(Vector3 position)
    {
        agent.Warp(position);
    }

}
