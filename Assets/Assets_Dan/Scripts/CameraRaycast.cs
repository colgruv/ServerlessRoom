using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRaycast : MonoBehaviour
{
    private Camera m_Camera;
    private RaycastItem m_CurrentSelected;
    private bool m_IsDragging, m_IsRotating;
    private float m_LastMouseX;
    private Vector3 m_StartPosition;
    private float m_StartRotation;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        DoSelect();
        DoDrag();
        DoRotate();
    }

    private void DoSelect()
    {
        if (m_IsDragging || m_IsRotating)
            return;

        if (Physics.Raycast(m_Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            Transform objectHit = hit.transform;

            if (objectHit == m_CurrentSelected)
                return;
            else if (m_CurrentSelected)
            {
                m_CurrentSelected.IsHighlighted = false;
            }

            RaycastItem item = objectHit.GetComponent<RaycastItem>();
            if (item)
            {
                m_CurrentSelected = item;
                m_CurrentSelected.IsHighlighted = true;
            }
            else
            {
                m_CurrentSelected = null;
            }
        }
    }

    private void DoDrag()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            m_IsDragging = false;
            m_CurrentSelected.IsInteracting = false;

            if (m_CurrentSelected)
            {
                if (!m_CurrentSelected.CheckCollisions())
                    m_CurrentSelected.transform.position = m_StartPosition;
            }
        }   

        if (!m_CurrentSelected)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            m_IsDragging = true;
            m_CurrentSelected.IsInteracting = true;
            m_StartPosition = m_CurrentSelected.transform.position;
        }
            

        if (!m_IsDragging)
            return;

        if (Physics.Raycast(m_Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 50f, 1 << LayerMask.NameToLayer("Terrain")))
        {
            m_CurrentSelected.transform.position = hit.point;
        }
    }

    private void DoRotate()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            m_IsRotating = false;
            m_CurrentSelected.IsInteracting = false;

            if (m_CurrentSelected)
            {
                if (!m_CurrentSelected.CheckCollisions())
                    m_CurrentSelected.transform.eulerAngles = new Vector3(0f, m_StartRotation, 0f);
            }
        }    

        if (!m_CurrentSelected)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            m_IsRotating = true;
            m_CurrentSelected.IsInteracting = true;
            m_StartRotation = m_CurrentSelected.transform.eulerAngles.y;
            m_LastMouseX = Input.mousePosition.x;
        }

        if (!m_IsRotating)
            return;

        float mouseX = Input.mousePosition.x;
        m_CurrentSelected.transform.Rotate(Vector3.up, m_LastMouseX - mouseX);
        m_LastMouseX = mouseX;
    }
}
