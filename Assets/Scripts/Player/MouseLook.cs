using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

    [Header("Mouse Settings")]
    [Tooltip("Sensitivity of the mouse")]
    public float mouseSens = 100f;
    public Transform playerBody;
    float xRotation = 0f;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        GameManager.gm.playerRB.MoveRotation(Quaternion.Euler(new Vector3(GameManager.gm.playerRB.rotation.eulerAngles.x, GameManager.gm.playerRB.rotation.eulerAngles.y + mouseX, GameManager.gm.playerRB.rotation.eulerAngles.z)));
        //playerBody.Rotate(Vector3.up * mouseX);
    }

}
