using System.Collections.Generic;
using UnityEngine;

public class DiceTray : MonoBehaviour
{
    [Header("Roll")]
    [SerializeField] private Vector3 rollDirection = new Vector3(1, 0, 1);

    // Track dice currently inside the tray trigger
    private readonly HashSet<Dice> diceInTray = new HashSet<Dice>();

    private void OnTriggerEnter(Collider other)
    {
        var dice = other.GetComponentInParent<Dice>();
        if (dice != null)
            diceInTray.Add(dice);
    }

    private void OnTriggerExit(Collider other)
    {
        var dice = other.GetComponentInParent<Dice>();
        if (dice != null)
            diceInTray.Remove(dice);
    }

    public void RollAllInTray()
    {
        foreach (var dice in diceInTray)
        {
            if (dice == null) continue;

            // Only roll dice that are not being dragged
            // You can choose your rule here:
            if (dice.state == DiceState.Dragging) continue;

            // If a die is slotted and you don't want it to roll, skip it:
            // if (dice.state == DiceState.Slotted) continue;

            var roller = dice.GetComponent<DiceRoller>();
            if (roller == null) continue;

            roller.Roll(rollDirection);
        }
    }
}