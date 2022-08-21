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

    public CompanimonThoughtHandler ThoughtHandler;
    private Thought LastThought;
    private float TimeLastThought;

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

        HandleThoughts();

        TemporaryHungerMeter.text = "Hunger: " + Hunger.ToString("0");
    }

    private void HandleThoughts()
    {
        if (TimeLastThought > Time.time - 10)
            return;

        List<Thought> thoughts = new List<Thought>();

        if (Hunger < 90)
            thoughts.Add(Thought.Hungry);

        thoughts.Add(Thought.Sad);

        if (thoughts.Count == 0)
            return;

        if (thoughts.Count > 1)
            thoughts.Remove(LastThought);

        int choice = Random.Range(0, thoughts.Count);
        LastThought = thoughts[choice];
        ThoughtHandler.ShowThought(thoughts[choice]);

        TimeLastThought = Time.time;
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
