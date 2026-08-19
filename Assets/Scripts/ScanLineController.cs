using UnityEngine;

public class ScanLineController : MonoBehaviour
{
    private GameObject scanLines;
    void Start()
    {
        scanLines = transform.GetChild(0).gameObject;
        ToggleScanLines();
    }

    public void ToggleScanLines()
    {
        if (scanLines == null) return;
        if(!PlayerPrefs.HasKey("ShowScanLines"))
        {
            scanLines.SetActive(false);
            return;
        }
        scanLines.SetActive(true);
    }
}
