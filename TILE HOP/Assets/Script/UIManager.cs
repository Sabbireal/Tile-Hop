using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject Restart_Btn;
    [SerializeField]
    private GameObject Quit_Btn;
    [SerializeField]
    private GameObject Score_Txt;
    [SerializeField]
    private GameObject Diamond_Txt;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<GameManager>().sentScore += setScoreValue;
        FindObjectOfType<GameManager>().sentDiamond += setDiamondValue;
        gamePlayUI();
    }

    public void restart_btn() {
        SceneManager.LoadScene(0);
    }

    public void showRestartAndQuiteButton() {
        Restart_Btn.SetActive(true);
        Quit_Btn.SetActive(true);
    }

    public void quiteButton()
    {
        Application.Quit();
    }

    void gamePlayUI() {
        showScoreTxt();
        showDiamonTxt();
    }

    public void showScoreTxt() {
        Score_Txt.SetActive(true);
    }

    public void showDiamonTxt() {
        Diamond_Txt.SetActive(true);
    }

    void setScoreValue(float score) {
        Score_Txt.GetComponent<TMP_Text>().text = "Score: " + (int)score;
    }

    void setDiamondValue(int diamond)
    {
        Diamond_Txt.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = " X " + diamond;
    }
}
