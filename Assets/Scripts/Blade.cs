using UnityEngine;

public class Blade: MonoBehaviour {
  public Vector3 direction {
    get;
    private set;
  }

  private Camera mainCamera;
  private Collider sliceCollider;
  private TrailRenderer sliceTrail;

  public bool slicing;
  public float sliceForce = 5f;
  public float minSliceVelocity = 0.01f;

  private void Awake() {
    mainCamera = Camera.main;
    sliceCollider = GetComponent<Collider>();
    sliceTrail = GetComponentInChildren<TrailRenderer>();
  }

  private void OnEnable() {
    StopSlice();
  }

  private void OnDisable() {
    StopSlice();
  }

  private void Update() {
    if (Input.GetMouseButtonDown(0)) {
      StartSlice();
    } else if (Input.GetMouseButtonUp(0)) {
      StopSlice();
    } else if (slicing) {
      ContinueSlice();
    }
  }

  private void StartSlice() {
    FindObjectOfType<GameManager>().currentCombo = 0;
    Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    position.z = 0f;
    transform.position = position;

    slicing = true;
    sliceCollider.enabled = true;
    sliceTrail.enabled = true;
    sliceTrail.Clear();
  }

  private void StopSlice() {
    if (FindObjectOfType<GameManager>().currentCombo > FindObjectOfType<GameManager>().maxCombo) {
      FindObjectOfType<GameManager>().maxCombo = FindObjectOfType<GameManager>().currentCombo;
    }
    
    slicing = false;
    sliceCollider.enabled = false;
    sliceTrail.enabled = false;
  }

  private void ContinueSlice() {
    Vector3 newPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    newPosition.z = 0f;

    direction = newPosition - transform.position;

    float velocity = direction.magnitude / Time.deltaTime;
    sliceCollider.enabled = velocity > minSliceVelocity;

    transform.position = newPosition;
  }
}