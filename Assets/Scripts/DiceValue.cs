using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DiceValue : MonoBehaviour
{
    [Serializable]
    public struct Face
    {
        public int value;
        public Transform marker; // marker.up should point outward from the face
    }

    [SerializeField] private Face[] faces = new Face[6];

    // C# auto property
    [field: SerializeField] public int CurrentValue { get; private set; } = 1;
    

    public event Action<int> OnValueChanged;

    // Call this when the die has stopped moving
    public int ComputeAndStoreValue()
    {
        float bestDot = float.NegativeInfinity;
        int bestValue = CurrentValue;

        Vector3 up = Vector3.up;

        for (int i = 0; i < faces.Length; i++)
        {
            if (faces[i].marker == null)
                continue;

            float d = Vector3.Dot(faces[i].marker.up, up);

            if (d > bestDot)
            {
                bestDot = d;
                bestValue = faces[i].value;
            }
        }

        SetValue(bestValue);
        return CurrentValue;
    }

    private void SetValue(int newValue)
    {
        if (CurrentValue == newValue)
            return;

        CurrentValue = newValue;
        OnValueChanged?.Invoke(CurrentValue);
    }
}