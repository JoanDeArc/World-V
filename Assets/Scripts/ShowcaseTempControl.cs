using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowcaseTempControl : MonoBehaviour
{
    public Transform Companimons;
    private int index;
    public Companimon CurrentCompanimon;

    // Start is called before the first frame update
    void Start()
    {
        // Just made as a temporary way of navigation showcase scene
        index = 0;
        CurrentCompanimon = Companimons.GetChild(index).GetComponent<Companimon>();

    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            index++;
            if (index >= Companimons.childCount)
                index = 0;

            CurrentCompanimon = Companimons.GetChild(index).GetComponent<Companimon>();   
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentCompanimon.gameObject.GetComponent<Animator>().SetTrigger("AttackClose");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentCompanimon.gameObject.GetComponent<Animator>().SetTrigger("AttackRange");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrentCompanimon.gameObject.GetComponent<Animator>().SetTrigger("Dash");
        }


        if (Input.GetKeyDown(KeyCode.Q))
        {
            foreach (Transform companimon in Companimons)
            {
                companimon.GetComponent<Animator>().SetBool("IsMoving", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            foreach (Transform companimon in Companimons)
            {
                companimon.GetComponent<Animator>().SetBool("IsMoving", true);
            }
        }
    }
}
