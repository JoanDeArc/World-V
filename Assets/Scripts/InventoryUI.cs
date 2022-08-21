using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;

// Controlls the inventory on the screen, handles input etc.
public class InventoryUI : MonoBehaviour
{
    public Inventory Inventory;
    public Transform PanelChoice;
    public Transform ChoiceCover;
    private int indexChoice;
    public Transform Slots;
    public Sprite SlotEmptyIcon;
    private Image[] imagesSlots;
    public int indexSelected;

    public Image imageSelected;
    public Text textName;
    public Text textAbout;

    public bool IsOpen;
    private System.Type typeLookingFor;

    // Start is called before the first frame update
    void Start()
    {
        indexChoice = 0;
        indexSelected = 0;
    }

    public void ConnectToInventory()
    {
        Inventory.onItemChangedCallback += UpdateUI;

        imagesSlots = Slots.GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOpen)
        {
            if (PanelChoice.gameObject.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow) && indexChoice < 3)
                {
                    indexChoice++;
                    ChoiceCover.transform.localPosition = new Vector3(ChoiceCover.transform.localPosition.x, ChoiceCover.transform.localPosition.y - 1, ChoiceCover.transform.localPosition.z);
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow) && indexChoice > 0)
                {
                    indexChoice--;
                    ChoiceCover.transform.localPosition = new Vector3(ChoiceCover.transform.localPosition.x, ChoiceCover.transform.localPosition.y + 1, ChoiceCover.transform.localPosition.z);
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (indexChoice == 0)
                        Debug.Log("Use");
                    else if (indexChoice == 1)
                    {
                        Player.instance.Feed(Inventory.items[indexSelected]);
                        Close();
                    }
                    else if (indexChoice == 2)
                        Debug.Log("Info");
                    else if (indexChoice == 3)
                    {
                        Inventory.RemoveItem(Inventory.items[indexSelected]);
                        UpdateInfo();
                    }

                    PanelChoice.gameObject.SetActive(false);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                    MoveSelection(4);
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                    MoveSelection(-4);
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    MoveSelection(-1);
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                    MoveSelection(1);
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (typeLookingFor != null)
                    {
                        typeLookingFor = null;
                    }
                    else
                    {
                        if (Inventory.items[indexSelected] != null)
                            if (!PanelChoice.gameObject.activeSelf)
                            {
                                PanelChoice.transform.position = imagesSlots[indexSelected].transform.position;
                                PanelChoice.transform.Translate(new Vector3(0.9f, -0.3f, 0));
                                PanelChoice.gameObject.SetActive(true);
                            }
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Tab))
                    Close();
            }
        }
    }

    public void Open()
    {
        UpdateInfo();

        imagesSlots[indexSelected].color = Color.gray;
        gameObject.SetActive(true);
        IsOpen = true;
    }

    public void Close()
    {
        ChoiceCover.transform.localPosition = new Vector3(ChoiceCover.transform.localPosition.x, 1.5f, ChoiceCover.transform.localPosition.z);
        PanelChoice.gameObject.SetActive(false);
        indexChoice = 0;

        typeLookingFor = null;

        gameObject.SetActive(false);
        IsOpen = false;
    }

    public async Task<int> ShowChooseType(System.Type type)
    {
        Open();

        typeLookingFor = type;
        while (typeLookingFor != null)
        {
            await Task.Delay(1);
        }

        Close();
        return indexSelected;
    }

    private void MoveSelection(int amount)
    {
        if (indexSelected + amount < 0 || indexSelected + amount >= Inventory.space)
            return;

        imagesSlots[indexSelected].color = Color.white;
        indexSelected += amount;
        imagesSlots[indexSelected].color = Color.gray;

        UpdateInfo();
    }

    private void UpdateInfo()
    {
        if (Inventory.items[indexSelected] != null)
        {
            imageSelected.sprite = Inventory.items[indexSelected].Icon;
            imageSelected.gameObject.SetActive(true);
            textName.text = Inventory.items[indexSelected].Name;
            textAbout.text = Inventory.items[indexSelected].About;
        }
        else
        {
            imageSelected.gameObject.SetActive(false);
            textName.text = "";
            textAbout.text = "";
        }
    }

    void UpdateUI(ItemData data, int indexSlot, bool adding)
    {
        if (adding)
        {
            imagesSlots[indexSlot].sprite = data.Icon;
            imagesSlots[indexSlot].enabled = true;
        }
        else
        {
            imagesSlots[indexSlot].enabled = false;
        }
    }
}
