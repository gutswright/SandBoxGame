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
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
