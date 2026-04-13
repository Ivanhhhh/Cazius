using UnityEngine;

public class EnemySaveLoader : MonoBehaviour
{
    [SerializeField] private string enemyID;
    [SerializeField] private int enemyHealth = 100;

    [SerializeField] private bool isDead;

    //estas variables las llamas despues de LoadMyData() para saber el estado del enemigo
    public string EnemyID => enemyID;
    public int EnemyHealth => enemyHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        LoadMyData();
    }

    public void LoadMyData()
    {
        if (EnemyDatabase.Instance.TryGetEnemyData(enemyID, out EnemyInfo enemyInfo))
        {
            enemyHealth = enemyInfo.EnemyHealth;
            transform.position = enemyInfo.EnemyPos;
            isDead = enemyInfo.IsEnemyDead;
        }
        else
        {
            EnemyDatabase.Instance.SaveEnemyData(enemyID, enemyHealth, transform.position, isDead);
        }
    }

    public void SaveMyData()
    {
        EnemyDatabase.Instance.SaveEnemyData(enemyID, enemyHealth, transform.position, isDead);
    }

}