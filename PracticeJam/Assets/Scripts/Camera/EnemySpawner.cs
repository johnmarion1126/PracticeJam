using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject playerObject;
    [SerializeField]
    private List <GameObject> enemyObjects;
    private GameObject enemyObject;

    [SerializeField]
    private GameObject dialogBox;
    private DialogBox dialog;
    [SerializeField]
    private List <string> dialogList;

    private int numberOfEnemies = 0;
    private float playerPosition;
    private float startingPosition;
    private float differenceInPosition;
    private Vector3 spawnPosition;

    void Awake() {
        dialog = dialogBox.GetComponent<DialogBox>();
    }

    void Start() {
        startingPosition = playerObject.transform.position.x;
    }

    void Update()
    {
        playerPosition = playerObject.transform.position.x;
        differenceInPosition = Mathf.Abs(startingPosition - playerPosition);

        if (differenceInPosition >= 25.0f) {
            differenceInPosition = 0.0f;
            startingPosition = playerPosition;
            StartCoroutine(spawnEnemies());
        }
    }

    IEnumerator spawnEnemies() {
        numberOfEnemies += 1;
        dialog.addDialog(dialogList[numberOfEnemies-1]);
        for (int i = 0; i < numberOfEnemies; i++) {
            enemyObject = enemyObjects[Random.Range(0,enemyObjects.Count-1)];
            if (i % 2 == 0) spawnPosition = new Vector3(Random.Range(playerPosition+11.0f,playerPosition+15.0f),Random.Range(-2.0f,1.5f), 0f);
            else spawnPosition = new Vector3(Random.Range(playerPosition-9.0f,playerPosition-12.0f),Random.Range(-2.0f,1.5f), 0f);
            Instantiate(enemyObject, spawnPosition, Quaternion.identity);

            if (i % 2 == 1) yield return new WaitForSeconds(2.5f);
        }
        if (numberOfEnemies >= 4) {
            yield return new WaitForSeconds(4.0f);
            Instantiate(enemyObjects[enemyObjects.Count-1], new Vector3(playerPosition+15.0f, 1f, 0f), Quaternion.identity);
            yield return new WaitForSeconds(2.0f);
            dialog.addDialog(dialogList[numberOfEnemies]);
        }
    }
}
