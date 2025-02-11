using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReadyUI : BasePresenter<ReadyView, ReadyModel>
{
    [SerializeField] private ChoiceTable _itemnSkillChoiceTable;
    [SerializeField] private ChoiceTable _statChoiceTable;
    private int _statChoiceCount;
    public int StatChoiceCount => _statChoiceCount;

    private bool _isReady;

    public void OnEnable()
    {
        _isReady = true;
        ChoiceTable();
    }

    public async void ChoiceTable()
    {
        bool isPlayer = false;
        Unit player = null;

        while (!isPlayer) //처음에 안받아져서 받을때까지 기다림
        {
            await Awaitable.NextFrameAsync();
            player = UnitFactory.Instance.GetTeamUnits(Team.Friendly).FirstOrDefault();
            isPlayer = player != null;
        }

        if (!isPlayer) return;

        List<ChoiceData> datas = new();

        if (_isReady)
        {
            if (player.StageLevel.Value > 0)
            {
                _statChoiceCount = player.StageLevel.Value;
            }

            _isReady = false;
        }

        if (_statChoiceCount > 0)
        {
            datas = _statChoiceTable.GetRandomChoices();
        }
        else
        {
            _statChoiceCount = -1;
            datas = _itemnSkillChoiceTable.GetRandomChoices();
        }

        _view.UpdateChoiceView(datas);
    }

    public void DisCountStatChoiceCount()
    {
        _statChoiceCount--;
        GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvnet_LevelUpCount, _statChoiceCount);
        ChoiceTable();
    }
}