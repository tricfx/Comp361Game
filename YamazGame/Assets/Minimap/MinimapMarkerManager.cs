using System.Collections.Generic;
using UnityEngine;

public class MinimapMarkerManager : MonoBehaviour
{
    [SerializeField] private List<string> tags;
    [SerializeField] private List<GameObject> markerPrefabs;
    [SerializeField] private float scanInterval = 0.5f;

    private float scanTimer;
    private readonly HashSet<int> markedObjects = new();

    private void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer > 0f) return;

        scanTimer = scanInterval;
        AddMarkers();
    }

    private void AddMarkers()
    {
        int count = Mathf.Min(tags.Count, markerPrefabs.Count);

        for (int i = 0; i < count; i++)
        {
            string tagName = tags[i];
            GameObject markerPrefab = markerPrefabs[i];
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tagName);

            foreach (GameObject obj in objects)
            {
                int id = obj.GetInstanceID();
                if (markedObjects.Contains(id)) continue;

                GameObject marker = Instantiate(markerPrefab, obj.transform);
                marker.transform.localPosition = Vector3.zero;

                marker.layer = LayerMask.NameToLayer("MinimapIcon");
                markedObjects.Add(id);
            }
        }
    }
}