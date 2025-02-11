using System.Collections;
using UnityEngine;

public abstract class PopupPresenter<V, M> : BasePresenter<V, M>, IPopup
    where V : BaseView
    where M : BaseModel
{
    private Coroutine _showCoroutine;

    public Transform tr => transform;

    public virtual void Initialize()
    {
        _view.BindUI();
        _model.Initialize();
        gameObject.SetActive(false);
    }

    public void Show()
    {
        if (_showCoroutine != null) return;
        gameObject.SetActive(true);
        _showCoroutine = StartCoroutine(RunShow());
    }

    protected abstract void Open();

    private IEnumerator RunShow()
    {
        // ���ε� �ð� ���
        yield return null;
        Open();
    }

    public virtual void Hide()
    {
    }

    public virtual void ClosePopupUI()
    {
        if (_showCoroutine != null)
            StopCoroutine(_showCoroutine);
        
        Hide();
        UIManager.Instance.ClosePopupUI(this);
    }
}