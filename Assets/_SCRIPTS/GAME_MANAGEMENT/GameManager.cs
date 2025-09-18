using UnityEngine;
using TMPro; // Bu satýr zaten vardý, emin olalým.
using UnityEngine.SceneManagement; // Bu satýrý ekliyoruz!

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int score = 0;
    public TextMeshProUGUI scoreText;

    // YENÝ: Inspector'dan atayacaðýmýz UI referanslarý
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    private void Awake()
    {
        // YENÝ: Oyun baþladýðýnda zamanýn normal aktýðýndan emin olalým.
        Time.timeScale = 1f;

        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (scoreText != null) { scoreText.text = "Skor: " + score; }
    }

    // YENÝ FONKSÝYON: Oyun bittiðinde çaðrýlacak.
    public void GameOver()
    {
        // Oyunu dondur.
        Time.timeScale = 0f;
        // Oyun Bitti panelini aktif et.
        gameOverPanel.SetActive(true);
        // Final skoru ekrana yazdýr.
        finalScoreText.text = "Skor: " + score;
    }

    // YENÝ FONKSÝYON: Buton tarafýndan çaðrýlacak.
    public void RestartGame()
    {
        // Oyunu yeniden baþlatmadan önce zamaný normale döndürmek çok önemli!
        Time.timeScale = 1f;
        // Mevcut sahneyi yeniden yükle.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}