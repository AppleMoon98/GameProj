using UnityEngine;

public class AutoMoveTestScript : MonoBehaviour
{
    public Transform target;
    Vector2 targetVec = new Vector2();
    Rigidbody2D rigid;
    public float obj_speed;

    public LineRenderer line;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        line.enabled = true;
        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target.position);
    }

    void FixedUpdate()
    {
        targetVec = target.position;
        rigid.linearVelocity = Vector2.zero;
        transform.position = Vector2.MoveTowards(transform.position, targetVec, obj_speed * Time.deltaTime);
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target.position);
    }
}
