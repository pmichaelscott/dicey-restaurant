using TMPro;
using UnityEngine;

public class DiceSlot : MonoBehaviour
{
    [SerializeField] private Transform snapPoint;
    [SerializeField] private TMP_Text valueText;

    public Dice Occupant { get; private set; }

    private DiceValue occupantValue;

    public bool IsEmpty => Occupant == null;
    public Transform SnapPoint => snapPoint;

    public bool CanAccept(Dice dice)
    {
        return IsEmpty;
    }

    public void Occupy(Dice dice)
    {
        Occupant = dice;

        occupantValue = dice.GetComponent<DiceValue>();
        if (occupantValue != null)
            occupantValue.OnValueChanged += HandleOccupantValueChanged;

        RefreshValueText();
    }

    public void Vacate(Dice dice)
    {
        if (Occupant != dice)
            return;

        if (occupantValue != null)
            occupantValue.OnValueChanged -= HandleOccupantValueChanged;

        Occupant = null;
        occupantValue = null;
        ClearValueText();
    }

    private void HandleOccupantValueChanged(int newValue)
    {
        RefreshValueText();
    }

    public void RefreshValueText()
    {
        if (valueText == null)
            return;

        if (occupantValue != null)
            valueText.text = occupantValue.CurrentValue.ToString();
        else
            valueText.text = "-";
    }

    private void ClearValueText()
    {
        if (valueText != null)
            valueText.text = "-";
    }

    private void OnDestroy()
    {
        if (occupantValue != null)
            occupantValue.OnValueChanged -= HandleOccupantValueChanged;
    }
}