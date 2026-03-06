using UnityEngine;

public class DiceSlot : MonoBehaviour
{
    public Transform snapPoint;
    public Dice Occupant { get; private set; }

    public bool isEmpty => Occupant == null;


    public bool CanAccept(Dice dice)
    {
        
        return isEmpty;
    }

    public void Occupy(Dice dice)
    {
        Occupant = dice;
    }

    public void Vacate(Dice dice)
    {
        if (Occupant == dice)
            Occupant = null;
    }
}
