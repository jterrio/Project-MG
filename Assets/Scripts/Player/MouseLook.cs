using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

    [Header("Mouse Settings")]
    [Tooltip("Sensitivity of the mouse")]
    public float HorizontalMouseSens = 1f;
    public float VerticalMouseSens = 1f;
    public Transform playerBody;
    float xRotation = 0f;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        float mouseX = Input.GetAxis("Mouse X") * HorizontalMouseSens;
        float mouseY = Input.GetAxis("Mouse Y") * (VerticalMouseSens / 2f);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Vector3 playerRotation = GameManager.gm.playerRB.transform.rotation.eulerAngles;
        playerRotation.y += mouseX;

        //GameManager.gm.playerRB.MoveRotation(Quaternion.Euler(new Vector3(GameManager.gm.playerRB.rotation.eulerAngles.x, GameManager.gm.playerRB.rotation.eulerAngles.y + mouseX, GameManager.gm.playerRB.rotation.eulerAngles.z)));
        //playerBody.Rotate(Vector3.up * mouseX);

        //this.transform.rotation = Quaternion.Euler(cameraRotation);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        GameManager.gm.playerRB.rotation = Quaternion.Euler(playerRotation);

    }

    public void updateHorizontalSens(float value) {
        HorizontalMouseSens = value;
    }
    public void updateVerticalSens(float value) {
        VerticalMouseSens = value;
    }

}
