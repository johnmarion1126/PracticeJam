using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Rigidbody2D rigidbodyEnemy;
    [SerializeField]
    private Animator animatorEnemy;
    [SerializeField]
    private SpriteRenderer spriteRendererEnemy;

    [SerializeField]
    private GameObject enemyObject;
    [SerializeField]
    private BoxCollider2D enemyAttack;
    [SerializeField]
    protected Stats enemyStats;
    [SerializeField]
    protected List <string> enemyAnimations;

    [SerializeField]
    private List <GameObject> enemyDrops;

    [SerializeField]
    private GameObject playerObject;
    [SerializeField]
    protected Transform player;
    [SerializeField]
    protected Vector2 movement;

    [SerializeField]
    protected float moveEnemySpeed = 2f;
    [SerializeField]
    protected float attackDuration;
    [SerializeField]
    protected int points;

    protected GameObject dialogBox;
    protected DialogBox dialog;
    protected GameObject HPScore;
    protected GameObject scoreObject;
    protected Score score;
    private GameObject soundManager;
    private SoundManager sound;

    protected int randomNum;
    protected int damage;
    protected float damaged = 0.5f;
    protected float damagedDuration = 0.5f;
    protected float range = 2.0f;
    protected bool inAction = false;
    protected bool inRange = false;
    protected bool isDead = false;

    protected void Awake() {
        enemyStats = enemyObject.GetComponent<Stats>();
        rigidbodyEnemy = GetComponent<Rigidbody2D>();
        animatorEnemy = GetComponent<Animator>();
        HPScore = GameObject.Find("HP&Score");
        playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<Transform>();

        dialogBox = GameObject.Find("DialogBox");
        dialog = dialogBox.GetComponent<DialogBox>();
        soundManager = GameObject.Find("SoundManager");
        sound = soundManager.GetComponent<SoundManager>();
    }

    protected void Start() {
        scoreObject = HPScore.transform.GetChild(2).gameObject;
        score = HPScore.GetComponent<Score>();
        damage = playerObject.GetComponent<Stats>().damage;
    }

    public virtual void Update() {
        Vector3 direction = player.position - transform.position;
        direction.Normalize();
        movement = direction;

        if (damaged < damagedDuration) damaged += Time.deltaTime;
        if (inRange && !inAction && damaged >= damagedDuration) StartCoroutine(delayCall());
        checkInRange();
    }

    protected void FixedUpdate() {
        if (!inAction && damaged >= damagedDuration) moveEnemy(movement);
    }

    protected void moveEnemy(Vector2 direction) {
        if (direction.x > 0) {
            enemyObject.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
        }
        else {
            enemyObject.transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
        }

        if (direction.y > 0.01f) {
            spriteRendererEnemy.sortingOrder = 3;
        }
        else {
            spriteRendererEnemy.sortingOrder = 1;
        }

        animatorEnemy.Play(enemyAnimations[0]);
        rigidbodyEnemy.MovePosition((Vector2)transform.position + (Vector2)(direction * moveEnemySpeed * Time.deltaTime));
    }

    protected void checkInRange() {
        if (Vector3.Distance(player.position, transform.position) >= range) {
            inRange = false;
            rigidbodyEnemy.constraints = RigidbodyConstraints2D.None;
            rigidbodyEnemy.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    protected void OnTriggerEnter2D(Collider2D playerObject) {
        inRange = true;
        rigidbodyEnemy.constraints = RigidbodyConstraints2D.FreezeAll;
        if (playerObject.name == "PlayerAttack" && !isDead) StartCoroutine(takeDamage(damage));
    }

    protected IEnumerator delayCall() {
        inAction = true;
        animatorEnemy.Play(enemyAnimations[1]);
        yield return new WaitForSeconds(0.5f);
        if (inRange && damaged >= damagedDuration) StartCoroutine(attackPlayer());
        else inAction = false;
    }

    protected IEnumerator attackPlayer() {

        if (Random.Range(1,3) == 1) {
            animatorEnemy.Play(enemyAnimations[4]);
        }
        else {
            animatorEnemy.Play(enemyAnimations[5]);
        }
        sound.playSound("miss");
        enemyAttack.size = new Vector2(1.5f,0.1f);
        yield return new WaitForSeconds(attackDuration);
        enemyAttack.size = new Vector2(0.0001f,0.0001f);
        
        inAction = false;
    }

    public IEnumerator takeDamage(int amount) {
        damaged = 0f;
        isDead = enemyStats.takeDamage(amount);

        if (isDead) {
            enemyObject.GetComponent<Transform>().GetChild(1).gameObject.SetActive(false);
            enemyObject.GetComponent<BoxCollider2D>().enabled = false;
            animatorEnemy.Play(enemyAnimations[2]);
            sound.playSound("faint");
            damaged = -1f;
            score.increaseScore(points);
            StartCoroutine(dropItem());
            StartCoroutine(enemyStats.fadeOut());
        }
        else {
            animatorEnemy.Play(enemyAnimations[3]);
            sound.playSound("jab");
            yield return new WaitForSeconds(0.2f);
        }
    }

    public IEnumerator dropItem() {
        randomNum = Random.Range(0,7);
        yield return new WaitForSeconds(0.8f);
        if (randomNum < 2) {
            Instantiate(enemyDrops[randomNum], 
            new Vector3(enemyObject.transform.position.x, enemyObject.transform.position.y, 0f),
            Quaternion.identity);
        }
    }

}
