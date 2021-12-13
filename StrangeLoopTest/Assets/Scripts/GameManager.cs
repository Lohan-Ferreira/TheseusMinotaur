using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] Minotaur minotaur;
    [SerializeField] Player player;
    [SerializeField] GameObject FinishPanel;
    void Start()
    {
        minotaur.AddGotPlayerListener(ResetLevel);
        player.AddPlayerEscapedListener(NextLevel);
        
    }


    void Update()
    {
        
    }

    //Reset level with delay
    public void ResetLevel()
    {
        StartCoroutine(WaitBeforeReset());
        
    }

    IEnumerator WaitBeforeReset()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Load next level with delay
    public void NextLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex + 1;
        if(index <= 3)
        {
            StartCoroutine(WaitBeforeNextLevel(index));

        }

        else
        {
            FinishPanel.SetActive(true);
        }
        
    }

    IEnumerator WaitBeforeNextLevel(int index)
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(index);
    }

    //Load previous level
    public void PreviousLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex - 1;
        if (index >= 0)
        {
            SceneManager.LoadScene(index);

        }
    }

    //Undo Last Action
    public void Undo()
    {
        player.Undo();
        minotaur.Undo();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

}
