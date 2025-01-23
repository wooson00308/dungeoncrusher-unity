using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LinoleumDB", menuName = "Data/DB/LinoleumDB")]
public class LinoleumDB : ScriptableObject
{
    public float TickDamagePercent;
    public float TickInterval;
    public float DetectRange;

    public List<LinoleumData> LinoleumDatas = new();
    public Stack<LinoleumData> AddedLinoleumDatas = new();
}