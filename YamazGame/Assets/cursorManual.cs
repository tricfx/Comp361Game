using UnityEngine;

public class cursorManual : MonoBehaviour
{

    void Update()
    {
     if (Input.GetKeyDown(KeyCode.V))
        {
              if (CursorManager.Instance != null)
                CursorManager.Instance.ShowCursor();
        }
    }
}
