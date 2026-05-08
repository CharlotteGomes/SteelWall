using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float horizontal, vertical;
    [SerializeField] float WalkDist;
    [SerializeField] Vector3 WalkDir = Vector3.zero;
    [SerializeField] bool IsWalking = false;
    [SerializeField] LayerMask CollisionMask;
    Transform tf;
    
    public bool CheckCollision()
    {
        return Physics2D.BoxCast(tf.position + WalkDir , Vector2.one * 0.75f, 0f, Vector2.zero, CollisionMask);
    }
    void Awake()
    {
        tf = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        Walk();
    }

    void Walk()
    {
        WalkDir = new Vector3(horizontal, vertical) * WalkDist;
        if(vertical == 0 && IsWalking == false)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
        }

        if (horizontal == 0 && IsWalking == false)
        {
            vertical = Input.GetAxisRaw("Vertical");
        }

        if(horizontal !=0 || vertical != 0)
        {
            StartCoroutine(StartWalk(WalkDir));
        }
    }

    IEnumerator StartWalk(Vector3 dir)
    {
        if(IsWalking == true || CheckCollision() == true) yield break;
        IsWalking = true;
        Vector3 FinalPosition = tf.position + WalkDir;
        float Dist = Vector3.Distance(tf.position, FinalPosition);

        while(Dist > 0.1f)
        {
            tf.position = Vector3.Lerp(tf.position, FinalPosition, 0.15f);
            Dist = Vector3.Distance(tf.position, FinalPosition);
            yield return new WaitForSeconds(0.01f);
        }
        
        tf.position = FinalPosition;
        IsWalking = false;
        yield return null;
    }
}
