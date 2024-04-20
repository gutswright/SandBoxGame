using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MallardManager
{
    [HideInInspector] public GameObject m_Instance;
    public string m_PlayerName;
    public MallardMovement m_Movement;



    // Start is called before the first frame update
    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<MallardMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
