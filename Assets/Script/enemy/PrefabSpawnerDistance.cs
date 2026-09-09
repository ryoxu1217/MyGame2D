using System.Threading;
using UnityEngine;

public class MonsterSpawnerDistance : MonoBehaviour
{
    public GameObject summonEffectPrefab; // 召喚エフェクト
    public GameObject monsterPrefab;      // 出したいモンスター
    public float spawnDistance = 9f;      // プレイヤーがこの距離まで来たら発動
    public Transform player;              // プレイヤーの Transform
    public float summonDelay = 0.6f;      // 召喚演出後にモンスターが出るまでの時間
    public float interval = 12f;          // 何秒ごとに出すか
    public Vector2 spawnPosition;         // 召喚位置
    public int maxSpawnCount = int.MaxValue;         // 召喚回数

    private float timer = 0f;
    private int spawnCount = 0;
    
    void Start()
    {
        timer = interval; // 開始時に時間の条件をクリアする
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (spawnCount >= maxSpawnCount)
            return;

        // プレイヤーとの距離を計算
        float dist = Vector2.Distance(spawnPosition, player.position);

        // 一定距離以内に入ったらモンスター生成
        if (dist <= spawnDistance && timer >= interval)
        {
            spawnCount += 1;
            timer = 0f;
            StartCoroutine(SpawnSequence());
        } 
    }
    private System.Collections.IEnumerator SpawnSequence()
    {
        // ① 召喚エフェクト生成
        Instantiate(
            summonEffectPrefab, 
            new Vector3(spawnPosition.x, spawnPosition.y, -1f), 
            Quaternion.identity);

        // ② 少し待つ
        yield return new WaitForSeconds(summonDelay);

        // ③ モンスター生成
        Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);

    }

    void OnDrawGizmos()
    {
        // 召喚範囲
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(spawnPosition, spawnDistance);

        // 召喚位置
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(spawnPosition, 0.1f);

    }

}
