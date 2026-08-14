using UnityEngine;

public class FirstPerson : MonoBehaviour
{
    private Camera cam;
    private float Speed = 5f;
    private float Jump = 8f;
    private float Sensitivity = 2f;
    private float Height = 1.6f;
    private float rotX;
    private Rigidbody rb;
    private Vector3 mRayCastOffset;
    private LayerMask mask;
    public bool grounded;
    void Start()
    {
        InitData();
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        mRayCastOffset = new Vector3(0, -GetComponent<CapsuleCollider>().height / 2 + 0.1f);
        int playerLayer = LayerMask.NameToLayer("Player");
        mask = ~(1 << playerLayer);
    }
    void Update()
    {
        // 从脚底发射射线
        grounded = Physics.Raycast(transform.position + mRayCastOffset, Vector3.down, 1f, mask);
        Debug.DrawRay(transform.position + mRayCastOffset, Vector3.down, Color.green);
        // --- 视角 ---
        transform.Rotate(0, Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime, 0);
        rotX -= Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, -90f, 90f);
        cam.transform.rotation = Quaternion.Euler(rotX, transform.eulerAngles.y, 0f);
        cam.transform.position = transform.position + Vector3.up * Height;
        // --- 移动 ---
        Vector3 move = transform.right * Input.GetAxis("Horizontal")
                     + transform.forward * Input.GetAxis("Vertical");
        rb.velocity = new Vector3(move.x * Speed, rb.velocity.y, move.z * Speed);
        // --- 跳跃 & 重力（关键修复）---
        if (grounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = new Vector3(rb.velocity.x, Jump, rb.velocity.z);
            }
        }
    }
    void InitData()
    {
        Speed = 0;
        Jump = 0;
        Sensitivity = 1;
        Height = 0;
    }
}