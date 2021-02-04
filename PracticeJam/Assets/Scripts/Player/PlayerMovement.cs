using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    public Animator animator;

    PlayerStats playerStats;
    public GameObject playerObject;
    public BoxCollider2D playerAttack;
    public HPAndScore hpScore;

    public GameObject enemyObject;

    public float moveSpeed = 5f; 
    private bool inAction = false;
    private bool isDead = false;

    Vector2 movement;

    void Start() {
        playerStats = playerObject.GetComponent<PlayerStats>();
        hpScore = hpScore.GetComponent<HPAndScore>();
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();

        hpScore.setHP(playerStats.playerCurrentHP);
        hpScore.setScore(0);
    }

    void Update() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (!inAction) {

            if (movement.y != 0) {
                animator.Play("HeroWalk");
            }
            else if (movement.x == 1) {
                animator.Play("HeroWalk");
                playerObject.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
            }
            else if (movement.x == -1) {
                animator.Play("HeroWalk");
                playerObject.transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
            }
            else {
                animator.Play("HeroIdle");
            }

            if (Input.GetKey("z")) {
                StartCoroutine(heroAction("HeroPunch"));
            }
            if (Input.GetKey("x")) {
                StartCoroutine(heroAction("HeroJab"));
            }

            rigidBody.MovePosition(rigidBody.position + movement * moveSpeed * Time.fixedDeltaTime);
        }  
    }

    void OnTriggerEnter2D(Collider2D enemyObject) {
        if (enemyObject.name == "EnemyAttack" && !isDead) StartCoroutine(takeDamage());
    }

    IEnumerator heroAction(string action) {
        inAction = true;
        animator.Play(action);

        playerAttack.size = new Vector2(1.5f,0.1f);
        yield return new WaitForSeconds(0.3f);
        playerAttack.size = new Vector2(0.0001f,0.0001f);
        
        inAction = false;
    }

    IEnumerator takeDamage() {
        inAction = true;
        hpScore.removeHP(1);
        isDead = playerStats.takeDamage(1);

        if (isDead) {
            animator.Play("HeroFaint");
            StartCoroutine(playerStats.fadeOut());
        }
        else {
            animator.Play("HeroHurt");
            yield return new WaitForSeconds(0.3f);
            inAction = false;
        }
    }

}
