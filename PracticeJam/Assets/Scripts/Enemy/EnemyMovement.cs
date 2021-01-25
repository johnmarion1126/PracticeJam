using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rigidbodyEnemy;
    public Animator animatorEnemy;
    public SpriteRenderer spriteRendererEnemy;

    public GameObject enemyObject;
    public BoxCollider2D enemyAttack;

    public GameObject playerObject;

    public Transform player;
    private Vector2 movement;

    public float moveEnemySpeed = 2f;
    private bool inAction = false;
    private bool inRange = false;
    private float damaged = 0.5f;

    void Start() {
        rigidbodyEnemy = GetComponent<Rigidbody2D>();
        animatorEnemy = GetComponent<Animator>();
    }

    void Update() {
        Vector3 direction = player.position - transform.position;
        direction.Normalize();
        movement = direction;

        if (damaged < 0.5f) damaged += Time.deltaTime;
        if (inRange && !inAction && damaged >= 0.5f) StartCoroutine(delayCall());
    }

    private void FixedUpdate() {
        if (!inAction && damaged >= 0.5f) moveEnemy(movement);
    }

    void moveEnemy(Vector2 direction) {
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

        animatorEnemy.Play("BasicEnemyWalk");
        rigidbodyEnemy.MovePosition((Vector2)transform.position + (Vector2)(direction * moveEnemySpeed * Time.deltaTime));
    }

    void OnTriggerEnter2D(Collider2D playerObject) {
        inRange = true;
        rigidbodyEnemy.constraints = RigidbodyConstraints2D.FreezeAll;
        if (playerObject.name == "PlayerAttack") StartCoroutine(takeDamage());
    }

    void OnTriggerExit2D(Collider2D playerObject) {
        inRange = false;
        rigidbodyEnemy.constraints = RigidbodyConstraints2D.None;
        rigidbodyEnemy.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    IEnumerator delayCall() {
        inAction = true;
        animatorEnemy.Play("BasicEnemyIdle");
        yield return new WaitForSeconds(0.5f);
        if (inRange && damaged >= 0.5f) StartCoroutine(attackPlayer());
        else inAction = false;
    }

    IEnumerator attackPlayer() {

        if (Random.Range(1,3) == 1) {
            animatorEnemy.Play("BasicEnemyPunch");
        }
        else {
            animatorEnemy.Play("BasicEnemyJab");
        }
        
        enemyAttack.size = new Vector2(1.5f,0.5f);
        yield return new WaitForSeconds(0.3f);
        enemyAttack.size = new Vector2(0.0001f,0.0001f);
        
        inAction = false;
    }

    IEnumerator takeDamage() {
        damaged = 0f;
        animatorEnemy.Play("BasicEnemyHurt");
        yield return new WaitForSeconds(0.2f);
    }

}
