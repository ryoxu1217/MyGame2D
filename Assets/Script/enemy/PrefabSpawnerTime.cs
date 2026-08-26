using UnityEngine;

public class PrefabSpawnerTime : MonoBehaviour
{
    public GameObject prefab;   // 出したいプレハブ
    public float interval = 2f; // 何秒ごとに出すか

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;

            // このオブジェクトの位置にプレハブを生成
            Instantiate(prefab, transform.position, Quaternion.identity);
        }
    }
}
