using Unity.Entities;

public struct Timer : IComponentData
{
    public float ElapsedTime;
    public float TargetDuration;
 
    public Timer(float targetDuration) : this() {
        SetDuration(targetDuration);
    }
 
    public bool IsDone => ElapsedTime >= TargetDuration;

    private void SetDuration(float targetDuration) {
        ElapsedTime = 0;
        TargetDuration = targetDuration;
    }
}

public struct Chronometer : IComponentData
{
    public float ElapsedTime;
    public bool IsPaused;

    public void Run(float timeInterval)
    {
        IncreaseTime(timeInterval);
        IsPaused = false;
    }

    public void Pause() => IsPaused = true;

    public void Stop()
    {
        ElapsedTime = 0;
        IsPaused = true;
    }

    public void Reset()
    {
        ElapsedTime = 0;
        IsPaused = false;
    }

    public void IncreaseTime(float timeAmount) => ElapsedTime += timeAmount;
}
