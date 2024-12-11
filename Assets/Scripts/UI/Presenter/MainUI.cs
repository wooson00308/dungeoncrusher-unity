using UnityEngine;

public class MainUI : BasePresenter<MainView, MainModel>
{
    public void ChangeGameSpeed()
    {
        _model.ChangeGameSpeed();
    }

    public int GetGameSpeed()
    {
        return _model.GetGameSpeed();
    }
}