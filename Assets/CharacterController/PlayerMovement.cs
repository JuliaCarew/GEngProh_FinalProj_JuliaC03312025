using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5.0f;
    private Rigidbody2D playerRigidbody;
    public Animator playerAnimator;
    private Vector2 moveDirection;
    private bool canMove = true;
    
    void Awake()
    {
        playerAnimator = this.GetComponentInChildren<Animator>();
        playerRigidbody = this.GetComponent<Rigidbody2D>();
        InputActions.MoveEvent += UpdateMoveVector;
    }
    private void Update()
    {
        HandleAnim();
    }
    private void UpdateMoveVector(Vector2 inputVector) // player input = moveVector(current vector2 from HandlePlayerMove)
    {
        if (canMove == true){
            moveDirection = inputVector;          
        }
        else{
            moveDirection = Vector2.zero; // stop player movement when not allowed to move
        }
    }

    void HandlePlayerMove(Vector2 moveVector) // use .Move functionality to move player on set veriables, gets updated by UpdateMoveVector() method
    {
        playerRigidbody.MovePosition(playerRigidbody.position + moveVector * moveSpeed * Time.fixedDeltaTime);
    }

    private void FixedUpdate() // moving player by character controller component every frame
    {
        HandlePlayerMove(moveDirection);
    }

    public void SetCanMove(bool state){
        canMove = state; // set canMove to true or false depending on game state
        if (state == false) 
        {
            moveDirection = Vector2.zero; // stop player movement when not allowed to move
        }
    }

    /// <summary>
    /// Swap between animations in the blend tree based on 
    /// horizontal/vertical input vector. Handles idle state as well.
    /// </summary>
    private void HandleAnim() 
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (movement.magnitude != 0)
        {
            playerAnimator.SetFloat("Horizontal", movement.y);
            playerAnimator.SetFloat("Vertical", movement.x);
            playerAnimator.SetBool("Moving",true);
        }
        else {
            playerAnimator.SetBool("Moving", false);
        }
    }

    private void OnDisable()
    {
        InputActions.MoveEvent -= UpdateMoveVector;
    }
}
