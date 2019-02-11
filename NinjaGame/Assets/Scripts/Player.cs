using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DeadEventHandler();

public class Player : Character {

    private static Player instance;

    private IUseable useable;

    public event DeadEventHandler Dead;
    public static Player Instance
    {
        get
        {
            if (instance==null)
            {
                instance = GameObject.FindObjectOfType<Player>();
            }

            return instance;
        }
    }
    [SerializeField]
    private Transform[] groundPoints;
    [SerializeField]
    private float groundRadius;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private bool airControl;
    [SerializeField]
    private float jumpForce;
    private bool immortal=false;
    [SerializeField]
    private float immortalTime;
    [SerializeField]
    private float climbSpeed;
    private SpriteRenderer spriteRenderer;
    private Vector2 startPos;



    private float dashSpeed;

    private DashState dashState;
    private enum DashState
    {
        Normal,
        DashState
    }

    //(KarakterHareket)Bu bizim Rigidbody'miz karaktere bir hareket yaptırdığımız zaman kullanıyoruz.
    public Rigidbody2D MyRigidbody { get; set; }

    public bool OnLadder { get; set; }
    public bool Slide { get; set; }
    public bool Jump { get; set; }
    public bool OnGround { get; set; }

    public override bool IsDead
    {
        get
        {
            if (healthStat.CurrentVal<=0)
            {
                OnDead();
            }
            return healthStat.CurrentVal <= 0;
        }
    }



    // Use this for initialization
    public override void Start () {

        base.Start();
        OnLadder = false;
        startPos = transform.position;
        MyRigidbody = GetComponent<Rigidbody2D>();//(KarakterHareket)Karakterimiz için kullanacağımız Rigidbody yapısını GetComponent<> methodu ile çekiyoruz.
        spriteRenderer = GetComponent<SpriteRenderer>();
        dashState = DashState.Normal;
        
    }
    void Update()
    {
        if (!TakingDamage && !IsDead)
        {
            if (transform.position.y<=-14f)
            {
                Death();
            }
            HandleInput();//(KarakterHareket)Klavyeden karakter girişlerini halleden fonsiyonumuz.
        }

        
        
    }

    // Update is called once per frame
    void FixedUpdate () {
        
        if (!TakingDamage && !IsDead)
        {
            Debug.Log(dashState);
            switch(dashState)
            {
                case DashState.Normal:
            //(KarakterHareket)Unity'nin kendi içerisinde bulunan Horizontal(yatay) eksende hareket etmemiz için kullandığımız komut.Horizontal bize A tuşu için -1 D tuşu için +1 değeri verir.
            float horizontal = Input.GetAxis("Horizontal");//(KarakterHareket)
            float vertical = Input.GetAxis("Vertical");
            OnGround = IsGrounded();
            HandleMovement(horizontal,vertical);//(KarakterHareket)
            Flip(horizontal);//(KarakterHareket)       
            DashMove();
            HandleLayers();//(KarakterHareket)
                    break;
                case DashState.DashState:
                    HandleDashMove();
                    break;
                }
            }
     
    }
    public void OnDead()
    {
        if (Dead!=null)
        {
            Dead();
        }

    }


    private void HandleMovement(float horizontal,float vertical)
    {
        if (MyRigidbody.velocity.y<0)
        {
            MyAnimator.SetBool("land",true);
        }
        if (!Attack && !Slide && (OnGround || airControl))
        {
            MyRigidbody.velocity = new Vector2(horizontal*(movspeed*Time.deltaTime),MyRigidbody.velocity.y);
        }
        if (Jump && MyRigidbody.velocity.y==0 && !OnLadder)
        {
            MyRigidbody.AddForce(new Vector2(0, jumpForce));
        }
        if (OnLadder)
        {
            MyAnimator.speed = vertical != 0 ? Mathf.Abs(vertical) : Mathf.Abs(horizontal);
            MyRigidbody.velocity = new Vector2(horizontal*climbSpeed,vertical*climbSpeed);
        }


        MyAnimator.SetFloat("speed",Mathf.Abs(horizontal));
    }


    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            MyAnimator.SetTrigger("attack");
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            MyAnimator.SetTrigger("slide");
        }
        if (Input.GetKeyDown(KeyCode.Space) && !OnLadder)
        {
            MyAnimator.SetTrigger("jump");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            MyAnimator.SetTrigger("throw");            
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Use();
        }
     
    }

    private void DashMove()
    {


        if (Input.GetKeyDown(KeyCode.C))
        {
          
            Debug.Log("Pressed Dash");
            dashState = DashState.DashState;
            dashSpeed = 35;
        }
    }

    private void HandleDashMove()
    {
        if(!facingRight)
        transform.position += new Vector3(-1f, 0, 0) * dashSpeed*Time.fixedDeltaTime;
        else
        transform.position += new Vector3(1f, 0, 0) *dashSpeed* Time.fixedDeltaTime;

        dashSpeed -= dashSpeed * 5*Time.fixedDeltaTime;
        if (dashSpeed<5)
        {
            dashState = DashState.Normal;
        }
    }

    private void Flip(float horizontal)
    {
     
        //(KarakterHareket)Eğer 0'dan büyük ise biz pozitif değer alıyoruz(D tuşu) ve o an doğru yöne bakıyoruz(Bu programa göre en azından).
        if ((horizontal>0 && !facingRight)||(horizontal<0 &&facingRight))      
        {
            ChangeDirection();

        }

    }



    private bool IsGrounded()
    {
        if (MyRigidbody.velocity.y<=0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject!=gameObject)
                    {
                        return true;
                    }

                }
            }
        }
        return false;

    }

    private void HandleLayers()
    {
        if (!OnGround)
        {
            MyAnimator.SetLayerWeight(1,1);
        }
        else
        {
            MyAnimator.SetLayerWeight(1, 0);
        }

    }

    public override void ThrowKnife(int value)
    {
        if ((!OnGround && value==1)||(OnGround && value==0))
        {
            base.ThrowKnife(value);
        }

      
    }

    private IEnumerator IndicateImmortal()
    {

        while (immortal)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(.1f);
        }
    }

    public override IEnumerator TakeDamage()
    {
        if (!immortal)
        {
            healthStat.CurrentVal -= 10;
            if (!IsDead)
            {
                MyAnimator.SetTrigger("damage");
                immortal = true;
                StartCoroutine(IndicateImmortal());
                yield return new WaitForSeconds(immortalTime);
                immortal = false;
            }
            else
            {
                MyAnimator.SetLayerWeight(1, 0);
                MyAnimator.SetTrigger("die");
            }
            yield return null;
        }
 
    }

    public override void Death()
    {
        MyRigidbody.velocity = Vector2.zero;
        MyAnimator.SetTrigger("idle");
        healthStat.CurrentVal = healthStat.MaxVal;
        transform.position = startPos;

    }

    public void Use()
    {
        if (useable!=null)
        {
            useable.Use();
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag=="Coin")
        {
            GameManager.Instance.CollectedCoins++;
            Destroy(other.gameObject);
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag=="Useable")
        {
            useable = other.GetComponent<IUseable>();
        }
        base.OnTriggerEnter2D(other);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag=="Useable")
        {
            useable = null;
        }   
    }

}
