using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// Created by Alexander Anokhin

public class InterfaceController : MonoBehaviour {

    // Config
    public TextMeshProUGUI scoreMultText;
    public TextMeshProUGUI miningText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreEndText;
    public Image gameOverScreen;
    public Button gameOverButton;
    public TextMeshProUGUI creditsText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI comboText;

    public Sprite heartSprite;
    public Sprite heartBrokenSprite;
    public Sprite heartTempSprite;

    public List<Image> hearts;

    public float comboDissapearTime = 1;
    private float comboDissapearTimer;
    private int prevCombo;

    // Cache
    private Player player;

    public void Start() {
        gameOverScreen.gameObject.SetActive(false);
        gameOverButton.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        scoreEndText.gameObject.SetActive(false);
        creditsText.gameObject.SetActive(false);

        comboDissapearTimer = comboDissapearTime;
        comboText.enabled = false;
        player = FindObjectOfType<Player>();
        prevCombo = 0;
    }

    public void Update() {
        if (player == null) {
            Debug.Log("no player");
        }
        scoreMultText.text = "SCORE X" + player.scoreMultiplier;
        miningText.text = "MINING " + player.miningMultiplier;
        scoreText.text = "" + player.score;
        goldText.text = "" + player.gold;

        comboDissapearTimer -= Time.deltaTime;
        if (comboDissapearTimer <= 0) {
            comboText.enabled = false;
        }
    }

    public void UpdateHearts() {
        for (int i = 0; i < 5; i++) {
            if (player.hp > i) {
                hearts[i].enabled = true;
                hearts[i].sprite = heartSprite;
            } else if (player.maxHp > i) {
                hearts[i].enabled = true;
                hearts[i].sprite = heartBrokenSprite;
            } else {
                hearts[i].enabled = false;
            }
        }
    }

    public void UpdateComboText() {
        if (player.combo < prevCombo) {
            comboText.text = "COMBO LOST";
            prevCombo = 0;
            comboDissapearTimer = comboDissapearTime;
            comboText.enabled = true;
        } else if (player.combo > prevCombo) {
            comboText.text = player.combo + " COMBO " + player.combo;
            prevCombo = player.combo;
            comboDissapearTimer = comboDissapearTime;
            comboText.enabled = true;
        }
    }

    public void GameOver(int s) {
        gameOverScreen.gameObject.SetActive(true);
        gameOverButton.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);
        scoreEndText.gameObject.SetActive(true);
        creditsText.gameObject.SetActive(true);
        scoreEndText.text = "Score " + s;
    }

    public void RegenerateLevel() {
        SceneManager.LoadScene(0);
    }
}
