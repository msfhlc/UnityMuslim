using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

// Question class'� ayn�, ona dokunmuyoruz.
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

    // YEN�: Oyuncunun hareket script'ine eri�mek i�in bir referans.
    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }

    public void StartQuiz(GameObject gate)
    {
        Debug.Log("S�nav ba�l�yor, PlayerMovement script'i aran�yor...");

        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        // YEN� ��ARET F��E�� 2:
        if (playerMovement == null)
        {
            Debug.LogError("HATA: PlayerMovement script'i BULUNAMADI! Player objesinin Tag'i 'Player' olarak ayarl� m�?");
        }
        else
        {
            Debug.Log("PlayerMovement script'i ba�ar�yla bulundu!");
        }


        currentGate = gate;

        // YEN�: S�nav ba�larken, sahnedeki "Player" etiketli objeyi bulup onun script'ini haf�zaya al�yoruz.
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
       
        // DE����KL�K: Art�k Time.timeScale'i durdurmuyoruz, ��nk� animasyonlar�n (Idle) oynamas� laz�m.
        // Oyuncuyu zaten PlayerMovement script'i kendi i�inde durdurdu.

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
            feedbackTextUI.text = "Do�ru!";
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

    // Bu Coroutine de g�ncellendi.
    System.Collections.IEnumerator EndQuizCoroutine()
    {
        yield return new WaitForSeconds(2f);
        quizPanel.SetActive(false);

        // YEN� ��ARET F��E�� 3:
        Debug.Log("Coroutine bitti, ResumeRunning �a�r�lmaya �al���l�yor...");

        if (playerMovement != null)
        {
            playerMovement.ResumeRunning();
        }
        else
        {
            Debug.LogError("HATA: PlayerMovement referans� BO� oldu�u i�in ResumeRunning �a�r�lamad�!");
        }
    }
}