using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

// Question class'ý ayný, ona dokunmuyoruz.
[System.Serializable]
public class Question
{
    public string questionText;
    public string[] answers = new string[3];
    public int correctAnswerIndex;
}

public class QuizManager : MonoBehaviour
{
    public static QuizManager instance;

    [Header("Quiz Data")]
    public List<Question> questions;

    [Header("UI References")]
    public GameObject quizPanel;
    public TextMeshProUGUI questionTextUI;
    public TextMeshProUGUI feedbackTextUI;
    public TextMeshProUGUI[] answerButtonTexts;

    private Question currentQuestion;
    private GameObject currentGate;

    // YENÝ: Oyuncunun hareket script'ine eriþmek için bir referans.
    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }

    public void StartQuiz(GameObject gate)
    {
        Debug.Log("Sýnav baþlýyor, PlayerMovement script'i aranýyor...");

        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        // YENÝ ÝÞARET FÝÞEÐÝ 2:
        if (playerMovement == null)
        {
            Debug.LogError("HATA: PlayerMovement script'i BULUNAMADI! Player objesinin Tag'i 'Player' olarak ayarlý mý?");
        }
        else
        {
            Debug.Log("PlayerMovement script'i baþarýyla bulundu!");
        }


        currentGate = gate;

        // YENÝ: Sýnav baþlarken, sahnedeki "Player" etiketli objeyi bulup onun script'ini hafýzaya alýyoruz.
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
       
        // DEÐÝÞÝKLÝK: Artýk Time.timeScale'i durdurmuyoruz, çünkü animasyonlarýn (Idle) oynamasý lazým.
        // Oyuncuyu zaten PlayerMovement script'i kendi içinde durdurdu.

        currentQuestion = questions[Random.Range(0, questions.Count)];
        questionTextUI.text = currentQuestion.questionText;

        for (int i = 0; i < answerButtonTexts.Length; i++)
        {
            answerButtonTexts[i].text = currentQuestion.answers[i];
        }

        feedbackTextUI.text = "";
        quizPanel.SetActive(true);
    }

    public void AnswerButtonPressed(int buttonIndex)
    {
        if (buttonIndex == currentQuestion.correctAnswerIndex)
        {
            feedbackTextUI.text = "Doðru!";
            feedbackTextUI.color = Color.green;

            if (currentGate != null)
            {
                currentGate.GetComponent<Animator>().SetTrigger("Open");
            }

            StartCoroutine(EndQuizCoroutine());
        }
        else
        {
            feedbackTextUI.text = "Tekrar Dene!";
            feedbackTextUI.color = Color.red;
        }
    }

    // Bu Coroutine de güncellendi.
    System.Collections.IEnumerator EndQuizCoroutine()
    {
        yield return new WaitForSeconds(2f);
        quizPanel.SetActive(false);

        // YENÝ ÝÞARET FÝÞEÐÝ 3:
        Debug.Log("Coroutine bitti, ResumeRunning çaðrýlmaya çalýþýlýyor...");

        if (playerMovement != null)
        {
            playerMovement.ResumeRunning();
        }
        else
        {
            Debug.LogError("HATA: PlayerMovement referansý BOÞ olduðu için ResumeRunning çaðrýlamadý!");
        }
    }
}