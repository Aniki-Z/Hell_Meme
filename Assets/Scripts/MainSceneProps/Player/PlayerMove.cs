using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 1f;
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveX, moveY) * moveSpeed * Time.deltaTime;

        transform.Translate(movement);
    }
}

