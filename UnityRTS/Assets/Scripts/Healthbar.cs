using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Takes care of healhbar related stuff, like updating the sprite
/// </summary>
public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image _healthbarSprite;
    [SerializeField] private float reduceSpeed = 2f;
    private float target = 1f;
    Camera myCam;

    void Awake()
    {
        myCam = Camera.main;
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        target = currentHealth / maxHealth;
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - myCam.transform.position);
        _healthbarSprite.fillAmount = Mathf.MoveTowards(_healthbarSprite.fillAmount, target, reduceSpeed * Time.deltaTime);
    }

}
