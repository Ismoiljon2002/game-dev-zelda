
using UnityEngine;

public class CommentedCameraController : MonoBehaviour
{

    public bool clickToMoveCamera = false;
    public bool canZoom = true;
    public float sensitivity = 5f;

    public Vector2 cameraLimit = new Vector2(-45, 40);

    float mouseX;
    float mouseY;

    float offsetDistanceY;

    Transform player;


    void Start()
    {
        if (clickToMoveCamera == false)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked; // Makes the mouse stick to the center of the screen
            UnityEngine.Cursor.visible = false; // Disappears with the cursor
        }

        player = GameObject.FindWithTag("Player").transform;

        offsetDistanceY = transform.position.y;
    }


    void Update()
    {
        transform.position = player.position + new Vector3(0, offsetDistanceY, 0);

        if (canZoom && Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Camera.main.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * sensitivity * 2;
        }

        if (clickToMoveCamera == true)
        {
            if (Input.GetAxisRaw("Fire2") == 0)
                return;
        }

        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY += Input.GetAxis("Mouse Y") * sensitivity;

        mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);

        transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0);

    }
}

