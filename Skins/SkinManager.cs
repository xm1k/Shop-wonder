using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    private int CurrentSkin = 0;

    [SerializeField] List<Skin> Skins = new List<Skin>();

    [SerializeField] private Sprite SelectedSkin;
    [SerializeField] private Sprite DeselectedSkin;
    private UnityAnalytics UnityAnalytics;
    private void Start()
    {
        Init();
        UnityAnalytics = FindObjectOfType<UnityAnalytics>();
    }

    private void Init()
    {
        for(int i=0;i<Skins.Count;i++)
        {
            Skins[i].SkinObject.transform.Find("SkinImage").GetComponent<Image>().sprite = Skins[i].SkinImage;
            Skins[i].SkinObject.transform.Find("NameBack/NameText").GetComponent<TextMeshProUGUI>().text = Skins[i].Name;
        }
        ChangeSkin(CurrentSkin);
    }
    public void ChangeSkin(int Index)
    {
        if (Index != CurrentSkin)
        {
            Skins[CurrentSkin].SkinObject.GetComponent<Image>().sprite = DeselectedSkin;
            CurrentSkin = Index;
            Skins[CurrentSkin].SkinObject.GetComponent<Image>().sprite = SelectedSkin;
            SendToAnalytics(CurrentSkin);
            Player.SkinSelected = Skins[CurrentSkin].ID+1;
        }
    }
    private void SendToAnalytics(int Index)
    {
        if (UnityAnalytics != null)
        {
            switch (Index)
            {
                case 0:
                    UnityAnalytics.PlayerChangeSkin("STANDART");
                    break;
                case 1:
                    UnityAnalytics.PlayerChangeSkin("ZOMBIE");
                    break;
                case 2:
                    UnityAnalytics.PlayerChangeSkin("PRISONER");
                    break;
                case 3:
                    UnityAnalytics.PlayerChangeSkin("KNIGHT");
                    break;
                case 4:
                    UnityAnalytics.PlayerChangeSkin("DOG");
                    break;
                case 5:
                    UnityAnalytics.PlayerChangeSkin("BOY");
                    break;
            }
        }
    }
}
[Serializable]
public class Skin
{
    public GameObject SkinObject;
    public string Name;
    public int ID;
    public Sprite SkinImage;
}
