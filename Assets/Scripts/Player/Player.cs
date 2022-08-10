using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    public Companimon Companimon;

    private Backpack backpack;
    

    private CharacterController controller;
    private Animator animator;
    public float MovementSpeed = 4;

    public bool MovementDisabled;
    public bool IsInDialogue;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        backpack = new Backpack(10);

        controller = gameObject.GetComponent<CharacterController>();
        animator = gameObject.GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!MovementDisabled)
        {
            HandleMovement();
        }
        if (IsInDialogue)
        {
            HandleDialogue();
        }
        else
        {
            if (Input.GetButtonDown("Submit"))
            {
                Collider[] hitColliders = Physics.OverlapBox(transform.position + transform.forward * 1.5f, Vector3.one);
                if (hitColliders.Length > 0)
                {
                    foreach (Collider c in hitColliders)
                    {
                        if (c.gameObject.tag == "Interactable")
                        {
                            MovementDisabled = true;
                            animator.SetBool("IsMoving", false);
                            c.gameObject.GetComponent<Interactable>().Trigger();
                        }
                        else if (c.gameObject.tag == "Item")
                        {
                            Item item = c.gameObject.GetComponent<Item>();
                            if (backpack.TryAddItem(item))
                            {
                                FindObjectOfType<DialogueManager>().StartDialogue(item, true);
                                Destroy(c.gameObject);
                            }
                            else
                            {
                                FindObjectOfType<DialogueManager>().StartDialogue(item, false);
                            }
                        }
                        else if (c.gameObject.tag == "Companimon")
                        {
                            // Bring up companimon interract menu

                            // For now, just feed
                            Item item = backpack.HasItem(ItemTag.edible);
                            //Debug.Log(item.name);
                            if (Companimon.Feed(item))
                            {
                                backpack.RemoveItem(item);
                                Debug.Log("Yummy");
                            }
                            else
                            {
                                Debug.Log("Angry mon!");
                            }
                        }
                    }
                }
            }

            else if (Input.GetButtonDown("Inventory"))
            {
                List<Item> items = backpack.GetItems();
                Debug.Log("Inventory items:");
                foreach (Item item in items)
                    Debug.Log(item.Name);
            }
            else if (Input.GetButtonDown("Menu")) // currently just shows companimon stats
            {
                Debug.Log("HP: " + Companimon.HP + "  MP: " + Companimon.MP + "  Attack: " + Companimon.Attack + "  Defence: " + Companimon.Defence + "  Speed: " + Companimon.Speed + "  Attunements F/W/A/E: " + Companimon.AttunementFire + "/" + Companimon.AttunementWater + "/" + Companimon.AttunementAir + "/" + Companimon.AttunementEarth);
            }
            
        }
    }

    private void HandleDialogue()
    {
        if (Input.GetButtonDown("Submit"))
            FindObjectOfType<DialogueManager>().StepForwardDialogue();

        else if (Input.GetButtonDown("Up"))
        {
            FindObjectOfType<DialogueManager>().AddAnswerIndex(-1);
        }
        else if (Input.GetButtonDown("Down"))
        {
            FindObjectOfType<DialogueManager>().AddAnswerIndex(1);
        }
    }

    private void HandleMovement()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        var camera = Camera.main;

        //camera forward and right vectors:
        var forward = camera.transform.forward;
        var right = camera.transform.right;

        //project forward and right vectors on the horizontal plane (y = 0)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        //this is the direction in the world space we want to move:
        var desiredMoveDirection = forward * verticalAxis + right * horizontalAxis;


        //Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(desiredMoveDirection * Time.deltaTime * MovementSpeed);

        if (desiredMoveDirection != Vector3.zero)
        {
            animator.SetBool("IsMoving", true);
            gameObject.transform.forward = desiredMoveDirection;
        }
        else
            animator.SetBool("IsMoving", false);
    }
}
