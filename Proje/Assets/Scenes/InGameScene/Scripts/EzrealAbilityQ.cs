using UnityEngine;
using UnityEngine.UI;

// Require components that are essential for this script to function properly
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(ManaSystem))]
public class EzrealAbilityQ : MonoBehaviour
{
    [Header("Ability Values")]
    public KeyCode abilityKey;
    public float cooldown = 5f;
    public float manaCost = 30f;

    [Header("Projectile")]
    public Transform spawnPoint;
    public GameObject skillshotPrefab;

    [Header("UI Elements 2D")]
    public Image abilityImageMain;
    public Image abilityImageGreyed;
    public Text abilityText;

    [Header("UI Elements 3D")]
    public Canvas abilityCanvas;
    public Image skillshotIndicator;

    private ManaSystem manaSystem;
    private Movement movement;
    private bool isCooldown = false;
    private float currentCooldown;
    private Vector3 aimPosition;

    private Camera mainCamera;
    private Animator anim;

    void Awake()
    {
        CacheComponents();
        InitializeUI();
    }

    void Update()
    {
        HandleInput();
        UpdateCooldown();
        UpdateUI();

        if (skillshotIndicator.enabled) UpdateSkillshotIndicator();
    }

    // Cache frequently used components to improve performance
    private void CacheComponents()
    {
        manaSystem = GetComponent<ManaSystem>();
        movement = GetComponent<Movement>();
        mainCamera = Camera.main;

        abilityCanvas ??= GetComponentInChildren<Canvas>();
        skillshotIndicator ??= GetComponentInChildren<Image>();
    }

    // Handle user input for activating the ability and aiming
    private void HandleInput()
    {
        if (Input.GetKeyDown(abilityKey) && !isCooldown && manaSystem.CanAffordAbility(manaCost))
        {
            EnableAimingMode(true);
        }

        if (skillshotIndicator.enabled && Input.GetMouseButtonDown(0))
        {
            FireSkillshot();
        }
    }

    // Fire the skillshot ability
    private void FireSkillshot()
    {
        if (!manaSystem.CanAffordAbility(manaCost)) return;

        movement.StopMovement();
        manaSystem.UseAbility(manaCost);
        StartCooldown();
        RotateCharacter();

        EnableAimingMode(false);
        Cursor.visible = true;

        anim = GetComponent<Animator>();
        anim.SetTrigger("Ezreal Q");
    }

    // Rotate the character to face the aim direction
    private void RotateCharacter()
    {
        Vector3 direction = CalculateAimDirection();
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    // Start the cooldown timer for the ability
    private void StartCooldown()
    {
        isCooldown = true;
        currentCooldown = cooldown;
    }

    // Instantiate the projectile at the correct position and rotation
    public void AimAndFireProjectile()
    {
        if (skillshotPrefab)
        {
            Instantiate(skillshotPrefab, spawnPoint.position, spawnPoint.rotation);
            movement.ResumeMovement();
        }
    }

    // Calculate the direction of the aim based on mouse position
    private Vector3 CalculateAimDirection()
    {
        Vector3 direction = (aimPosition - transform.position).normalized;
        direction.y = 0;
        return direction;
    }

    // Update the cooldown timer and UI elements accordingly
    private void UpdateCooldown()
    {
        if (!isCooldown) return;

        currentCooldown -= Time.deltaTime;
        isCooldown = currentCooldown > 0;
    }

    // Update the UI elements based on ability state
    private void UpdateUI()
    {
        if (abilityImageGreyed)
        {
            abilityImageGreyed.color = isCooldown ? Color.grey : Color.white;
            abilityImageGreyed.fillAmount = isCooldown ? currentCooldown / cooldown : 0;
            abilityImageMain.color = manaSystem.CanAffordAbility(manaCost) ? Color.white : Color.red;
        }
        abilityText.text = isCooldown ? Mathf.Ceil(currentCooldown).ToString() : "";
    }

    // Update the position and rotation of the skillshot indicator
    private void UpdateSkillshotIndicator()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            aimPosition = hit.point;
            UpdateIndicatorRotation();
        }
    }

    // Rotate the skillshot indicator to align with the aim direction
    private void UpdateIndicatorRotation()
    {
        Quaternion targetRotation = Quaternion.LookRotation(aimPosition - transform.position);
        targetRotation.eulerAngles = new Vector3(0, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z);
        abilityCanvas.transform.rotation = targetRotation;
    }

    // Enable or disable the aiming mode UI elements
    private void EnableAimingMode(bool isEnabled)
    {
        abilityCanvas.enabled = isEnabled;
        skillshotIndicator.enabled = isEnabled;
        Cursor.visible = !isEnabled;
    }

    // Initialize UI elements to their default state
    private void InitializeUI()
    {
        abilityCanvas.enabled = false;
        skillshotIndicator.enabled = false;
        if (abilityImageGreyed) abilityImageGreyed.color = Color.white;
        abilityText.text = "";
    }
}
