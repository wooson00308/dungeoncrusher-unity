using System.Collections.Generic;
using UnityEngine;
public class ReadyUI : BasePresenter<ReadyView, ReadyModel>
{
    [SerializeField] private ChoiceTable _choiceTable;

    public void OnEnable()
    {
        List<ChoiceData> datas = _choiceTable.GetRandomChoices();
        _view.UpdateChoiceView(datas);
    }
}