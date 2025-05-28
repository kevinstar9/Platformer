using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float lifetime = 1f;
    private Vector3 velocity;

    void Start()
    {
        velocity = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1f, 1.5f), 0f);
        Destroy(gameObject, lifetime);
        TextMeshProUGUI tmp = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        velocity += Vector3.down * 5f * Time.deltaTime; // 중력감
    }

    public void SetText(int damage)
    {
        var tmp = GetComponent<TextMeshPro>();
        if (tmp != null)
            tmp.text = damage.ToString();
        else
            Debug.LogWarning("TMP가 안 붙어 있음");
    }
}