using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float WalkDist;
    [SerializeField] bool IsWalking = false;
    [SerializeField] LayerMask CollisionMask;
    Transform tf;
    
    void Awake()
    {
        tf = GetComponent<Transform>();
    }


    public void Walk(Vector3 WalkDir)
    {
        StartCoroutine(StartWalk(WalkDir));
    }

    IEnumerator StartWalk(Vector3 dir)
    {
        Vector3 Normalized = dir.normalized;
        float magnitude = Mathf.Abs(Normalized.x) + Mathf.Abs(Normalized.y);
        float walkingTime = 0.01f;
        if (magnitude > 1f) walkingTime = magnitude / 100;
        bool CheckCollision = Physics2D.BoxCast(tf.position + dir, Vector2.one * 0.75f, 0f, Vector2.zero, 1f, CollisionMask);
        if (IsWalking == true || CheckCollision == true) yield break;
        IsWalking = true;
        Vector3 FinalPosition = tf.position + dir;
        float Dist = Vector3.Distance(tf.position, FinalPosition);

        while(Dist > 0.1f)
        {
            tf.position = Vector3.Lerp(tf.position, FinalPosition, 0.15f);
            Dist = Vector3.Distance(tf.position, FinalPosition);
            yield return new WaitForSeconds(walkingTime);
        }
        
        tf.position = FinalPosition;
        IsWalking = false;
        yield return null;
    }
}
