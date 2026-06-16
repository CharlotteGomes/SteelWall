using TMPro;
using UnityEngine;

public class BattleCameraController : MonoBehaviour
{
    [SerializeField] Transform cam;

    [SerializeField] Vector3 TargetPos;
    Vector3 velocity;
    [SerializeField] float SmoothTime = 0.2f;
    [SerializeField] Vector3 DefaultOffset;
    
    

    void Start()
    {
        
    }

    void LateUpdate()
    {
        cam.transform.position = Vector3.SmoothDamp(
          cam.transform.position,
          TargetPos,
          ref velocity,
          SmoothTime
      );
    }

    public void FocusBetween(Transform a, Transform b)
    {
        Vector3 midfocus = (a.position + b.position) / 2;
        TargetPos = midfocus + DefaultOffset;
    }
}
