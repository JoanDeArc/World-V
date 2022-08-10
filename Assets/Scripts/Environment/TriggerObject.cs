using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : MonoBehaviour
{
    public int TriggerID;
    public bool OneTimeTrigger;

    private void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<EventManager>().ExecuteEvent(TriggerID);

        if (OneTimeTrigger)
            gameObject.SetActive(false);
    }
}
