using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    [SerializeField] private GameObject FadeIn;
    [SerializeField] private GameObject FadeOut;
    [SerializeField] private GameObject BGMusic;
    [SerializeField] private int IndexNextMap=0;
    private void Start()
    {
        if (FadeOut != null)
            FadeOut.SetActive(true);
        if(FadeIn!=null)
            FadeIn.SetActive(false);
        Player.FreezeMove = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Enter(IndexNextMap);
        }
    }

    public void Enter(int i)
    {
        FadeIn.SetActive(true);
        FadeIn.GetComponent<Teleport>().TeleportTo = i;
        BGMusic.GetComponent<Animator>().SetTrigger("BGMusic_In");
        Player.FreezeMove = true;
    }
}
