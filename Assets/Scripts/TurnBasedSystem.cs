using UnityEngine;
using UnityEngine.UIElements;

public class TurnBasedSystem : MonoBehaviour
{
    public Label turnText;
    public VisualElement root;
    private bool playerTurn = true;
    private int enemyTurnCount = 0;
    private int maxEnemyTurns = 12;
    private float enemyTurnDelay = 5f;



    private void Start()
    {
        turnText = GetComponent<UIDocument>().rootVisualElement.Q<Label>("label");
        turnText.text = "Player's Turn";

        root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("Watter").clicked += PlayerButtonClicked;
        root.Q<Button>("Air").clicked += PlayerButtonClicked;
        root.Q<Button>("Land").clicked += PlayerButtonClicked;
    }


    private void PlayerButtonClicked()
    {
        if (playerTurn)
        {
            DisableButtons();
            PerformPlayerAction();
            StartCoroutine(EnemyTurnDelay());
        }
    }

    private void PerformPlayerAction()
    {
        Debug.Log("Player action performed");
    }

    private System.Collections.IEnumerator EnemyTurnDelay()
    {
        turnText = GetComponent<UIDocument>().rootVisualElement.Q<Label>("label");
        yield return new WaitForSeconds(enemyTurnDelay);
        if (enemyTurnCount < maxEnemyTurns)
        {
            PerformEnemyAction();
            enemyTurnCount++;
            if (enemyTurnCount == maxEnemyTurns)
            {
                turnText.text = "GG WP ! :D";
            }
            else
            {
                playerTurn = true;
                EnableButtons();
                turnText.text = "Player's Turn";
            }
        }
    }

    private void PerformEnemyAction()
    {
        turnText.text = "Ennemies Turn";
        Debug.Log("Enemy action performed");
    }

    private void EnableButtons()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("Watter").SetEnabled(true);
        root.Q<Button>("Air").SetEnabled(true);
        root.Q<Button>("Land").SetEnabled(true);
    }

    private void DisableButtons()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("Watter").SetEnabled(false);
        root.Q<Button>("Air").SetEnabled(false);
        root.Q<Button>("Land").SetEnabled(false);
    }
}
