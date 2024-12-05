using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class EndScript : MonoBehaviour
{
    GameObject player;
    Player ComponentPlayer;
    public GameObject playerPosition;
    public GameObject cameraPosition;
    public GameObject EndPanel;
    public GameObject Camera;
    public GameObject MainBoo;
    public GameObject DialogWindow;
    public GameObject Chest;
    public GameObject ChestAnim;
    public AudioSource AudioSourceBoo;
    public TextMeshProUGUI DialogText;
    public TeleportManager TeleportManager;
    public GameObject TelegramIcon;

    private bool DialogEntered = false;
    public List<string> DialogStrokes = new List<string>();
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ComponentPlayer = FindObjectOfType<Player>();
        EndPanel.SetActive(false);
    }
    private void OnEnable()
    {
        Player.PressEButton += PressEAction;
    }
    private void OnDisable()
    {
        Player.PressEButton -= PressEAction;
    }
    public IEnumerator StartEndCutScene()
    {
        EndPanel.SetActive(true);
        EndPanel.GetComponent<Animator>().Play("FadeAnim");

        yield return new WaitForSecondsRealtime(1f);

        Player.FreezeMove = true;

        player.transform.position = playerPosition.transform.position;
        ComponentPlayer.cam = null;
        Camera.transform.position = cameraPosition.transform.position;

        ComponentPlayer.PlayerAnimationFreeze = true;
        ComponentPlayer.playerAnimations.Play("IdleU_Skin_" + Player.SkinSelected);
        ComponentPlayer.faceR.SetActive(false);
        ComponentPlayer.faceL.SetActive(false);
        ComponentPlayer.faceD.SetActive(false);
        ComponentPlayer.faceU.SetActive(true);
        
        //yield return new WaitForSecondsRealtime(0.5f);
       
        yield return new WaitForSecondsRealtime(1f);
        Chest.transform.position = new Vector3(0, 30, 0);
        ChestAnim.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);

        MainBoo.SetActive(true);
        StartCoroutine(RepeatSound(UnityEngine.Random.Range(4, 8 + 1)));

        yield return new WaitForSecondsRealtime(0.5f);

        DialogWindow.SetActive(true);
        DialogText.text = DialogStrokes[Index];
        DialogEntered = true;
    }
    private int Index = 0;
    private void Temp()
    {
        Destroy(Chest);
        ChestAnim.SetActive(true);
    }
    private void PressEAction()
    {
        if (DialogEntered && !BlockerBool)
        {
            Index++;
            if (Index < DialogStrokes.Count)
            {
                DialogText.text = DialogStrokes[Index];
                StartCoroutine(RepeatSound(UnityEngine.Random.Range(4,8+1)));
                
            }
            else
            {
                DialogEntered = false;
                EndPanel.gameObject.SetActive(false);
                DialogWindow.gameObject.SetActive(false);
                Player.FreezeMove = false;
                ComponentPlayer.PlayerAnimationFreeze = false;
                ComponentPlayer.cam = Camera.GetComponent<Camera>();

            }
            if (Index == 6)
            {
                TelegramIcon.SetActive(true);
            }
            
            StartCoroutine(Blocker());
        }
    }
    private bool BlockerBool = false;
    private IEnumerator Blocker()
    {
        BlockerBool = true;
        yield return new WaitForSecondsRealtime(1f);
        BlockerBool = false;
    }
    private IEnumerator RepeatSound(int count)
    {
        for(int i=0;i<count;i++)
        {
            AudioSourceBoo.Play();
            yield return new WaitForSecondsRealtime(AudioSourceBoo.clip.length);
        }
    }
}
