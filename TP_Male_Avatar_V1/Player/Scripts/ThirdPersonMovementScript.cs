using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovementScript : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Animator anim;
    Vector3 previous;

    [SerializeField] private float Mass;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpanimationdelay;


    private Vector3 vel;
    private bool isGrounded;

    private float timer;

    private float landmoveTimer;

    private bool jumping;

    private bool hasDirectionalInput;



    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        landmoveTimer = 0;
        jumping = false;
        hasDirectionalInput = false;
        previous = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isControlKeyDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)|| Input.GetKey(KeyCode.C);
        bool isSpaceKeyDown = Input.GetKeyDown(KeyCode.Space);
        //Vector3 offsetforGroundcheck = new Vector3(transform.position.x,transform.position.y-1,transform.position.z);
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        isGrounded = Physics.CheckSphere(transform.position,groundCheckDistance,groundMask);
        if(isGrounded){
            anim.SetBool("isGrounded",true);
        }
        else{
            anim.SetBool("isGrounded",false);
        }
        if(isGrounded&& vel.y <0){//Check if grounded
            vel.y = -2f;
        }
        if(isGrounded){
            //Update Speed of character controller depending on current state or transition
            UpdateSpeed();
            if(direction.x>0 || CameraAngle(direction)=="right"){
                anim.SetBool("TurningRight",true);
                turnSmoothTime = 0.3f;
            }
            else if(direction.x<0|| CameraAngle(direction)=="left"){
                anim.SetBool("TurningLeft",true);
                turnSmoothTime = 0.3f;
            }
            else{
                anim.SetBool("TurningRight",false);
                anim.SetBool("TurningLeft",false);
                turnSmoothTime = 0.1f;
            }
            // Debug.Log(" Target Angle in degrees: "+Mathf.Atan2(direction.x,direction.z) *Mathf.Rad2Deg+cam.eulerAngles.y);
            ///Debug.Log(" Current Avatar Angle in degrees: "+transform.eulerAngles.y);
            //Debug.Log(" Difference Between in degrees: "+((Mathf.Atan2(direction.x,direction.z) *Mathf.Rad2Deg+cam.eulerAngles.y)-(transform.eulerAngles.y)));
            //Debug.Log(" Target Angle in Rads: "+(Mathf.Atan2(direction.x,direction.z)+cam.eulerAngles.y*Mathf.Deg2Rad));
            //Debug.Log(" Current Avatar Angle: "+transform.eulerAngles.y*Mathf.Deg2Rad);
            //Debug.Log(" Difference Between in rads: "+((Mathf.Atan2(direction.x,direction.z))-(transform.eulerAngles.y*Mathf.Deg2Rad)));
            //Debug.Log(" Difference Between in rads: "+((Mathf.Atan2(direction.x,direction.z)+cam.eulerAngles.y*Mathf.Deg2Rad)-(transform.eulerAngles.y*Mathf.Deg2Rad)));


            if(direction.magnitude >= 0.1f && !anim.GetCurrentAnimatorStateInfo(0).IsName("Run To Stop")&&!anim.GetAnimatorTransitionInfo(0).IsName("Jump (1) -> Breathing Idle")){
                if(timer<0){
                    timer = 1f;
                    anim.SetBool("CanWalk",false);
                }
                float targetAngle = Mathf.Atan2(direction.x,direction.z) * Mathf.Rad2Deg+cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,targetAngle, ref turnSmoothVelocity , turnSmoothTime);
                //Debug.Log(Mathf.Abs(turnSmoothVelocity));
                if(Mathf.Abs(turnSmoothVelocity)>600){
                    Debug.Log("Pivot Animation");
                    anim.SetBool("180Turn",true);
                }
                else{
                    anim.SetBool("180Turn",false);
                }
                transform.rotation = Quaternion.Euler(0f,angle,0f);
                anim.SetBool("hasDirection",true);
                hasDirectionalInput = true;
                Vector3 movedir = Quaternion.Euler(0f,targetAngle,0f)*Vector3.forward;
                controller.Move(movedir.normalized * speed * Time.deltaTime);
            }
            else if(direction.magnitude < 0.1f){
                anim.SetBool("hasDirection",false);
                hasDirectionalInput = false;
            }
            timer-=Time.deltaTime;
            if(timer<0){
                anim.SetBool("CanWalk",true);
            }

            //Debug.Log(timer);
            float velocity = Vector3.Distance(previous,transform.position)/Time.deltaTime;
            previous = transform.position;
            anim.SetFloat("Velocity",velocity);
            //Debug.Log(velocity);
            
            if(isControlKeyDown){//Check if Ctrl or C is pressed for crouching animations
                speed = 3f;
                turnSmoothTime = 0.1f;
                anim.SetBool("isCrouched",true);
            }
            else{
                if(isSpaceKeyDown && Jumpable() && jumping==false){//Check if Space is pressed for jump animation
                    jumping=true;
                    anim.SetBool("isJumping",true);
                    if(velocity>0.1 && hasDirectionalInput){
                        jumpanimationdelay = 0.05f;
                        jumpHeight = 9;
                        Invoke("Jumpdelay", jumpanimationdelay);
                    }
                    else if(!hasDirectionalInput&&!isShiftKeyDown){
                        jumpanimationdelay = 0.67f;
                        jumpHeight =8;
                        Invoke("Jumpdelay", jumpanimationdelay);
                    }
                }
                else{
                    anim.SetBool("isJumping",false);
                    jumping=false;
                }
                if(isShiftKeyDown){
                    //jumpanimationdelay = 0.05f;
                    if(jumping==true){
                        //anim.SetBool("isJumping",false);
                        //jumping=false;
                    }
                    speed = 10f;
                    turnSmoothTime = 0.3f;
                    anim.SetBool("ShiftKeyDown",true);
                    //anim.SetBool("isJumping",false);
                }
                else{
                    speed = 6f;
                    turnSmoothTime = 0.1f;
                    anim.SetBool("ShiftKeyDown",false);
                }
                anim.SetBool("isCrouched",false);
            }
               
                
               
        }
       
        vel.y+= gravity*Mass*Time.deltaTime;
        controller.Move(vel*Time.deltaTime);

    }

    private bool Jumpable(){
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Jump (1)")
            &&!anim.GetCurrentAnimatorStateInfo(0).IsName("Jump (2)")
            &&!anim.GetCurrentAnimatorStateInfo(0).IsName("Running Jump")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Jump(1) -> Breathing Idle")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Breathing Idle -> Start Walking")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Start Walking -> Breathing Idle")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Start Walking -> Walking")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Start Walking -> Running")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Breathing Idle -> Left Turn")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Breathing Idle -> Turn To Running (Right)")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Breathing Idle -> Turn To Running (Left)")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Breathing Idle -> Right Turn") 
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Breathing Idle -> Jump (1)")                         
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Left Turn -> Breathing Idle")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Left Turn -> Start Walking")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Turn To Running (Left) -> Running")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Turn To Running (Left) -> Breathing Idle")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Right Turn -> Breathing Idle")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Right Turn -> Start Walking")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Turn To Running (Right) -> Running")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Turn To Running (Right) -> Breathing Idle")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Walking -> Breathing Idle")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Walking -> Running")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Running Jump -> Walking") 
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Running Jump -> Run to Stop")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Running Jump -> Running")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Running -> Walking")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Running -> Run to Stop")
            &&!anim.GetAnimatorTransitionInfo(0).IsName("Run to Stop -> Breathing Idle")
            &&!anim.GetCurrentAnimatorStateInfo(0).IsName("Run to Stop")
            &&!anim.GetCurrentAnimatorStateInfo(0).IsName("Left Turn")
            &&!anim.GetCurrentAnimatorStateInfo(0).IsName("Right Turn")
            &&!anim.GetCurrentAnimatorStateInfo(0).IsName("Turn To Running (Left)")
            &&!anim.GetCurrentAnimatorStateInfo(0).IsName("Turn To Running (Right)")){
                return true;
            }
        else{
            return false;
        }
    }
    private void Jumpdelay(){
            vel.y = Mathf.Sqrt(jumpHeight*-2*gravity);        
    }

    private void UpdateSpeed(){
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Jump (1)")){
            speed = 0.0f;
        }
        else if(anim.GetAnimatorTransitionInfo(0).IsName("Breathing Idle -> Start Walking")||anim.GetCurrentAnimatorStateInfo(0).IsName("Breathing Idle")){
            speed = 0.5f;
        }
        else if(anim.GetCurrentAnimatorStateInfo(0).IsName("Start Walking")){
            speed = 2.8f;
        }
        else if(anim.GetAnimatorTransitionInfo(0).IsName("Walking -> Running") || anim.GetAnimatorTransitionInfo(0).IsName("Start Walking -> Running")){// If starting to run , act as Artificial acceleration
            speed = 8f;
        }
    }

    private string CameraAngle(Vector3 dir){
        /*
        Debug.Log(((Mathf.Atan2(dir.x,dir.z)+cam.eulerAngles.y*Mathf.Deg2Rad)-(transform.eulerAngles.y*Mathf.Deg2Rad)));
        // create check to make sure that the arrow keys dont affect the CC when the TPC is between 280degrees and 100degrees
        if((((Mathf.Atan2(dir.x,dir.z)+cam.eulerAngles.y*Mathf.Deg2Rad)-(transform.eulerAngles.y*Mathf.Deg2Rad))>1.8f&&((Mathf.Atan2(dir.x,dir.z)+cam.eulerAngles.y*Mathf.Deg2Rad)-(transform.eulerAngles.y*Mathf.Deg2Rad))<2.61f)
         ||(((Mathf.Atan2(dir.x,dir.z)+cam.eulerAngles.y*Mathf.Deg2Rad)-(transform.eulerAngles.y*Mathf.Deg2Rad))>-4.5f&&((Mathf.Atan2(dir.x,dir.z)+cam.eulerAngles.y*Mathf.Deg2Rad)-(transform.eulerAngles.y*Mathf.Deg2Rad))<-3.92f)){
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,cam.eulerAngles.y, ref turnSmoothVelocity , turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f,angle,0f);
            return "right";
        }
        else if((((Mathf.Atan2(dir.x,dir.z)+cam.eulerAngles.y*Mathf.Deg2Rad)-(transform.eulerAngles.y*Mathf.Deg2Rad))>3.92f&&((Mathf.Atan2(dir.x,dir.z)+cam.eulerAngles.y*Mathf.Deg2Rad)-(transform.eulerAngles.y*Mathf.Deg2Rad))<4.5f)
         ||(((Mathf.Atan2(dir.x,dir.z)+cam.eulerAngles.y*Mathf.Deg2Rad)-(transform.eulerAngles.y*Mathf.Deg2Rad))>-2.61f&&((Mathf.Atan2(dir.x,dir.z)+cam.eulerAngles.y*Mathf.Deg2Rad)-(transform.eulerAngles.y*Mathf.Deg2Rad))<-1.8f) ){
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,cam.eulerAngles.y, ref turnSmoothVelocity , turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f,angle,0f);
            return "left";
        }
        else return null;
        */
        return null;
        
    }


}
