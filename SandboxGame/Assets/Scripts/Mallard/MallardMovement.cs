using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MallardMovement : MonoBehaviour
{
    public CapsuleCollider collider;
    private Rigidbody m_Rigidbody;
    public float m_MovementInputValue = 0f;
    public float m_AngleInputValue = 0f;
    public float m_Speed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Move(m_Speed, m_MovementInputValue);
        Turn();
    }

    // This function is for moving the mallard
    private void Move(float speed, float inputValue)
    {
        Vector3 movement = transform.forward * inputValue * speed * Time.deltaTime;

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private void Turn()
    {
        m_Rigidbody.rotation = Quaternion.Euler(0f, m_AngleInputValue, 0f);
    }

    // Handle a collision with another object
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Goose"))
        {
            Debug.Log("Collision with goose");
        }
    }
}
