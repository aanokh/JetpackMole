using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Created by Alexander Anokhin

public class Player : MonoBehaviour {

    // Config
    public float moveSpeed = 10f;
    // per second
    public float jetpackSpeedAdd = 1f;
    public float jetpackSpeedMax = 3f;
    public float jetpackDurationMax = 3f;
    public float jetpackCooldownMax = 6f;
    public Color jetpackDurationColor;
    public Color jetpackCooldownColor;
    public float sliderDissapearTime = 0.4f;
    public float iframesMax = 3f;
    public float blinkPeriod = 0.5f;
    public BoxCollider2D boxCollider;
    public Slider jetpackSlider;
    public BoxCollider2D feet;
    public ParticleSystem jetpackEffect;
    public int maxHp = 3;
    public int maxPower = 3;

    // seconds
    public float animationBufferMax = 0.1f;

    // per second
    public float miningSpeed = 50;

    // Cache
    private Rigidbody2D rbody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private InterfaceController interfaceController;

    public int gold = 0;
    public int score = 0;
    public int hp = 3;
    public int combo = 0;
    public int power = 0;
    public BlockProperty comboBlock;
    public List<float> comboMultiplier;

    public AudioManager audioManager;

    public int miningMultiplier = 100;
    public int scoreMultiplier = 100;

    private float jetpackDuration;
    private float jetpackCooldown;
    private float sliderDissapearTimer;
    private float iframes;
    private float blinkCounter;
    private bool blinkToggle;

    private bool dead = false;

    // seconds
    private float falling = 0;
    private float idling = 0;

    public void Start() {

        audioManager = FindObjectOfType<AudioManager>();

        comboMultiplier = new List<float> {1, 1, 2, 3, 4, 5, 6};

        hp = maxHp;
        rbody = GetComponent<Rigidbody2D>();
        interfaceController = FindObjectOfType<InterfaceController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        interfaceController.UpdateHearts();
        jetpackSlider.maxValue = jetpackDurationMax;
        jetpackSlider.minValue = 0;

        jetpackDuration = jetpackDurationMax;
        jetpackCooldown = 0;

        jetpackSlider.value = jetpackDuration;

        iframes = 0;
        blinkCounter = 0;
        blinkToggle = true;
    }

    public void Update() {
        Blink();
        Run();
        Jetpack();
        Mine();
    }

    private void Blink() {
        if (iframes > 0) {
            if (blinkCounter <= 0) {
                if (blinkToggle) {
                    spriteRenderer.color = Color.red;
                    blinkCounter = blinkPeriod;
                    blinkToggle = !blinkToggle;
                } else {
                    spriteRenderer.color = Color.white;
                    blinkCounter = blinkPeriod;
                    blinkToggle = !blinkToggle;
                }
            } else {
                blinkCounter -= Time.deltaTime;
            }
            iframes -= Time.deltaTime;
        } else {
            spriteRenderer.color = Color.white;
        }
    }

    private void Mine() {
        float controlThrow = Input.GetAxisRaw("Horizontal"); // value is between -1 to +1

        Vector3 facingDirection = transform.right;
        if (controlThrow < 0) {
            facingDirection = -facingDirection;
        }

        if (Input.GetButton("Dig")) {
            facingDirection = -transform.up;
        } else if (Input.GetButton("Jump")) {
            facingDirection = transform.up;
        }

        if (controlThrow != 0 || Input.GetButton("Dig") || Input.GetButton("Jump")) {
            Debug.DrawRay(transform.position, facingDirection, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDirection, 1f, LayerMask.GetMask("Blocks", "Shop"));

            if (hit.collider != null) {
                Block b = hit.collider.gameObject.GetComponent<Block>();
                if (b != null) {
                    if (boxCollider.IsTouching(hit.collider)) {
                        //Debug.Log("MINING " + Time.deltaTime);
                        idling = 0;
                        animator.SetBool("Mining", true);
                        b.TakeDamage(miningSpeed * (miningMultiplier / 100f) * Time.deltaTime);
                        return;
                    }
                } else {
                    ShopBlock sh = hit.collider.gameObject.GetComponent<ShopBlock>();
                    if (sh != null) {
                        if (boxCollider.IsTouching(hit.collider)) {
                            if (gold >= sh.price && !sh.used) {
                                sh.Shopping();
                            }
                        }
                    }
                }
            }
        }

        // sticky mining disable because of flashing blocks
        if (idling > animationBufferMax) {
            animator.SetBool("Mining", false);
        } else {
            idling += Time.deltaTime;
        }

        /*
        if (miningAnimationBuffer <= 0) {
            animator.SetBool("Mining", false);
        } else {
            miningAnimationBuffer -= Time.deltaTime;
        }


        if (controlThrow != 0) {
            if (miningLastFrame) {
                miningAnimationBuffer = animationBufferMax;
                miningLastFrame = false;
            }
        }*/
    }

    private void Run() {
        float controlThrow = Input.GetAxisRaw("Horizontal"); // value is between -1 to +1

        if (controlThrow < 0) {
            spriteRenderer.flipX = true;
        } else if (controlThrow > 0) {
            spriteRenderer.flipX = false;
        }

        animator.SetBool("Walking", (controlThrow != 0));

        Vector2 playerVelocity = new Vector2(controlThrow * moveSpeed, rbody.velocity.y);
        rbody.velocity = playerVelocity;
    }

