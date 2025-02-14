using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int MaxHealth;
    public int CurrentHealth;
    public StatsData Stats = new StatsData();
    public GameObject Prefab;

}