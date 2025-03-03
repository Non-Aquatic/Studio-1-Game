using UnityEngine;

//Simple script that follows the location of the player
public class CameraFollow : MonoBehaviour
{
    int cameraAdjustment = 4;
    public Transform player; 
    private void LateUpdate()
    {
        //Keeps the same x and y, changes the z
       transform.position = new Vector3(transform.position.x, transform.position.y, (player.position.z/cameraAdjustment));
    }
}
