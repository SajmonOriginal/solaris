using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public List<Rigidbody> ragdollParts = new List<Rigidbody>();

    private void Start()
    {
        SetRagdollState(false); // Nepřítel se normálně pohybuje na začátku
    }

    public void SetRagdollState(bool state)
    {
        // Zapne/vypne ragdoll
        foreach (Rigidbody rb in ragdollParts)
        {
            rb.isKinematic = !state;
        }
        
        // Zde také deaktivujeme animátora, pokud ragdoll je zapnuto
        GetComponent<Animator>().enabled = !state;
    }
}