using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    public Companimon Companimon;

    public Inventory inventory;
    

    private CharacterController controller;
    private Animator animator;
    public float MovementSpeed = 4;

    public bool MovementDisabled;
    public bool InputDisabled;
    public bool IsInDialogue;

    public static Player instance;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);


        controller = gameObject.GetComponent<CharacterController>();
        animator = gameObject.GetComponent<Animator>();

        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);
    }


    // Update is called once per frame
    async void Update()
    {
        if (!MovementDisabled)
        {
            HandleMovement();
        }
        if (IsInDialogue)
        {
            HandleDialogue();
        }
        else if (!InputDisabled)
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
                            break;
                        }
                        else if (c.gameObject.tag == "Item")
                        {
                            Item item = c.gameObject.GetComponent<Item>();
                            ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                            itemData.Init(item.GetData());
                            if (inventory.TryAddItem(itemData))
                            {
                                FindObjectOfType<DialogueManager>().StartDialogue(item, true);
                                Destroy(c.gameObject);
                            }
                            else
                            {
                                FindObjectOfType<DialogueManager>().StartDialogue(item, false);
                            }
                            break;
                        }
                        else if (c.gameObject.tag == "Companimon")
                        {
                            // Bring up companimon interract menu

                            // For now, just feed
                            await PauseForAction(FeedChooseItem());

                            break;
                        }
                    }
                }
            }

            else if (Input.GetButtonDown("Inventory"))
            {
                await PauseForAction(inventory.Open());
            }
            else if (Input.GetButtonDown("Menu")) // currently just shows companimon stats
            {
                Debug.Log("HP: " + Companimon.HP + "  MP: " + Companimon.MP + "  Attack: " + Companimon.Attack + "  Defence: " + Companimon.Defence + "  Speed: " + Companimon.Speed + "  Attunements F/W/A/E: " + Companimon.AttunementFire + "/" + Companimon.AttunementWater + "/" + Companimon.AttunementAir + "/" + Companimon.AttunementEarth);
            }
            
        }
    }

    async Task PauseForAction(Task methodName)
    {
        MovementDisabled = true;
        InputDisabled = true;

        await methodName;

        MovementDisabled = false;
        InputDisabled = false;
    }

    async Task FeedChooseItem()
    {
        ItemData itemData = await inventory.OpenChooseType(typeof(Edible));

        Feed(itemData);
    }

    public void Feed(ItemData itemData)
    {
        if (Companimon.Feed(itemData))
        {
            inventory.RemoveItem(itemData);
            Debug.Log("Yummy");
        }
        else
        {
            Debug.Log("Angry mon!");
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
