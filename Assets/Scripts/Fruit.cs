using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit: MonoBehaviour {
  public GameObject whole;
  public GameObject sliced;
  public int points = 1;
  public bool isSlowmotion = false;
  public bool isFireBuff = false;
  public bool isWaterBuff = false;

  private Rigidbody fruitRigidbody;
  private Collider fruitCollider;
  private ParticleSystem juiceEffect;

  private void Awake() {
    fruitRigidbody = GetComponent<Rigidbody>();
    fruitCollider = GetComponent<Collider>();
    juiceEffect = GetComponentInChildren<ParticleSystem>();
  }

  private void Slice(Vector3 direction, Vector3 position, float force, TrailRenderer[] trail) {
    FindObjectOfType<GameManager>().IncreaseScore(points);

    if (isFireBuff) {
      FindObjectOfType<GameManager>().activateFireMode();
      StartCoroutine(ChangeTrailColor(new Color(0.89f, 0.34f, 0.13f), trail[0]));
    }

    if (isWaterBuff) {
      FindObjectOfType<GameManager>().activateWaterMode();
      StartCoroutine(ChangeTrailColor(new Color(0.14f, 0.54f, 0.85f), trail[0]));
    }

    if (isSlowmotion) {
      StartCoroutine(WaitThenRestoreTime());
    }

    fruitCollider.enabled = false;
    whole.SetActive(false);

    sliced.SetActive(true);
    juiceEffect.Play();

    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);

    Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

    foreach(Rigidbody slice in slices) {
      slice.velocity = fruitRigidbody.velocity;
      slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
    }

    FindObjectOfType<GameManager>().PlaySound("fruitSlice");
    FindObjectOfType<GameManager>().currentCombo += 1;
  }

  private IEnumerator WaitThenRestoreTime() {
    FindObjectOfType<GameManager>().isSlowmotion = true;
    Time.timeScale = 0.6f;
    yield
    return new WaitForSecondsRealtime(5f);
    Time.timeScale = 1f;
    FindObjectOfType<GameManager>().isSlowmotion = false;
  }

  private IEnumerator ChangeTrailColor(Color color, TrailRenderer trail) {
    trail.startColor = color;
    trail.endColor = color;
    yield
    return new WaitForSecondsRealtime(5f);
    trail.startColor = Color.white;
    trail.endColor = Color.white;
    FindObjectOfType<GameManager>().disableSpecialMode();

  }

  private void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Player")) {
      Blade blade = other.GetComponent<Blade>();
      Slice(blade.direction, blade.transform.position, blade.sliceForce, blade.GetComponentsInChildren<TrailRenderer>());
    }
  }
}