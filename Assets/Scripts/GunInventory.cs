using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MenuStyle
{
    horizontal,
    vertical
}

public class GunInventory : MonoBehaviour
{
    public GameObject currentGun;
    private Animator currentHAndsAnimator;
    private int currentGunCounter = 0;

    public List<string> gunsIHave = new List<string>();
    public Texture[] icons;

    [HideInInspector]
    public float switchWeaponCooldown;

    public bool isInventoryVisible = true;

    void Awake()
    {
        InitializeInventory();
    }

    void Update()
    {
        UpdateCooldown();
        ProcessWeaponSwitch();
    }

    [Header("GUI Gun preview variables")]
    public MenuStyle menuStyle = MenuStyle.horizontal;
    public int spacing = 10;
    public Vector2 beginPosition;
    public Vector2 size;

    [Header("Sounds")]
    public AudioSource weaponChanging;

    private void InitializeInventory()
    {
        StartCoroutine("UpdateIconsFromResources");
        StartCoroutine("SpawnWeaponUponStart");

        if (gunsIHave.Count == 0)
        {
        }
    }

    IEnumerator SpawnWeaponUponStart()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("Spawn", 0);
    }

    private void UpdateCooldown()
    {
        switchWeaponCooldown += 1 * Time.deltaTime;
    }

    private void ProcessWeaponSwitch()
    {
        if (switchWeaponCooldown > 1.2f && Input.GetKey(KeyCode.LeftShift) == false)
        {
            HandleWeaponInput();
        }
    }

    IEnumerator UpdateIconsFromResources()
    {
        yield return new WaitForEndOfFrame();

        icons = new Texture[gunsIHave.Count];
        for (int i = 0; i < gunsIHave.Count; i++)
        {
            icons[i] = (Texture)Resources.Load("Weap_Icons/" + gunsIHave[i].ToString() + "_img");
        }
    }

    void HandleWeaponInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            SwitchToNextWeapon();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            SwitchToPreviousWeapon();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && currentGunCounter != 0)
        {
            SwitchToWeaponSlot(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && currentGunCounter != 1)
        {
            SwitchToWeaponSlot(1);
        }
    }

    private void SwitchToNextWeapon()
    {
        switchWeaponCooldown = 0;
        currentGunCounter++;
        
        if (currentGunCounter > gunsIHave.Count - 1)
        {
            currentGunCounter = 0;
        }
        
        StartCoroutine("Spawn", currentGunCounter);
    }

    private void SwitchToPreviousWeapon()
    {
        switchWeaponCooldown = 0;
        currentGunCounter--;
        
        if (currentGunCounter < 0)
        {
            currentGunCounter = gunsIHave.Count - 1;
        }
        
        StartCoroutine("Spawn", currentGunCounter);
    }

    private void SwitchToWeaponSlot(int slotIndex)
    {
        switchWeaponCooldown = 0;
        currentGunCounter = slotIndex;
        StartCoroutine("Spawn", currentGunCounter);
    }

    IEnumerator Spawn(int weaponIndex)
    {
        PlayWeaponChangeSound();

        if (currentGun)
        {
            if (currentGun.name.Contains("Gun"))
            {
                yield return SpawnGunWeapon(weaponIndex);
            }
            else if (currentGun.name.Contains("Sword"))
            {
                yield return SpawnMeleeWeapon(weaponIndex);
            }
        }
        else
        {
            InstantiateWeapon(weaponIndex);
        }
    }

    private void PlayWeaponChangeSound()
    {
        if (weaponChanging)
        {
            weaponChanging.Play();
        }
    }

    private IEnumerator SpawnGunWeapon(int weaponIndex)
    {
        currentHAndsAnimator.SetBool("changingWeapon", true);
        yield return new WaitForSeconds(0.8f);
        
        Destroy(currentGun);
        InstantiateWeapon(weaponIndex);
    }

    private IEnumerator SpawnMeleeWeapon(int weaponIndex)
    {
        currentHAndsAnimator.SetBool("changingWeapon", true);
        yield return new WaitForSeconds(0.25f);

        currentHAndsAnimator.SetBool("changingWeapon", false);
        yield return new WaitForSeconds(0.6f);
        
        Destroy(currentGun);
        InstantiateWeapon(weaponIndex);
    }

    private void InstantiateWeapon(int weaponIndex)
    {
        GameObject resource = (GameObject)Resources.Load(gunsIHave[weaponIndex].ToString());
        currentGun = (GameObject)Instantiate(resource, transform.position, Quaternion.identity);
        AssignHandsAnimator(currentGun);
    }

    void AssignHandsAnimator(GameObject weaponObject)
    {
        if (weaponObject.name.Contains("Gun"))
        {
            currentHAndsAnimator = currentGun.GetComponent<Gun>().handsAnimator;
        }
    }

    void OnGUI()
    {
        if (currentGun && isInventoryVisible)
        {
            RenderWeaponIcons();
        }
    }

    private void RenderWeaponIcons()
    {
        for (int i = 0; i < gunsIHave.Count; i++)
        {
            DrawCorrespondingImage(i);
        }
    }

    void DrawCorrespondingImage(int weaponNumber)
    {
        string deleteCloneFromName = currentGun.name.Substring(0, currentGun.name.Length - 7);

        if (menuStyle == MenuStyle.horizontal)
        {
            RenderHorizontalIcon(weaponNumber, deleteCloneFromName);
        }
        else if (menuStyle == MenuStyle.vertical)
        {
            RenderVerticalIcon(weaponNumber, deleteCloneFromName);
        }
    }

    private void RenderHorizontalIcon(int weaponNumber, string currentWeaponName)
    {
        if (currentWeaponName == gunsIHave[weaponNumber])
        {
            GUI.DrawTexture(new Rect(vec2(beginPosition).x + (weaponNumber * position_x(spacing)), vec2(beginPosition).y, vec2(size).x, vec2(size).y), icons[weaponNumber]);
        }
        else
        {
            GUI.DrawTexture(new Rect(vec2(beginPosition).x + (weaponNumber * position_x(spacing) + 10), vec2(beginPosition).y + 10, vec2(size).x - 20, vec2(size).y - 20), icons[weaponNumber]);
        }
    }

    private void RenderVerticalIcon(int weaponNumber, string currentWeaponName)
    {
        if (currentWeaponName == gunsIHave[weaponNumber])
        {
            GUI.DrawTexture(new Rect(vec2(beginPosition).x, vec2(beginPosition).y + (weaponNumber * position_y(spacing)), vec2(size).x, vec2(size).y), icons[weaponNumber]);
        }
        else
        {
            GUI.DrawTexture(new Rect(vec2(beginPosition).x, vec2(beginPosition).y + 10 + (weaponNumber * position_y(spacing)), vec2(size).x - 20, vec2(size).y - 20), icons[weaponNumber]);
        }
    }

    public void DeadMethod()
    {
        Destroy(currentGun);
        Destroy(this);
    }

    private float position_x(float var)
    {
        return Screen.width * var / 100;
    }

    private float position_y(float var)
    {
        return Screen.height * var / 100;
    }

    private float size_x(float var)
    {
        return Screen.width * var / 100;
    }

    private float size_y(float var)
    {
        return Screen.height * var / 100;
    }

    private Vector2 vec2(Vector2 _vec2)
    {
        return new Vector2(Screen.width * _vec2.x / 100, Screen.height * _vec2.y / 100);
    }
}