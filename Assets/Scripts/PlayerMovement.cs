using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(x, 0, z).normalized;
        transform.position += move * speed * Time.deltaTime;
    }
}