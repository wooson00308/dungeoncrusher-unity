using UnityEngine;

public class StageManager : SingletonMini<StageManager>
{
    private int _currentStage = 1;
    public int CurrentStage => _currentStage;
    public StageData _stageDatas;

    public bool IsAllStageClear => _stageDatas.stageInfos.Count <= _currentStage;

    public void ClearStage()
    {
        _currentStage++;
    }

    public StageInfo GetStageData()
    {
        if (_stageDatas.stageInfos.Count < _currentStage)
        {
            Debug.LogError("stageData 갯수보다 현재 스테이지가 더 높습니다");
            return null;
        }

        return _stageDatas.stageInfos[_currentStage - 1];
    }
}