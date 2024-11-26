using UnityEngine;

public class RatDetection : MonoBehaviour
{
    public Vector3[] ratPoints;
    public Vector3 front, back, top, bottom, left, right;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        front = transform.position + transform.forward;
        back = transform.position - transform.forward;
        top = transform.position + transform.up;
        bottom = transform.position - transform.up;
        left = transform.position - transform.right;
        right = transform.position + transform.right;
        ratPoints = new Vector3[] { front, back, top, bottom, left, right };



    }
    
    // Update is called once per frame
    void OnDrawGizmos()
    {
        Debug.Log("Drawing");
        Gizmos.color = Color.yellow;
        Vector3[] newPoints = new Vector3[6];
        for (int i = 0; i < ratPoints.Length; i++)
        {
            // (bad)assuming scale factor is same on all axes, which should be true
            newPoints[i] = ratPoints[i] * transform.localScale.x;
        }

        Gizmos.DrawSphere(front, 0.2f);
        Gizmos.DrawSphere(back, 0.2f);
        Gizmos.DrawSphere(top, 0.2f);
        Gizmos.DrawSphere(bottom, 0.2f);
        Gizmos.DrawSphere(front, 0.2f);
        Gizmos.DrawSphere(front, 0.2f);

    }
}
