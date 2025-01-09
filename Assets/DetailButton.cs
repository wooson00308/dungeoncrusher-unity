using UnityEngine;

public class DetailButton : MonoBehaviour
{
    [SerializeField] private GameObject _detailPanel;

    public void OnClick()
    {
        _detailPanel.SetActive(!_detailPanel.activeSelf);
    }
}