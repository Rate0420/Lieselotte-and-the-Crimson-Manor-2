using UnityEngine;

public class Test04 : MonoBehaviour
{
    [SerializeField] WeaponInventory weaponInventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        weaponInventory.LoadInventory();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            weaponInventory.SaveInventory();
        }
    }
}
