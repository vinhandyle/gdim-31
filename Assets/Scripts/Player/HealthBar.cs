using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the health bar UI.
/// </summary>
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform segmentContainer;
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private Sprite normalSegment;
    [SerializeField] private Sprite brokenSegment;

    private Slider healthBar;
    private RectTransform rt;
    private List<Image> segments = new List<Image>();

    private void Awake()
    {
        healthBar = GetComponent<Slider>();
        rt = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Initialize the health bar and the health segments.
    /// </summary>
    public void SetDefaults(float health, int numSegments)
    {
        healthBar.maxValue = health;
        healthBar.value = health;

        // Create health segments
        float hbLength = rt.sizeDelta.x / 2;
        float hbWidth = rt.sizeDelta.y;

        float segmentLength = hbLength * (hbWidth / numSegments);
        float segmentWidth = hbWidth * rt.localScale.y;
        float segmentBaseX = -hbLength + hbLength / numSegments;

        for (int i = 0; i < numSegments; ++i)
        {
            GameObject segment = Instantiate(segmentPrefab, segmentContainer);           
            segment.transform.localPosition = new Vector3(segmentBaseX + i * segmentLength / 10, 0);
            segment.GetComponent<RectTransform>().sizeDelta = new Vector2(segmentLength, segmentWidth);
            segments.Add(segment.GetComponent<Image>());
        }
    }

    public void SetHealth(float health)
    {
        healthBar.value = health;
    }

    /// <summary>
    /// Set the specified amount of health segments to be broken.
    /// </summary>
    public void SetDamagedHealth(int dmg)
    {
        // Damage cannot exceed # of segments
        if (dmg > segments.Count) dmg = segments.Count;

        for (int i = 0; i < dmg; ++i)
            segments[i].sprite = brokenSegment;
    }

    /// <summary>
    /// Set all health segments back to normal.
    /// </summary>
    public void ResetDamagedHealth()
    {
        foreach (Image segment in segments)
            segment.sprite = normalSegment;
    }
}
