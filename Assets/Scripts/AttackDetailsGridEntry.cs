using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

public class AttackDetailsGridEntry : MonoBehaviour
{
    [SerializeField]
    Color fastAttackColour;
    [SerializeField]
    Color specialAttackColour;

    [SerializeField]
    Text attackName;
    [SerializeField]
    Text attackType;
    [SerializeField]
    Text damage;

    [SerializeField]
    ProceduralImage proceduralImage;
    [SerializeField]
    ProceduralImage headerImage;
    [SerializeField]
    ProceduralImage typeImage;

    private Color colorThatReflectsType;

    /// <summary>
    /// Takes in the details contained in the json from the query request and sets up a UI entry based on those details
    /// </summary>
    /// <param name="_attackName">Name of the pokemons attack</param>
    /// <param name="_attackType">Grass, Bug, etc etc</param>
    /// <param name="_damage">Numerical value of damge the attack does</param>
    /// <param name="isFastAttack">fast or special</param>
    public void SetDetails(string _attackName, string _attackType, string _damage, bool isFastAttack)
    {
        attackName.text = _attackName.ToUpper();
        attackType.text = _attackType.ToUpper();
        damage.text = _damage;
        proceduralImage.fillAmount = float.Parse(damage.text)/100f;

        SetUIColourBasedOnAttackType(isFastAttack);
    }

    private void SetUIColourBasedOnAttackType(bool isThisMoveAFastAttack)
    {
        if (isThisMoveAFastAttack)
        {
            colorThatReflectsType = fastAttackColour;
        }
        else
        {
            colorThatReflectsType = specialAttackColour;
        }

        headerImage.color = colorThatReflectsType;
        typeImage.color = colorThatReflectsType;
        proceduralImage.color = colorThatReflectsType;
    }
}
