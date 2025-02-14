using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int Health;
    public StatsData Stats = new StatsData();
    public GameObject Prefab;
}