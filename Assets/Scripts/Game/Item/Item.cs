using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemData _data;
    public ItemData Data => _data;
}