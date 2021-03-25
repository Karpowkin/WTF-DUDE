using UnityEngine;

namespace Mirror.Examples.Additive
{    
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(Rigidbody))]
    //Components calling
    public class PlayerController : NetworkBehaviour
    {
        public CharacterController characterController;
        
        void OnValidate()
        {
            if (characterController == null)
                characterController = GetComponent<CharacterController>();

        }

        void Start()
        {
            characterController.enabled = isLocalPlayer;
            
        }

        public override void OnStartLocalPlayer()
        {
            Camera.main.orthographic = false;
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0f, 5f, 0f);
            Camera.main.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            //Camera position on Start
        }

        void OnDisable()
        {
            if (isLocalPlayer && Camera.main != null)
            {
                Camera.main.orthographic = true;
                Camera.main.transform.SetParent(null);
                Camera.main.transform.localPosition = new Vector3(0f, 70f, 0f);
                Camera.main.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            }
            
        }

        [Header("Movement Settings")]
        public float moveSpeed = 8f;
        public float turnSensitivity = 5f;
        public float maxTurnSpeed = 150f;

        [Header("Diagnostics")]
        public float horizontal;
        public float vertical;
        public float turn;
        public float jumpSpeed;
        public bool isGrounded = true;
        public bool isFalling;
        public Vector3 velocity;

        void Update()
        {
         
            if (!isLocalPlayer || !characterController.enabled)
                return;
            
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed = 7f;
            }
            else
            {
                moveSpeed = 2.7f;
            }
            //from walk to run

            // Q and E cancel each other out, reducing the turn to zero
            if (Input.GetKey(KeyCode.Q))
                turn = Mathf.MoveTowards(turn, -maxTurnSpeed, turnSensitivity);
            if (Input.GetKey(KeyCode.E))
                turn = Mathf.MoveTowards(turn, maxTurnSpeed, turnSensitivity);
            if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.E))
                turn = Mathf.MoveTowards(turn, 0, turnSensitivity);
            if (!Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
                turn = Mathf.MoveTowards(turn, 0, turnSensitivity);
            
        }

        void FixedUpdate()
        {
            if (!isLocalPlayer || characterController == null)
                return;
            
            transform.Rotate(0f, turn * Time.fixedDeltaTime, 0f);
            //Calling Q and E function
            Vector3 direction = new Vector3(horizontal, jumpSpeed, vertical);
            Vector3 rotation = new Vector3(horizontal, 0, vertical);
            direction = Vector3.ClampMagnitude(direction, 1f);
            direction = transform.TransformDirection(direction);
            direction *= moveSpeed;
            characterController.Move((Vector3.right * horizontal + Vector3.forward * vertical) * Time.deltaTime);
            if (jumpSpeed > 0)
                characterController.Move(direction * Time.fixedDeltaTime);
            else
                characterController.SimpleMove(direction);
            float rotate = rotation.x;
            transform.rotation = Quaternion.LookRotation(rotation);
            isGrounded = characterController.isGrounded;
            velocity = characterController.velocity;
            Debug.Log(rotation);    
            //Debug.Log(vertical);
            //character movement
        }
    }
}
