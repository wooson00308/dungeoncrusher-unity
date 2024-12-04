using System.Collections.Generic;
using UnityEngine;

public class ReadyView : BaseView
{
    [SerializeField] private List<Transform> _slots;

    private void OnEnable()
    {
        BindUI();
    }

    public void UpdateChoiceView(List<ChoiceData> tripleChoices)
    {
        int index = 0;
        foreach (var choice in tripleChoices)
        {
            var prefab = ResourceManager.Instance.SpawnFromPath(choice.GetPrefabPath, _slots[index++]);
            var choiceView = prefab.GetComponent<ChoiceView>();

            choiceView.SetupUI(choice);
        }
    }

    public override void BindUI()
    {
        
    }
}