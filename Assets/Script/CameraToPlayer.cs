using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    Vector3 prePlayerPos;

    void Update()
    {
        if(player.transform.position != prePlayerPos)
        {
            transform.position = new Vector3(player.transform.position.x + 2, player.transform.position.y + 1, -10);
            prePlayerPos = player.transform.position;
        }
    }
}