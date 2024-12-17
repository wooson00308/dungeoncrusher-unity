using UnityEngine;

public class FinalAttackEffect : MonoBehaviour
{
    [SerializeField] protected GameObject FinalFx;
    private Unit player;
    private Unit target;
    public void OnInitialized(Unit player, Unit target)
    {

    }
    private void Awake()
    {
        //GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_OnSpecialDeath.ToString(),);
    }
    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
    }
}