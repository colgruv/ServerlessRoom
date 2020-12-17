using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaycastItem : MonoBehaviour
{
    public bool IsHighlighted
    {
        set
        {
            m_material.SetColor("_EmissionColor", value ? Color.yellow * 0.2f : Color.black);
        }
    }

    public bool IsInteracting { private get; set; }

    private Material m_material;
    private int m_Collisions;
    private Collider m_Collider;

    // Start is called before the first frame update
    void Start()
    {
        m_material = GetComponentInChildren<Renderer>().material;
        m_Collisions = 0;
        m_Collider = GetComponentInChildren<Collider>();
    }

    void Update()
    {
        m_Collider.isTrigger = IsInteracting;

        if (IsInteracting)
        {
            m_material.SetColor("_EmissionColor", IsInteracting ? (m_Collisions == 0 ? Color.green * 0.2f : Color.red * 0.2f) : Color.black);
            
        }   
    }

    public bool CheckCollisions()
    {
        return m_Collisions == 0;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);

        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            m_Collisions++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            m_Collisions--;
        }
    }
}
