using JetBrains.Annotations;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotView : MonoBehaviour {
    [SerializeField] private Image cooldownFillImage;
    [SerializeField] private Image iconImage;          // the Image that displays the ability icon
    [SerializeField] private bool isQSlot;

    [Header("Keypress visual")]
    [SerializeField] private Image keyImage;
    [SerializeField] private Sprite normalKeySprite;
    [SerializeField] private Sprite pressedKeySprite;
    [SerializeField] private float pressedDuration = 0.5f;
    private Coroutine pressedRoutine;

    [SerializeField] private AudioSource cooldownAudio;
    [SerializeField] private AudioSource abilityPressed;
    private bool isOnCooldown;

    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var playerActions = player.GetComponent<PlayerActions>();
        if (playerActions == null) return;

        if (isQSlot && playerActions.qAbilityCard != null)
            SetIcon(playerActions.qAbilityCard.icon);

        if (!isQSlot && playerActions.eAbilityCard != null)
            SetIcon(playerActions.eAbilityCard.icon);

        if (keyImage != null && normalKeySprite != null)
            keyImage.sprite = normalKeySprite;
    }
    private void Update()
    {
        if (isQSlot && Input.GetKeyDown(KeyCode.Q))
        {
            PlayPressedEffect();
            if (isOnCooldown) cooldownAudio.Play();
            else abilityPressed.Play();
        }
        else if (!isQSlot && Input.GetKeyDown(KeyCode.E))
        {
            PlayPressedEffect();
            if (isOnCooldown) cooldownAudio.Play();
            else abilityPressed.Play();
        }
    }

    // 0 = on cooldown, 1 = ready (HUDController passes player's cooldown value)
    public void SetCooldownNormalized(float normalized){
        if (cooldownFillImage == null) return;
        // overlay full when on cd, empty when ready
        float fill = 1f - normalized;
        cooldownFillImage.fillAmount = Mathf.Clamp01(fill);
        isOnCooldown = normalized < 0.999f;
    }

    public void SetIcon(Sprite iconSprite)
    {
        if (iconImage == null) return;
        iconImage.sprite = iconSprite;
        iconImage.enabled = (iconSprite != null);
    }

    private void PlayPressedEffect()
    {
        if (keyImage == null || normalKeySprite == null || pressedKeySprite == null)
            return;

        if (pressedRoutine != null)
            StopCoroutine(pressedRoutine);

        pressedRoutine = StartCoroutine(PressedEffectRoutine());
    }

    private IEnumerator PressedEffectRoutine()
    {
        keyImage.sprite = pressedKeySprite;
        yield return new WaitForSeconds(pressedDuration);
        keyImage.sprite = normalKeySprite;
        pressedRoutine = null;
    }
}