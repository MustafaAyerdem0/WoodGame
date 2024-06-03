using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterProfile : MonoBehaviour
{
    public TMP_Text woodCount;
    public Outline outline;
    public Character character;
    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    public void SelectCharacter()
    {
        outline.enabled = false;
    }

    public void UnSelectCharacter()
    {
        outline.enabled = true;
    }


}
