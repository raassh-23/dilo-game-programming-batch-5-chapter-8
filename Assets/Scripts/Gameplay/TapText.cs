using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapText : MonoBehaviour
{
    public float spawnTime = 0.5f;

    public Text Text;

    private float _spawnTime;

    private void OnEnable() {
        _spawnTime = spawnTime;
    }

    void Update()
    {
        _spawnTime -= Time.deltaTime;
        if (_spawnTime <= 0) {
            gameObject.SetActive(false);
        } else {
            Text.CrossFadeAlpha(0f, 0.5f, false);

            if(Text.color.a <= 0f) {
                gameObject.SetActive(false);
            }
        }
    }
}
