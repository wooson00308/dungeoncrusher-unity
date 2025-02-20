public class MainModel : BaseModel
{
    public override void Initialize()
    {
    }

    public void ChangeGameSpeed()
    {
        TimeManager.Instance.ChangeTimeScale();
    }

    public int GetGameSpeed()
    {
        return TimeManager.Instance.GetGameSpeed();
    }
}