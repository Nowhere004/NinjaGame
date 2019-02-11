using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {
    [SerializeField]
    protected Transform knifePos;
    [SerializeField]
    protected float movspeed;//(KarakterHareket)Hareket hızımız karekteri haraket ettirirken kullanııyoruz
    protected bool facingRight;//(KarakterHareket)Karakterin hangi Tarafa Doğru baktığını sağlamamıza yarayacak ifademiz.
    [SerializeField]
    private GameObject knifePrefab;    
    [SerializeField]
    protected Stat healthStat;
    public abstract bool IsDead { get; }
    public bool Attack { get; set; }
    public Animator MyAnimator { get;private set; }
    public bool TakingDamage { get; set; }



    [SerializeField]
    private EdgeCollider2D swordCollider;
    [SerializeField]
    private List<string> damageSources;
    public EdgeCollider2D SwordCollider
    {
        get
        {
            return swordCollider;
        }

    }
    // Use this for initialization
    public virtual void Start ()
    {
        facingRight = true;//(KarakterHareket)Karakterimizin doğru yöne bakması için kullanacağımız ifademiz.
        MyAnimator = GetComponent<Animator>(); //(KarakterHareket)Karakterimiz için kullanacağımız Animasyonları oynatabilmemiz için karakterdeki animator'ü alıyoruz.
        healthStat.Initialize();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public abstract IEnumerator TakeDamage();

    public abstract void Death();

    public void ChangeDirection()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);

    }

    public virtual void ThrowKnife(int value)
    {

        if (facingRight)
        {
            GameObject tmp = (GameObject)Instantiate(knifePrefab, knifePos.position, Quaternion.Euler(new Vector3(0, 0, -90)));
            tmp.GetComponent<Knife>().Initialize(Vector2.right);
        }
        else
        {
            GameObject tmp = (GameObject)Instantiate(knifePrefab, knifePos.position, Quaternion.Euler(new Vector3(0, 0, 90)));
            tmp.GetComponent<Knife>().Initialize(Vector2.left);
        }
    }

    public void MeleeAttack()
    {
        swordCollider.enabled =true;

    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (damageSources.Contains(other.tag))
        {
            StartCoroutine(TakeDamage());
        }

    }
    
}
