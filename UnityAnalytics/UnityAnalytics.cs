using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Video;

public class UnityAnalytics : MonoBehaviour
{
    private string LastUsedSkin = "STANDART";
    private int OpenedTutorialCount = 0;
    private int ForestVisitCount = 0;
    private int GameEntered = 0;
    private int MenuTGComment = 0;
    private int MenuOtherGame = 0;
    private int MenuTGChannel = 0;
    private int EndTGChannel = 0;
    private int CoinsCollected = 0;
    private int NewPotionsCreatedCount = 0;
    private int PotionsSold = 0;
    private bool GameCompleted = false;
    private async void Start()
    {
        if (GameObject.FindGameObjectsWithTag("UnityAnalytics").Length >1)
            Destroy(gameObject);
        //Debug.Log("INIT");
        await UnityServices.InitializeAsync();
        StartNext();
    }
    IAnalyticsService Inst;
    private void StartNext()
    { 
    AnalyticsService.Instance.StartDataCollection();
        Inst =  AnalyticsService.Instance;

        if (PlayerPrefs.HasKey("LastUsedSkin"))
            LastUsedSkin = PlayerPrefs.GetString("LastUsedSkin");
        if (PlayerPrefs.HasKey("OpenedTutorialCount"))
            OpenedTutorialCount = PlayerPrefs.GetInt("OpenedTutorialCount");
        if (PlayerPrefs.HasKey("ForestVisitCount"))
            ForestVisitCount = PlayerPrefs.GetInt("ForestVisitCount");
        if (PlayerPrefs.HasKey("GameEntered"))
            GameEntered = PlayerPrefs.GetInt("GameEntered");
        if (PlayerPrefs.HasKey("MenuTGComment"))
            MenuTGComment = PlayerPrefs.GetInt("MenuTGComment");
        if (PlayerPrefs.HasKey("MenuOtherGame"))
            MenuOtherGame = PlayerPrefs.GetInt("MenuOtherGame");
        if (PlayerPrefs.HasKey("MenuTGChannel"))
            MenuTGChannel = PlayerPrefs.GetInt("MenuTGChannel");
        if (PlayerPrefs.HasKey("EndTGChannel"))
            EndTGChannel = PlayerPrefs.GetInt("EndTGChannel");
        if (PlayerPrefs.HasKey("CoinsCollected"))
            CoinsCollected = PlayerPrefs.GetInt("CoinsCollected");
        if (PlayerPrefs.HasKey("NewPotionsCreatedCount"))
            NewPotionsCreatedCount = PlayerPrefs.GetInt("NewPotionsCreatedCount");
        if (PlayerPrefs.HasKey("PotionsSold"))
            PotionsSold = PlayerPrefs.GetInt("PotionsSold");


        GameEntered++;
        PlayerPrefs.SetInt("GameEntered", GameEntered);

        Analytics.CustomEvent("GAME_ENTERED: " + GameEntered.ToString());
        
    }
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Y))
          //  SaveData();
    }
    public void PlayerChangeSkin(string NewSkin) 
    {
        LastUsedSkin = NewSkin;
        PlayerPrefs.SetString("LastUsedSkin", LastUsedSkin);
    }
    public void PlayerOpenedTutorial()
    {
        OpenedTutorialCount++;
        PlayerPrefs.SetInt("OpenedTutorialCount", OpenedTutorialCount);
    }
    public void PlayerEnteredForest()
    {
        ForestVisitCount++;
        PlayerPrefs.SetInt("ForestVisitCount", ForestVisitCount);
    }
    public void PlayerClickMenuTGComment()
    {
        MenuTGComment++;
        PlayerPrefs.SetInt("MenuTGComment", MenuTGComment);
    }
    public void PlayerClickMenuOtherGames()
    {
        MenuOtherGame++;
        PlayerPrefs.SetInt("MenuOtherGame", MenuOtherGame);
    }
    public void PlayerClickMenuTGChannel()
    {
        MenuTGChannel++;
        PlayerPrefs.SetInt("MenuTGChannel", MenuTGChannel);
    }
    public void PlayerClickEndTGChannel()
    {
        EndTGChannel++;
        PlayerPrefs.SetInt("EndTGChannel", EndTGChannel);
    }
    public void UpdateCoinsCollected(int CoinsAdded)
    {
        CoinsCollected += CoinsAdded;
        PlayerPrefs.SetInt("CoinsCollected", CoinsCollected);
    }
    public void GameCompletedFunc()
    {
        GameCompleted = true;
    }
    public void PotionNewCreated()
    {
        NewPotionsCreatedCount++;
        PlayerPrefs.SetInt("NewPotionsCreatedCount", NewPotionsCreatedCount);
    }
    public void PotionSoldFunc()
    {
        PotionsSold++;
        PlayerPrefs.SetInt("PotionsSold", PotionsSold);
    }
    private void SaveData()
    {
        AnalyticsPotioner myAnalyticsPotioner = new AnalyticsPotioner()
        {
            LastUsedSkin = LastUsedSkin,
            OpenedTutorialCount = OpenedTutorialCount,
            ForestVisitCount = ForestVisitCount,
            GameEntered = GameEntered,
            MenuTGComment = MenuTGComment,
            MenuOtherGame = MenuOtherGame,
            MenuTGChannel= MenuTGChannel,
            EndTGChannel = EndTGChannel,
            CoinsCollected= CoinsCollected,
            GameCompleted= GameCompleted,
            NewPotionsCreatedCount= NewPotionsCreatedCount,
            PotionsSold= PotionsSold,
        };
        Inst.RecordEvent(myAnalyticsPotioner);
       
    }
    private void OnDestroy()
    {
        SaveData();
        //AnalyticsService.Instance.Flush();
        //Inst.Flush();
    }
}

public class AnalyticsPotioner : Unity.Services.Analytics.Event
{
    public AnalyticsPotioner() : base("myAnalyticsPotioner")
    {
    }

    public string LastUsedSkin { set { SetParameter("LastUsedSkin", value); } }
    public int OpenedTutorialCount { set { SetParameter("OpenedTutorialCount", value); } }
    public int ForestVisitCount { set { SetParameter("ForestVisitCount", value); } }
    public int GameEntered { set { SetParameter("GameEntered", value); } }
    public int MenuTGComment { set { SetParameter("MenuTGComment", value); } }
    public int MenuOtherGame { set { SetParameter("MenuOtherGame", value); } }
    public int MenuTGChannel { set { SetParameter("MenuTGChannel", value); } }
    public int EndTGChannel { set { SetParameter("EndTGChannel", value); } }
    public int CoinsCollected { set { SetParameter("CoinsCollected", value); } }
    public int NewPotionsCreatedCount { set { SetParameter("NewPotionsCreatedCount", value); } }
    public int PotionsSold { set { SetParameter("PotionsSold", value); } }
    public bool GameCompleted { set { SetParameter("GameCompleted", value); } }
}