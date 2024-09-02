using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Created by Alexander Anokhin

public class ShopBlock : MonoBehaviour {

    // Config
    public TextMeshProUGUI shopText;
    public Slider slider;

    public int price;
    public float holdDurationMax = 3;
    public float shopCooldownMax = 3;
    public Sprite item;

    public bool isHealth;
    public bool isScore;
    public bool isMining;

    public bool used = false;

    // Cache
    private float holdDuration;
    private SpriteRenderer spriteRenderer;
    private float shopCooldown;

    public void Start() {
        shopText.text = "" + price;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = item;
        holdDuration = 0;
        shopCooldown = shopCooldownMax;
        slider.maxValue = holdDurationMax;
        slider.minValue = 0;
        slider.value = holdDuration;
        Color c;
        foreach (Image i in slider.GetComponentsInChildren<Image>()) {
            c = i.color;
            i.color = new Color(c.r, c.g, c.b, 0);
        }
    }

    public void Update() {
        shopCooldown -= Time.deltaTime;
        if (shopCooldown <= 0) {
            // dissapear
            Color c;
            foreach (Image i in slider.GetComponentsInChildren<Image>()) {
                c = i.color;
                i.color = new Color(c.r, c.g, c.b, 0);
            }
            holdDuration = 0;
        }
    }

    public void Shopping() {
        // appear
        Color c;
        foreach (Image i in slider.GetComponentsInChildren<Image>()) {
            c = i.color;
            i.color = new Color(c.r, c.g, c.b, 255);
        }
        shopCooldown = shopCooldownMax;
        holdDuration += Time.deltaTime;
        slider.value = holdDuration;
        if (holdDuration >= holdDurationMax) {
            Player player = FindObjectOfType<Player>();
            player.gold -= price;
            player.AddScore(price * 10, false);
            if (isHealth) {
                player.maxHp = Mathf.Min(5, player.maxHp + 1);
                player.hp = Mathf.Min(player.maxHp, player.hp + 1);
                player.UpdateHearts();
                FinishShopping();
            } else if (isScore) {
                player.AddScore(1000, false);
                FinishShopping();
            } else if (isMining) {
                player.miningMultiplier += 50;
                FinishShopping();
            }
        }
    }

    private void FinishShopping() {
        Color c;
        foreach (Image i in slider.GetComponentsInChildren<Image>()) {
            c = i.color;
            i.color = new Color(c.r, c.g, c.b, 0);
        }
        shopText.enabled = false;
        spriteRenderer.enabled = false;
        used = true;
        AudioManager am = FindObjectOfType<AudioManager>();
        am.playSound(am.buySound);
    }
}
