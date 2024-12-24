using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Abilities : MonoBehaviour
{
    [Header("Ability 1")]
    public Image abilityImage1;            // Yetenek 1 i�in Image bile�eni
    public Text abilityText1;              // Yetenek 1 i�in metin bile�eni
    public KeyCode ability1Key;            // Yetenek 1 i�in atanm�� tu�
    public float ability1Cooldown = 5;     // Yetenek 1'in yeniden kullan�labilirlik s�resi (cooldown)
    public float abilityManaCost = 30;     // Yetenek 1'in mana maliyeti
    public Canvas ability1Canvas;          // Yetenek 1'e ait Canvas bile�eni
    public Image ability1Skillshot;        // Yetenek 1 i�in g�rsel g�sterge (skillshot)

    [Header("Ability 2")]
    public Image abilityImage2;            // Yetenek 2 i�in Image bile�eni
    public Text abilityText2;              // Yetenek 2 i�in metin bile�eni
    public KeyCode ability2Key;            // Yetenek 2 i�in atanm�� tu�
    public float ability2Cooldown = 7;     // Yetenek 2'nin yeniden kullan�labilirlik s�resi (cooldown)
    public float ability2ManaCost = 30;     // Yetenek 2'nin mana maliyeti
    public Canvas ability2Canvas;          // Yetenek 2'ye ait Canvas bile�eni
    public Image ability2RangeIndicator;   // Yetenek 2 i�in menzil g�stergesi
    public float maxAbility2Distance = 7;  // Yetenek 2'nin maksimum menzili

    [Header("Ability 3")]
    public Image abilityImage3;            // Yetenek 3 i�in Image bile�eni
    public Text abilityText3;              // Yetenek 3 i�in metin bile�eni
    public KeyCode ability3Key;            // Yetenek 3 i�in atanm�� tu�
    public float ability3Cooldown = 10;    // Yetenek 3'�n yeniden kullan�labilirlik s�resi (cooldown)
    public float ability3ManaCost = 30;    // Yetenek 3'�n mana maliyeti
    public Canvas ability3Canvas;          // Yetenek 3'e ait Canvas bile�eni
    public Image ability3Cone;             // Yetenek 3 i�in koni g�sterge

    // Cooldown durumlar�n� kontrol eden bool de�i�kenler
    private bool isAbility1Cooldown = false;
    private bool isAbility2Cooldown = false;
    private bool isAbility3Cooldown = false;

    // Mevcut cooldown s�relerini takip eden de�i�kenler
    private float currentAbility1Cooldown;
    private float currentAbility2Cooldown;
    private float currentAbility3Cooldown;

    // Fare i�aret�isine g�re ���n atma de�i�kenleri
    private Vector3 position;
    private RaycastHit hit;
    private Ray ray;

    // Mana sistemini referans almak i�in kullan�lacak de�i�ken
    public ManaSystem manaSystem;

    void Start()
    {
        // Ba�lang��ta mana sistemi bile�enini al
        manaSystem = GetComponent<ManaSystem>();

        // Yetenek 1, 2 ve 3 i�in g�rselleri ve metinleri s�f�rla
        abilityImage1.fillAmount = 0;
        abilityImage2.fillAmount = 0;
        abilityImage3.fillAmount = 0;

        abilityText1.text = "";
        abilityText2.text = "";
        abilityText3.text = "";

        // Yetenek 1, 2 ve 3'e ait �zel g�stergeleri ba�lang��ta devre d��� b�rak
        ability1Skillshot.enabled = false;
        ability2RangeIndicator.enabled = false;
        ability3Cone.enabled = false;

        // Yetenek 1, 2 ve 3'e ait Canvas bile�enlerini ba�lang��ta devre d��� b�rak
        ability1Canvas.enabled = false;
        ability2Canvas.enabled = false;
        ability3Canvas.enabled = false;
    }

    void Update()
    {
        // Fare i�aret�isine g�re ���n atma i�lemi
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Yetenek 1, 2 ve 3 i�in girdi kontrolleri
        Ability1Input();
        Ability2Input();
        Ability3Input();

        // Yetenek 1, 2 ve 3'�n cooldown s�relerini kontrol etme ve g�ncelleme
        AbilityCooldown(ability1Cooldown, abilityManaCost, ref currentAbility1Cooldown, ref isAbility1Cooldown, abilityImage1, abilityText1);
        AbilityCooldown(ability2Cooldown, ability2ManaCost, ref currentAbility2Cooldown, ref isAbility2Cooldown, abilityImage2, abilityText2);
        AbilityCooldown(ability3Cooldown, ability3ManaCost, ref currentAbility3Cooldown, ref isAbility3Cooldown, abilityImage3, abilityText3);

        // Yetenek 1, 2 ve 3 i�in �zel Canvas i�lemleri
        Ability1Canvas();
        Ability2Canvas();
        Ability3Canvas();
    }

    // Yetenek 1 i�in �zel Canvas y�netimi
    private void Ability1Canvas()
    {
        if (ability1Skillshot.enabled)
        {
            // Fare i�aret�isine g�re ���n at, vuru� noktas�n� al
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }

            // Kameraya g�re Canvas'i d�nd�r
            Quaternion ab1Canvas = Quaternion.LookRotation(position - transform.position);
            ab1Canvas.eulerAngles = new Vector3(0, ab1Canvas.eulerAngles.y, ab1Canvas.eulerAngles.z);
            ability1Canvas.transform.rotation = Quaternion.Lerp(ab1Canvas, ability1Canvas.transform.rotation, 0);
        }
    }

    // Yetenek 2 i�in �zel Canvas y�netimi
    private void Ability2Canvas()
    {
        // Oyuncu d���nda t�m katmanlar i�in raycast kullan
        int layerMask = ~LayerMask.GetMask("Player");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // Oyuncu d���nda bir hedefi vur, pozisyonu al
            if (hit.collider.gameObject != this.gameObject)
            {
                position = hit.point;
            }
        }

        // Hedefe do�ru bir mesafe ayarla
        var hitPosDir = (hit.point - transform.position).normalized;
        float distance = Vector3.Distance(hit.point, transform.position);
        distance = Mathf.Min(distance, maxAbility2Distance);

        var newKitPos = transform.position + hitPosDir * distance;
        ability2Canvas.transform.position = (newKitPos);
    }

    // Yetenek 3 i�in �zel Canvas y�netimi
    private void Ability3Canvas()
    {
        if (ability3Cone.enabled)
        {
            // Fare i�aret�isine g�re ���n at, vuru� noktas�n� al
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }

            // Kameraya g�re Canvas'i d�nd�r
            Quaternion ab3Canvas = Quaternion.LookRotation(position - transform.position);
            ab3Canvas.eulerAngles = new Vector3(0, ab3Canvas.eulerAngles.y, ab3Canvas.eulerAngles.z);
            ability3Canvas.transform.rotation = Quaternion.Lerp(ab3Canvas, ability3Canvas.transform.rotation, 0);
        }
    }

    // Yetenek 1 girdi kontrol�
    private void Ability1Input()
    {
        if (Input.GetKeyDown(ability1Key) && !isAbility1Cooldown && manaSystem.CanAffordAbility(abilityManaCost))
        {
            // Yetenek 1 i�in Canvas ve skillshot g�ster
            ability1Canvas.enabled = true;
            ability1Skillshot.enabled = true;

            // Di�er yetenekleri kapat
            ability2Canvas.enabled = false;
            ability2RangeIndicator.enabled = false;
            ability3Canvas.enabled = false;
            ability3Cone.enabled = false;

            Cursor.visible = true; // Fare imleci gizle
        }

        if (ability1Skillshot.enabled && Input.GetMouseButtonDown(0))
        {
            if (manaSystem.CanAffordAbility(abilityManaCost))
            {
                // Yetenek kullan�m�
                manaSystem.UseAbility(abilityManaCost);
                isAbility1Cooldown = true;
                currentAbility1Cooldown = ability1Cooldown;

                // Canvas ve skillshot'u kapat
                ability1Canvas.enabled = false;
                ability1Skillshot.enabled = false;
            }
        }
    }

    // Yetenek 2 girdi kontrol�
    private void Ability2Input()
    {
        if (Input.GetKeyDown(ability2Key) && !isAbility2Cooldown && manaSystem.CanAffordAbility(ability2ManaCost))
        {
            // Yetenek 2 i�in Canvas ve menzil g�stergesi g�ster
            ability2Canvas.enabled = true;
            ability2RangeIndicator.enabled = true;

            // Di�er yetenekleri kapat
            ability1Canvas.enabled = false;
            ability1Skillshot.enabled = false;
            ability3Canvas.enabled = false;
            ability3Cone.enabled = false;

            Cursor.visible = true; // Fare imleci gizle        
        }

        if (ability2Canvas.enabled && Input.GetMouseButtonDown(0))
        {
            if (manaSystem.CanAffordAbility(ability2ManaCost))
            {
                // Yetenek kullan�m�
                manaSystem.UseAbility(ability2ManaCost);
                isAbility2Cooldown = true;
                currentAbility2Cooldown = ability2Cooldown;

                // Canvas ve menzil g�stergesini kapat
                ability2Canvas.enabled = false;
                ability2RangeIndicator.enabled = false;
            }
        }
    }

    // Yetenek 3 girdi kontrol�
    private void Ability3Input()
    {
        if (Input.GetKeyDown(ability3Key) && !isAbility3Cooldown && manaSystem.CanAffordAbility(ability3ManaCost))
        {
            // Yetenek 3 i�in Canvas ve koni g�stergesi g�ster
            ability3Canvas.enabled = true;
            ability3Cone.enabled = true;

            // Di�er yetenekleri kapat
            ability1Canvas.enabled = false;
            ability1Skillshot.enabled = false;
            ability2Canvas.enabled = false;
            ability2RangeIndicator.enabled = false;

            Cursor.visible = true; // Fare imleci g�ster
        }

        if (ability3Cone.enabled && Input.GetMouseButtonDown(0))
        {
            if (manaSystem.CanAffordAbility(ability3ManaCost))
            {
                // Yetenek kullan�m�
                manaSystem.UseAbility(ability3ManaCost);
                isAbility3Cooldown = true;
                currentAbility3Cooldown = ability3Cooldown;

                // Canvas ve koni g�stergesini kapat
                ability3Canvas.enabled = false;
                ability3Cone.enabled = false;
            }
        }
    }

    // Yetenek cooldown s�relerini ve ikonlar� kontrol eder
    private void AbilityCooldown(float abilityCooldown, float abilityManaCost, ref float currentCooldown, ref bool isCooldown, Image skillImage, Text skillText)
    {
        if (isCooldown)
        {
            // Cooldown s�resini azalt
            currentCooldown -= Time.deltaTime;

            // Cooldown s�resi bitti�inde
            if (currentCooldown <= 0f)
            {
                isCooldown = false;
                currentCooldown = 0;
            }

            // Yetenek ikonunu gri yap ve dolulu�unu g�ster
            if (skillImage != null)
            {
                skillImage.color = Color.grey;
                skillImage.fillAmount = 1;
            }

            // Yetenek metin bilgisini g�ncelle
            if (skillText != null)
            {
                skillText.text = Mathf.Ceil(currentCooldown).ToString();
            }
        }
        else
        {
            // Mana yeterliyse yetene�i kullan�lmaya haz�r g�ster
            if (manaSystem.CanAffordAbility(abilityManaCost))
            {
                if (skillImage != null)
                {
                    skillImage.color = Color.grey;
                    skillImage.fillAmount = 0;
                }
                if (skillText != null)
                {
                    skillText.text = " ";
                }
            }
            else
            {
                // Mana yetersizse yetene�i kullan�lamaz g�ster
                if (skillImage != null)
                {
                    skillImage.color = Color.red;
                    skillImage.fillAmount = 1;
                }
                if (skillText != null)
                {
                    skillText.text = "X";
                }
            }
        }
    }
}

