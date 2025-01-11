using UnityEngine;
using UnityEngine.UI; // Необходимо для использования Image

public class ScrollingTextureUI : MonoBehaviour
{
    public float scrollSpeedX = 0.5f;
    public float scrollSpeedY = 0.5f;
    private Image image;
    private Material mat;

    void Start()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("Image component not found on this GameObject!");
            enabled = false;
            return;
        }

        mat = image.material;
        if (mat == null)
        {
            Debug.LogError("Material not found on this Image!");
            enabled = false;
            return;
        }

        // Создаем копию материала
        mat = image.material = new Material(mat);
    }

    void Update()
    {
        float offsetX = Time.time * scrollSpeedX;
        float offsetY = Time.time * scrollSpeedY;

        mat.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}