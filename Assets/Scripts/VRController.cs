using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRController : MonoBehaviour
{
    public float m_Sensitivity = 0.1f;
    public float m_MaxSpeed = 1.0f;

    public SteamVR_Action_Boolean m_MovePress = null;
    public SteamVR_Action_Vector2 m_MoveValue = null;

    private float m_Speed = 0.0f;

    private CharacterController m_CharacterController = null;
    private Transform m_CameraRig = null;
    private Transform m_Head = null;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        m_CameraRig = SteamVR_Render.Top().origin;
        m_Head = SteamVR_Render.Top().head;
    }

    // Update is called once per frame
    void Update()
    {
        HandleHead();
        CalculateMvmt();
        HandleHeight();
    }

    private void HandleHead()
    {
        // curr transform vals
        Vector3 oldPosition = m_CameraRig.position;
        Quaternion oldRotation = m_CameraRig.rotation;

        // handle rotation of head
        transform.eulerAngles = new Vector3(0.0f, m_Head.rotation.eulerAngles.y, 0.0f);

        // restore transform after rotation
        m_CameraRig.position = oldPosition;
        m_CameraRig.rotation = oldRotation;
    }

    private void CalculateMvmt()
    {
        // find mvmt orientation
        Vector3 orientationEuler = new Vector3(0, transform.eulerAngles.y, 0);
        Quaternion orientation = Quaternion.Euler(orientationEuler);
        Vector3 movement = Vector3.zero;

        // not moving
        if (m_MovePress.GetStateUp(SteamVR_Input_Sources.Any))
        {
            m_Speed = 0;
        }

        // if pressed
        if (m_MovePress.state)
        {
            // clamp
            m_Speed += m_MoveValue.axis.y * m_Sensitivity;
            m_Speed = Mathf.Clamp(m_Speed, -m_MaxSpeed, m_MaxSpeed);

            // orientation
            movement += orientation * (m_Speed * Vector3.forward) * Time.deltaTime;

        }

        // apply mvmt
        m_CharacterController.Move(movement);
    }

    private void HandleHeight()
    {
        // get head in local space
        float headHeight = Mathf.Clamp(m_Head.localPosition.y, 1, 2);
        m_CharacterController.height = headHeight;

        // cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = m_CharacterController.height / 2;
        newCenter.y += m_CharacterController.skinWidth;

        // move capsule in local space
        newCenter.x = m_Head.localPosition.x;
        newCenter.z = m_Head.localPosition.z;

        // rotate
        newCenter = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * newCenter;

        // apply
        m_CharacterController.center = newCenter;
    }
}
