//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class CharacterCustomization_UI : MonoBehaviour
//{
//    [SerializeField] private Button genderButton;
//    private TextMeshProUGUI genderText;
//    [SerializeField] private Button skinButton;
//    [SerializeField] private Button hairButton;
//    [SerializeField] private Button hatButton;
//    [SerializeField] private Button faceButton;
//    [SerializeField] private Button returnBotton;
//    [Header("Character")]
//    [SerializeField] private CharacterBodyEventSO characterBodyEventSO;
//    [SerializeField] private Transform characterShowPositon;
//    [SerializeField] private CharacterCustomization character;

//    private SkinType bodyType;

//    private void Awake()
//    {
//        genderText = genderButton.GetComponentInChildren<TextMeshProUGUI>();

//        genderButton.onClick.AddListener(() => ChangeGender());
//        skinButton.onClick.AddListener(() => ChangeSkin());
//        hairButton.onClick.AddListener(() => ChangeHair());
//        hatButton.onClick.AddListener(() => ChangeHat());
//        faceButton.onClick.AddListener(() => ChangeFace());
//        returnBotton.onClick.AddListener(() => ResetCharacterTransform());
//    }

//    private void OnEnable()
//    {
//        characterBodyEventSO.RasieEvent(this);
//    }

//    private void OnDisable()
//    {

//    }

//    public void GetCharacter(CharacterCustomization characterCustomization)
//    {
//        this.character = characterCustomization;
//        character.transform.localPosition = characterShowPositon.position;
//    }

//    private void ResetCharacterTransform()
//    {
//        character.transform.localPosition = new Vector3(0f, 0f, -13f);
//    }

//    public void ChangeGender()
//    {
//        if (character.Gender == Gender.Male)
//        {
//            character.SetGender(Gender.Female);
//            bodyType = SkinType.FemaleBody;
//            genderText.text = "性别：女";
//        }
//        else
//        {
//            character.SetGender(Gender.Male);
//            bodyType = SkinType.MaleBody;
//            genderText.text = "性别：男";
//        }
//    }

//    public void ChangeSkin()
//    {
//        character.ChangeSkin(bodyType);
//    }

//    public void ChangeHat()
//    {
//        character.ChangeSkin(SkinType.Hat);
//    }

//    public void ChangeHair()
//    {
//        character.ChangeSkin(SkinType.Hair);
//    }

//    public void ChangeFace()
//    {
//        character.ChangeSkin(SkinType.Face);
//    }
//}
