using Godot;
using System;

public delegate void HealthEventHandler();
public class Health {
    public event HealthEventHandler HealthZeroEvent;

    public float Max {private set; get;}
    public float Current {private set; get;}

    public Health(float max) {
        Max = max;
        Current = max;
    }

    public void FillToMax() {
        Current = Max;
    }

    public void SetMax(float value) {
        Max = value;
    }

    public void Increment(float amount) {
        Current = Mathf.Clamp(Current + amount, 0, Max);
        if (Current <= 0) {
            HealthZeroEvent?.Invoke();
        }
    }
}
