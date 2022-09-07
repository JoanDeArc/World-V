using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasHandler : MonoBehaviour
{
    public BattleAIPlayer AI;

    public Image HealthBar;
    public Image EnergyBar;

    public Image UltiBar;

    public GameObject AttackBar;

    public Image DefenceOneBar;
    public Image DefenceOneButton;
    public Image DefenceTwoBar;
    public Image DefenceTwoButton;

    public Image[] AttackImages;

    // Start is called before the first frame update
    void Start()
    {
        SelectAttack(0);
    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.fillAmount = AI.Health * 0.01f;
        //EnergyBar.fillAmount = AI.Energy * 0.01f;

        UltiBar.fillAmount = AI.UltiCharge;

        DefenceOneBar.fillAmount = AI.GetDefenceChance(AI.DefenceType1);
        DefenceTwoBar.fillAmount = AI.GetDefenceChance(AI.DefenceType2);
    }

    public void ToggleAttackBar()
    {
        AttackBar.SetActive(!AttackBar.activeSelf);

        if (AttackBar.activeSelf)
        {
            DefenceOneButton.color = Color.gray;
            DefenceTwoButton.color = Color.gray;
        }
        else
        {
            DefenceOneButton.color = Color.white;
            DefenceTwoButton.color = Color.white;
        }
    }

    public void SelectAttack(int index)
    {
        for (int i = 0; i < AttackImages.Length; i++)
        {
            if (i == index)
                AttackImages[i].color = Color.red;
            else
                AttackImages[i].color = Color.white;
        }
    }
}
