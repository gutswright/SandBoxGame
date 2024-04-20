using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public class MallardManager
{
    [HideInInspector] public GameObject m_Instance;
    public string m_PlayerName;
    public MallardMovement m_Movement;
    public Transform m_SpawnPoint;
    [HideInInspector] public bool m_IsHost = false;
    [HideInInspector] public TextMeshProUGUI m_PlayerNameText;
    [HideInInspector] public int m_Wins;    
    // Start is called before the first frame update
    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<MallardMovement>();
        // m_PlayerNameText = m_CanvasGameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void DisableControl()
    {
        // if (m_IsHost)
        // {
        //     m_PlayerNameText.text = m_PlayerName;
        // }
        m_Movement.enabled = false;

    }
    public void EnableMovement()
    {
        m_Movement.enabled = true;
    }

    public void EnableControl()
    {
        m_Movement.enabled = true;
    }

    public void Reset()
    {
        // Set positon to 0 and rotation to 0

        m_Instance.transform.position = new Vector3(0, 0, 0);
        m_Instance.transform.rotation = new Quaternion(0, 0, 0, 0);

        // m_Instance.SetActive(false);
        // m_Instance.SetActive(true);

        m_Movement.m_Caught = false;
    }
}
