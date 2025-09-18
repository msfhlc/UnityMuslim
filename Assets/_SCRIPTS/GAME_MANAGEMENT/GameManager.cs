using UnityEngine;
using TMPro; // Bu sat�r zaten vard�, emin olal�m.
using UnityEngine.SceneManagement; // Bu sat�r� ekliyoruz!

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int score = 0;
    public TextMeshProUGUI scoreText;

    // YEN�: Inspector'dan atayaca��m�z UI referanslar�
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    private void Awake()
    {
        // YEN�: Oyun ba�lad���nda zaman�n normal akt���ndan emin olal�m.
        Time.timeScale = 1f;

        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (scoreText != null) { scoreText.text = "Skor: " + score; }
    }

    // YEN� FONKS�YON: Oyun bitti�inde �a�r�lacak.
    public void GameOver()
    {
        // Oyunu dondur.
        Time.timeScale = 0f;
        // Oyun Bitti panelini aktif et.
        gameOverPanel.SetActive(true);
        // Final skoru ekrana yazd�r.
        finalScoreText.text = "Skor: " + score;
    }

    // YEN� FONKS�YON: Buton taraf�ndan �a�r�lacak.
    public void RestartGame()
    {
        // Oyunu yeniden ba�latmadan �nce zaman� normale d�nd�rmek �ok �nemli!
        Time.timeScale = 1f;
        // Mevcut sahneyi yeniden y�kle.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}