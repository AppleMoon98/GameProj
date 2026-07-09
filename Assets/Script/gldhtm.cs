using UnityEngine;

public class gldhtm : MonoBehaviour
{
    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rigid.AddTorque(-20 * Time.deltaTime);
    }
}
