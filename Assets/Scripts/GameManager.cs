using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager: MonoBehaviour {
  public static AudioClip throwFruitSound, throwBombSound, bombExplodeSound, bombFuseSound, fruitSliceSound, fruitSliceWaterSound, fruitSliceFireSound, backgroundSound, startSound, bombSliceSound;

  public AudioSource audioSrc;
  public TMP_Text scoreText;
  public TMP_Text comboText;
  public Image fadeImage;
  public TMP_Text countdownText;
  public TMP_Text gameOverText;
  public TMP_Text summaryText;
  public GameObject playAgainButton;
  public GameObject exitButton;
  public int currentCombo = 0;
  public int maxCombo = 0;

  private Blade blade;
  private Spawner spawner;
  private int score;
  private float currentTime = 0f;
  private float startingTime = 2f;
  public bool isFireMode = false;
  public bool isWaterMode = false;
  public bool isSlowmotion = false;

  private void Awake() {
    currentTime = startingTime;
    blade = FindObjectOfType<Blade>();
    spawner = FindObjectOfType<Spawner>();
  }

  private void Start() {
    throwFruitSound = Resources.Load<AudioClip>("Throw_Fruit");
    throwBombSound = Resources.Load<AudioClip>("Throw_Bomb");
    fruitSliceSound = Resources.Load<AudioClip>("Fruit_Slice");
    fruitSliceWaterSound = Resources.Load<AudioClip>("Fruit_Slice_Water");
    fruitSliceFireSound = Resources.Load<AudioClip>("Fruit_Slice_Fire");
    backgroundSound = Resources.Load<AudioClip>("Background");
    startSound = Resources.Load<AudioClip>("Game_Start");
    bombExplodeSound = Resources.Load<AudioClip>("Bomb_Explode");
    bombFuseSound = Resources.Load<AudioClip>("Bomb_Fuse");
    bombSliceSound = Resources.Load<AudioClip>("Bomb_Slice");
    audioSrc = GetComponent<AudioSource>();

    audioSrc.PlayOneShot(startSound, 0.8f);
    audioSrc.PlayDelayed(2);
    audioSrc.PlayOneShot(backgroundSound, 0.3f);
    audioSrc.PlayDelayed(0);

    NewGame();
  }

  private void Update() {
    switch (currentTime.ToString("0")) {
    case "2":
      countdownText.text = "READY??";
      break;
    case "1":
      countdownText.text = "GO!!";
      break;
    case "0":
      countdownText.enabled = false;
      break;
    default:
      countdownText.enabled = false;
      break;
    }

    if (currentTime >= 0) {
      currentTime -= 1 * Time.deltaTime;
    }

    comboText.text = "Combo: " + currentCombo.ToString() + "x";
  }

  public void PlaySound(string clip) {
    switch (clip) {
    case "throwFruit":
      audioSrc.PlayOneShot(throwFruitSound, 0.7f);
      break;
    case "throwBomb":
      audioSrc.PlayOneShot(throwBombSound, 1f);
      break;
    case "bombExplode":
      audioSrc.PlayOneShot(bombExplodeSound, 1f);
      break;
    case "bombFuse":
      audioSrc.PlayOneShot(bombFuseSound, 1f);
      break;
    case "fruitSlice":
      if (isFireMode) {
        audioSrc.PlayOneShot(fruitSliceFireSound, 0.8f);
      } else if (isWaterMode) {
        audioSrc.PlayOneShot(fruitSliceWaterSound, 0.8f);
      } else {
        audioSrc.PlayOneShot(fruitSliceSound, 0.8f);
      }
      break;
    case "bombSlice":
      audioSrc.PlayOneShot(bombSliceSound, 0.8f);
      break;
    }
  }

  public bool isSpecialEffect() {
    return isFireMode || isWaterMode || isSlowmotion;
  }

  public void activateFireMode() {
    isFireMode = true;
    isWaterMode = false;
  }

  public void activateWaterMode() {
    isWaterMode = true;
    isFireMode = false;
  }

  public void disableSpecialMode() {
    isWaterMode = false;
    isFireMode = false;
  }

  private void NewGame() {
    Time.timeScale = 1f;

    ClearScene();

    blade.enabled = true;
    spawner.enabled = true;
    scoreText.text = "Points:" + score.ToString();
    score = 0;
  }

  private void ClearScene() {
    Fruit[] fruits = FindObjectsOfType<Fruit>();

    foreach(Fruit fruit in fruits) {
      Destroy(fruit.gameObject);
    }

    Bomb[] bombs = FindObjectsOfType<Bomb>();

    foreach(Bomb bomb in bombs) {
      Destroy(bomb.gameObject);
    }
  }

  public void IncreaseScore(int points) {
    score += points;
    scoreText.text = "Points: " + score.ToString();
  }

  public void Explode() {
    blade.enabled = false;
    spawner.enabled = false;
    PlaySound("bombExplode");
    StartCoroutine(ExplodeSequence());
  }

  private IEnumerator ExplodeSequence() {
    float elapsed = 0f;
    float duration = 0.5f;

    while (elapsed < duration) {
      float t = Mathf.Clamp01(elapsed / duration);
      fadeImage.color = Color.Lerp(Color.clear, Color.white, t);

      Time.timeScale = 1f - t;
      elapsed += Time.unscaledDeltaTime;

      yield
      return null;
    }

    yield
    return new WaitForSecondsRealtime(2f);

    audioSrc.Pause();

    summaryText.text = "Points: " + score + ", MaxCombo: " + maxCombo;
    summaryText.enabled = true;
    gameOverText.enabled = true;
    playAgainButton.SetActive(true);
    exitButton.SetActive(true);
  }

  public void PlayAgain() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  public void Exit() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
  }

}