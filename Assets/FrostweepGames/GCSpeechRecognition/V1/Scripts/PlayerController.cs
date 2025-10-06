using UnityEngine;

public class PlayerController : MonoBehaviour
{
 //  private Animator animator;
    private Rigidbody rb;
    
    [Header("Tốc độ di chuyển")]
    public float runSpeed = 5f;
    public float jumpForce = 7f;
    
    private bool isRunning = false;

    void Start()
    {
       // animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    public void Stand()
    {
        isRunning = false;
      
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    public void Run()
    {
        isRunning = true;
      
    }

    public void Jump()
    {
       
        if (rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void Stop()
    {
        isRunning = false;
      
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    void Update()
    {
        // Tự động di chuyển nếu đang trong trạng thái chạy
        if (isRunning && rb != null)
        {
            Vector3 movement = transform.forward * runSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }
}
