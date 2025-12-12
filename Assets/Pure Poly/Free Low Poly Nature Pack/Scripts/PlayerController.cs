using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 10f;

    private float movementX;
    private float movementY;

    void Update()
    {
        movementX = Input.GetAxis("Horizontal");
        movementY = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    // Variable to keep track of collected "PickUp" objects.
    private int count;

    // UI text component to display count of "PickUp" objects collected.
    public TextMeshProUGUI countText;
    // Start is called before the first frame update.
    void Start()
    {
        // Get and store the Rigidbody component attached to the player.
        rb =  GetComponent<Rigidbody>();
        // Initialize count to zero.
        count = 0;
        // Update the count display.
        SetCountText();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object the player collided with has the "PickUp" tag.
        if (other.gameObject.CompareTag("PickUp"))
        {
            // Deactivate the collided object (making it disappear).
            other.gameObject.SetActive(false);
            // Increment the count of "PickUp" objects collected.
            count = count + 1;
            // Update the count display.
            SetCountText();
        }
    }
    // Function to update the displayed count of "PickUp" objects collected.
    void SetCountText()
    {
        countText.text = "Coins: " + count.ToString();
    }
}