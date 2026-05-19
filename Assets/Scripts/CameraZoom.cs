using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CameraZoom : MonoBehaviour
{
    private Camera cam;
    
    [Header("Pengaturan Zoom")]
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 15f;
    public float smoothness = 10f;

    private float targetZoom;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        targetZoom = cam.orthographicSize;
    }

    void Update()
    {
        // CEK APAKAH MOUSE DI ATAS UI
        if (IsPointerOverUIElement())
        {
            return; // Jangan zoom kalau lagi di atas UI
        }

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            targetZoom -= scrollInput * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * smoothness);
    }

    // Fungsi deteksi UI yang lebih akurat untuk sistem baru
    private bool IsPointerOverUIElement()
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // LOGIKA BARU:
        // Jika mouse menyentuh benda apa pun yang ada di Layer "UI", maka blokir zoom.
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return true;
            }
        }

        return false;
    }
}