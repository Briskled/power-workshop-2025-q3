using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class DamagePopup : MonoBehaviour
{
    [SerializeField] private float despawnTime = 2f;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color damageColor;
    [SerializeField] private Color healColor;
    [SerializeField] private Vector2 force;

    public int Value
    {
        set
        {
            var isDamage = value < 0;
            var abs = Mathf.Abs(value);
            text.text = abs.ToString();
            text.color = isDamage ? damageColor : healColor;
        }
    }

    private IEnumerator Start()
    {
        var rigid = GetComponent<Rigidbody>();
        var r = Random.value * 2 - 1;

        var relativeRight = Vector3.Cross(Camera.main.transform.forward, Vector3.up).normalized;

        var upForce = Vector3.up * force.y;
        var hForce = relativeRight * force.x * r;

        rigid.linearVelocity = upForce + hForce;

        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }
}