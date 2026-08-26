using System.Threading;
using UnityEngine;

public class MonsterSpawnerDistance : MonoBehaviour
{
    public GameObject summonEffectPrefab; // 召喚エフェクト
    public GameObject monsterPrefab;   // 出したいモンスター
    public float spawnDistance = 9f;   // プレイヤーがこの距離まで来たら発動
    public Transform player;           // プレイヤーの Transform
    public float summonDelay = 0.5f;      // 召喚演出後にモンスターが出るまでの時間
    public float interval = 12f; // 何秒ごとに出すか

    private float timer = 0f;
    
    void Start()
    {
        timer = interval; // 開始時に時間の条件をクリアする。
    }

    void Update()
    {
        timer += Time.deltaTime;
            

        // プレイヤーとの距離を計算
        float dist = Vector2.Distance(transform.position, player.position);

        // 一定距離以内に入ったらモンスター生成
        if (dist <= spawnDistance && timer >= interval)
        {
            timer = 0f;
            StartCoroutine(SpawnSequence());
        } 
    }
    private System.Collections.IEnumerator SpawnSequence()
    {
        // ① 召喚エフェクト生成
        Instantiate(summonEffectPrefab, transform.position, Quaternion.identity);

        // ② 少し待つ
        yield return new WaitForSeconds(summonDelay);

        // ③ モンスター生成
        Instantiate(monsterPrefab, transform.position, Quaternion.identity);
    }
}
