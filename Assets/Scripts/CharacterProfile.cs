using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterProfile : MonoBehaviour
{
    public int woodCount;
    public TMP_Text woodCountText;
    public UnityEngine.UI.Outline outline;
    [HideInInspector]
    public Character character;


    public void SelectCharacterButtton()
    {
        character.SelectCharacter();
    }



}
