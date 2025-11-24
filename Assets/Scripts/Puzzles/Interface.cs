public interface IToggleObjects
{
    void TriggerOn();
    void TriggerOff();
}

public interface ITriggerObjects
{
    void Trigger();
}

public interface ILevers
{
    void StartGrip();
    void EndGrip();
    void UpdateSpeed();
}
