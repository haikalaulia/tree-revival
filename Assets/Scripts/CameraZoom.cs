using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private Camera cam;
    
    [Header("Pengaturan Zoom")]
    public float zoomSpeed = 5f;      // Kecepatan respon scroll
    public float minZoom = 2f;       // Batas paling dekat
    public float maxZoom = 12f;      // Batas paling jauh
    public float smoothness = 10f;   // Kehalusan gerakan zoom

    private float targetZoom;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        
        // Ambil nilai awal kamera sebagai target awal
        targetZoom = cam.orthographicSize;
    }

    void Update()
    {
        // 1. Ambil input dari roda mouse (Scroll Wheel)
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            // 2. Hitung target zoom baru
            targetZoom -= scrollInput * zoomSpeed;
            
            // 3. Batasi agar tidak terlalu dekat atau terlalu jauh
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        // 4. Terapkan zoom secara halus (Lerp)
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * smoothness);
    }
}