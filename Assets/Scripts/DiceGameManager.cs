using UnityEngine;

public class DiceGameManager : MonoBehaviour
{
    [SerializeField] private PointerInput input;
    [SerializeField] private DiceRoller[] diceRollers;
    [SerializeField] private Vector3 rollDirection = new Vector3(1, 0, 1);

    public bool CanInteract { get; private set; } = true;

    private void Awake()
    {
        if (input == null) input = FindFirstObjectByType<PointerInput>();
    }

    private void Update()
    {
        if (input != null && input.RollPressedThisFrame)
            RollAll();
    }

    public void RollAll()
    {
        CanInteract = false;

        foreach (var dr in diceRollers)
            dr.Roll(rollDirection);

        StartCoroutine(WaitForAllDiceToSettle());
    }

    private System.Collections.IEnumerator WaitForAllDiceToSettle()
    {
        while (true)
        {
            bool allIdle = true;

            foreach (var dr in diceRollers)
            {
                var dice = dr.GetComponent<Dice>();
                if (dice.state == DiceState.Rolling)
                {
                    allIdle = false;
                    break;
                }
            }

            if (allIdle)
                break;

            yield return null;
        }

        CanInteract = true;
    }
}