    private void Jetpack() {
        /*
        if (!feet.IsTouchingLayers(LayerMask.GetMask("Blocks"))) {
            return;
        }
        */

        // sticky flying enable because of flashing block
        if (feet.IsTouchingLayers(LayerMask.GetMask("Blocks"))) {
            animator.SetBool("Flying", false);
            falling = 0;
        } else {
            if (falling > animationBufferMax) {
                animator.SetBool("Flying", true);
            } else {
                falling += Time.deltaTime;
            }
        }

        if (Input.GetButtonDown("Jump")) {
            if (jetpackCooldown <= 0) {
                jetpackEffect.Play();
                if (!dead) {
                    audioManager.startFlySound();
                }
            }
        } else if (Input.GetButtonUp("Jump")) {
            jetpackEffect.Stop();
            audioManager.stopFlySound();
        }

        //Debug.Log(jetpackCooldown);

        sliderDissapearTimer += Time.deltaTime;
        if (sliderDissapearTimer >= sliderDissapearTime) {
            Color c;
            foreach (Image i in jetpackSlider.GetComponentsInChildren<Image>()) {
                c = i.color;
                i.color = new Color(c.r, c.g, c.b, 0);
            }
        }

        if (Input.GetButton("Jump")) {
            if (jetpackCooldown <= 0) {
                if (jetpackDuration > 0) {
                    Vector2 jumpVelocityToAdd = new Vector2(0f, jetpackSpeedAdd * Time.deltaTime);
                    rbody.velocity += jumpVelocityToAdd;
                    rbody.velocity = new Vector2(rbody.velocity.x, Mathf.Min(rbody.velocity.y, jetpackSpeedMax));

                    jetpackDuration -= Time.deltaTime;
                    jetpackSlider.value = Mathf.Max(0, jetpackDuration);
                } else {
                    // duration ran out
                    jetpackCooldown = jetpackCooldownMax;
                    jetpackEffect.Stop();
                    audioManager.stopFlySound();
                    jetpackSlider.maxValue = jetpackCooldownMax;
                    jetpackSlider.fillRect.GetComponentInChildren<Image>().color = jetpackCooldownColor;
                    jetpackSlider.value = jetpackCooldown;
                    //Debug.Log("COOLING DOWN!");
                }
            }
        } else {
            if (jetpackDuration > 0) {
                if (jetpackDuration < jetpackDurationMax) {
                    jetpackDuration += (Time.deltaTime / 2);
                    jetpackSlider.value = Mathf.Min(jetpackDurationMax, jetpackDuration);
                }
            } else {
                jetpackCooldown -= Time.deltaTime;
                jetpackSlider.value = jetpackCooldown;
                if (jetpackCooldown <= 0) {
                    jetpackDuration = jetpackDurationMax;
                    jetpackSlider.maxValue = jetpackDurationMax;
                    jetpackSlider.fillRect.GetComponentInChildren<Image>().color = jetpackDurationColor;
                    jetpackSlider.value = jetpackDuration;
                }
            }
        }
    }

    public void TakeDamage() {
        if (iframes > 0) {
            return;
        }
        hp--;
        if (hp <= 0) {
            // death
            interfaceController.UpdateHearts();
            Die();
        } else {
            if (!dead) {
                audioManager.playSound(audioManager.damageSound);
            }
            interfaceController.UpdateHearts();
            iframes = iframesMax;
        }
    }

    public void Die() {
        dead = true;
        audioManager.stopFlySound();
        audioManager.playSound(audioManager.loseSound);
        Color c;
        foreach (Image i in jetpackSlider.GetComponentsInChildren<Image>()) {
            c = i.color;
            i.color = new Color(c.r, c.g, c.b, 0);
        }
        interfaceController.GameOver(score);
        spriteRenderer.enabled = false;
        boxCollider.enabled = false;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        feet.enabled = false;
        enabled = false;
    }

    public void OnSliderChange() {
        sliderDissapearTimer = 0;
        Color c;
        foreach (Image i in jetpackSlider.GetComponentsInChildren<Image>()) {
            c = i.color;
            i.color = new Color(c.r, c.g, c.b, 255);
        }
    }

    public void AddCombo(BlockProperty block) {
        if (block == comboBlock && block.hasCombo) {
            combo++;
            if (combo > 0) {
                audioManager.playNote(combo - 1);
            }
        } else {
            combo = 0;
            comboBlock = block;
        }
        interfaceController.UpdateComboText();
    }

    public float GetComboMult() {
        return comboMultiplier[Mathf.Min(combo, comboMultiplier.Count - 1)];
    }

    public void AddScore(int val, bool comboAffectsScore) {
        if (comboAffectsScore) {
            score += (int)Mathf.Floor(val * (scoreMultiplier / 100f) * GetComboMult());
        } else {
            score += (int)Mathf.Floor(val * (scoreMultiplier / 100f));
        }
    }

    public void AddMining(int val) {
        miningMultiplier += (int)Mathf.Floor(val * GetComboMult());
    }

    public void AddGold(int val) {
        gold += (int)Mathf.Floor(val * GetComboMult());
    }

    public void AddMult(int val) {
        scoreMultiplier += (int)Mathf.Floor(val * GetComboMult());
    }

    public void AddPower(int power) {
        this.power += power;
    }

    public void UpdateHearts() {
        interfaceController.UpdateHearts();
    }

    public void OnMineImpact() {
        if (!dead) {
            audioManager.playSound(audioManager.digSound);
        }
    }
}
