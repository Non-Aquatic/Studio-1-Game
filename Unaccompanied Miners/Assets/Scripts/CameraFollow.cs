using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; 
    private void LateUpdate()
    {
       transform.position = new Vector3(transform.position.x, transform.position.y, player.position.z);
    }
}
