using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu: MonoBehaviour {
  public static bool GameIsPaused = false;
  public GameObject pauseMenu;

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      if (GameIsPaused) {
        Resume();
      } else {
        Pause();
      }
    }
  }

  public void Resume() {
    FindObjectOfType <GameManager>().audioSrc.Play();
    pauseMenu.SetActive(false);
    GameIsPaused = false;
    Time.timeScale = 1f;
  }

  private void Pause() {
    FindObjectOfType <GameManager>().audioSrc.Pause();
    pauseMenu.SetActive(true);
    GameIsPaused = true;
    Time.timeScale = 0f;
  }

  public void Menu() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
  }

  public void Exit() {
    Application.Quit();
  }
}