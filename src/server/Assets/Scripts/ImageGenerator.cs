using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageGenerator : MonoBehaviour
{
    [SerializeField] public int resolution = 10;
    [SerializeField] public int leftMargin = 10;
    [SerializeField] public int rightMargin = 10;
    [SerializeField] public int topMargin = 10;
    [SerializeField] public int bottomMargin = 10;

    private Bounds bounds;

    void Start() {
        bounds = new Bounds(Vector3.zero, Vector3.zero);
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");
        foreach (GameObject obstacle in obstacles) {
            Bounds obstacleBounds = obstacle.GetComponent<Collider>().bounds;
            bounds.Encapsulate(obstacleBounds);
        }
        int imageWidth = Mathf.CeilToInt((bounds.size.x + 1 + leftMargin + rightMargin) * resolution); // + 1 because SetPixel(imageWidth, y) paints the leftmost pixel, instead of the rightmost.
        int imageHeight = Mathf.CeilToInt((bounds.size.z + 1 + bottomMargin + topMargin) * resolution);
        Texture2D texture2D = new Texture2D(imageWidth, imageHeight);
        Color[] pixels = new Color[imageWidth * imageHeight];
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = Color.white;
        }
        texture2D.SetPixels(pixels);

        // Draw obstacle black shapes
        foreach (GameObject obstacle in obstacles) {
            Bounds obstacleBounds = obstacle.GetComponent<Collider>().bounds;
            Vector3 minDisplaced = obstacleBounds.min + new Vector3(leftMargin, 0, bottomMargin) - bounds.min;
            Vector3 maxDisplaced = minDisplaced + obstacleBounds.size;
            Vector3Int min = new Vector3Int(
                Mathf.CeilToInt(minDisplaced.x * resolution),
                Mathf.CeilToInt(minDisplaced.y * resolution),
                Mathf.CeilToInt(minDisplaced.z * resolution)
                );
            Vector3Int max = new Vector3Int(
                Mathf.CeilToInt(maxDisplaced.x * resolution),
                Mathf.CeilToInt(maxDisplaced.y * resolution),
                Mathf.CeilToInt(maxDisplaced.z * resolution)
                );
            for (int x = min.x; x <= max.x; x++) {
                for (int y = min.z; y <= max.z; y++) {
                    if (x < 0 || x >= imageWidth || y < 0 || y >= imageHeight) {
                        Debug.Log($"Obstacle at {obstacleBounds.center} is overflowing the PNG image.");
                    }
                    texture2D.SetPixel(x, y, Color.black);
                }
            }
        }

        // Export the PNG image
        texture2D.Apply();
        byte[] imageBytes = texture2D.EncodeToPNG();
        System.IO.File.WriteAllBytes("GeneratedImage.png", imageBytes);
    }

    void Update() {
        DrawObstacleDebugBoundingBox();
    }

    private void DrawObstacleDebugBoundingBox() {
        DrawRectangle(bounds.min, bounds.max, y: 2, Color.red);
        Debug.DrawLine(new Vector3(bounds.center.x, 2, bounds.center.z), new Vector3(bounds.center.x, 10, bounds.center.z), Color.red);
    }

    private void DrawRectangle(Vector3 leftBotPoint, Vector3 rightTopPoint, float y, Color color) {
        Debug.DrawLine(new Vector3(leftBotPoint.x, y, leftBotPoint.z), new Vector3(leftBotPoint.x, y, rightTopPoint.z), color); // left
        Debug.DrawLine(new Vector3(leftBotPoint.x, y, rightTopPoint.z), new Vector3(rightTopPoint.x, y, rightTopPoint.z), color); // top
        Debug.DrawLine(new Vector3(leftBotPoint.x, y, leftBotPoint.z), new Vector3(rightTopPoint.x, y, leftBotPoint.z), color); // bot
        Debug.DrawLine(new Vector3(rightTopPoint.x, y, rightTopPoint.z), new Vector3(rightTopPoint.x, y, leftBotPoint.z), color); // right
    }

    /*
    private void GoodApproachV2() {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");
        GameObject[] edges = GetExtremePositions(obstacles);
        GameObject leftmost = edges[0];
        GameObject bottommost = edges[1];
        GameObject rightmost = edges[2];
        GameObject topmost = edges[3];
        float distanceToOriginX = leftmost.GetComponent<MeshCollider>().bounds.min.x;
        float distanceToOriginZ = bottommost.GetComponent<MeshCollider>().bounds.min.z;
        int imageWidth = (GetImageWidth(leftmost, rightmost) + leftMargin + rightMargin) * resolution;
        int imageHeight = (GetImageHeight(bottommost, topmost) + topMargin + bottomMargin) * resolution;
        Texture2D generatedImage = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
        Color[] pixels = new Color[imageWidth * imageHeight];
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = Color.white;
        }
        generatedImage.SetPixels(pixels);

        // Draw black shapes on the image based on obstacle mesh colliders
        foreach (GameObject obstacle in obstacles) {
            MeshCollider obstacleMeshCollider = obstacle.GetComponent<MeshCollider>();
            if (obstacleMeshCollider != null) {
                // Convert world space coordinates to image space
                Bounds bounds = obstacleMeshCollider.bounds;
                Vector3 boundsMin = bounds.min;
                Vector3 boundsMax = bounds.max;

                // Draw a black rectangle on the image
                int xMin = (Mathf.RoundToInt(boundsMin.x - distanceToOriginX) + leftMargin) * resolution;
                int xMax = (Mathf.RoundToInt(boundsMax.x - distanceToOriginX) + leftMargin) * resolution;
                int yMin = (Mathf.RoundToInt(boundsMin.z - distanceToOriginZ) + bottomMargin) * resolution;
                int yMax = (Mathf.RoundToInt(boundsMax.z - distanceToOriginZ) + bottomMargin) * resolution;

                for (int x = xMin; x <= xMax; x++) {
                    for (int y = yMin; y <= yMax; y++) {
                        generatedImage.SetPixel(x, y, Color.black);
                    }
                }
            } else {
                Debug.LogWarning($"Obstacle {obstacle.name} does not have a MeshCollider.");
            }
        }
       
        byte[] imageBytes = generatedImage.EncodeToPNG();
        System.IO.File.WriteAllBytes("GeneratedImage.png", imageBytes);
    }

    private void GoodApproach() {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");
        var edges = GetExtremePositions(obstacles);
        var leftmost = edges[0];
        var bottommost = edges[1];
        var rightmost = edges[2];
        var topmost = edges[3];
        int imageWidth = (GetImageWidth(leftmost, rightmost) + leftMargin + rightMargin) * resolution;
        int imageHeight = (GetImageHeight(bottommost, topmost) + topMargin + bottomMargin) * resolution;
        Texture2D generatedImage = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
        Color[] pixels = new Color[imageWidth * imageHeight];
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = Color.white;
        }
        generatedImage.SetPixels(pixels);
        // Draw black shapes on the image based on obstacle mesh colliders
        foreach (GameObject obstacle in obstacles) {
            MeshCollider obstacleMeshCollider = obstacle.GetComponent<MeshCollider>();
            if (obstacleMeshCollider != null) {
                // Convert world space coordinates to image space
                Bounds bounds = obstacleMeshCollider.bounds;
                Vector3 boundsMin = bounds.min * resolution;
                Vector3 boundsMax = bounds.max * resolution;

                // Draw a black rectangle on the image
                int xMin = Mathf.RoundToInt(boundsMin.x) + leftMargin * resolution;
                int xMax = Mathf.RoundToInt(boundsMax.x) + leftMargin * resolution;
                int yMin = Mathf.RoundToInt(boundsMin.z) + topMargin * resolution;
                int yMax = Mathf.RoundToInt(boundsMax.z) + topMargin * resolution;

                for (int x = xMin; x <= xMax; x++) {
                    for (int y = yMin; y <= yMax; y++) {
                        generatedImage.SetPixel(x, y, Color.black);
                    }
                }
            } else {
                Debug.LogWarning($"Obstacle {obstacle.name} does not have a MeshCollider.");
            }
        }
       
        byte[] imageBytes = generatedImage.EncodeToPNG();
        System.IO.File.WriteAllBytes("GeneratedImage.png", imageBytes);
    }

    private int GetImageWidth(GameObject leftmostObject, GameObject rightmostObject) {
        MeshCollider leftMeshCollider = leftmostObject.GetComponent<MeshCollider>();
        MeshCollider rightMeshCollider = rightmostObject.GetComponent<MeshCollider>();
        return Mathf.CeilToInt(Mathf.Abs(leftMeshCollider.bounds.max.x - rightMeshCollider.bounds.min.x));
    }

    private int GetImageHeight(GameObject topmostObject, GameObject bottommostObject) {
        MeshCollider topMeshCollider = topmostObject.GetComponent<MeshCollider>();
        MeshCollider bottomMeshCollider = bottommostObject.GetComponent<MeshCollider>();
        return Mathf.CeilToInt(Mathf.Abs(topMeshCollider.bounds.max.z - bottomMeshCollider.bounds.min.z));
    }

    public GameObject[] GetExtremePositions(GameObject[] gameObjects) {
        leftmostObject = null;
        rightmostObject = null;
        topmostObject = null;
        bottommostObject = null;

        leftmostPosition = float.MaxValue;
        rightmostPosition = float.MinValue;
        topmostPosition = float.MinValue;
        bottommostPosition = float.MaxValue;

        foreach (GameObject go in gameObjects) {
            MeshCollider meshCollider = go.GetComponent<MeshCollider>();
            if (meshCollider != null) {
                Bounds bounds = meshCollider.bounds;
                if (bounds.min.x < leftmostPosition) {
                    leftmostPosition = bounds.min.x;
                    leftmostObject = go;
                }
                if (bounds.max.x > rightmostPosition) {
                    rightmostPosition = bounds.max.x;
                    rightmostObject = go;
                }
                if (bounds.min.z < bottommostPosition) {
                    bottommostPosition = bounds.min.z;
                    bottommostObject = go;
                }
                if (bounds.max.z > topmostPosition) {
                    topmostPosition = bounds.max.z;
                    topmostObject = go;
                }
            }
        }

        leftmostObject.name += " left";
        bottommostObject.name += " bottom";
        rightmostObject.name += " right";
        topmostObject.name += " top";

        return new GameObject[4] { leftmostObject, bottommostObject, rightmostObject, topmostObject };
    }

    private void Approach2() {
        int imageWidth = 1080;
        int imageHeight = 1080;

        // Renderiza la escena desde la perspectiva cenital
        RenderTexture renderTexture = new RenderTexture(imageWidth, imageHeight, 24);
        topDownCamera.targetTexture = renderTexture;
        topDownCamera.Render();

        // Crea una textura para almacenar la imagen
        Texture2D texture = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        texture.Apply();

        // Encuentra todos los objetos con la etiqueta "obstacle"
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");

        // Crea una imagen en blanco con fondo blanco
        Color[] pixels = new Color[imageWidth * imageHeight];
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = Color.white;
        }

        // Marca las formas negras en la imagen
        foreach (GameObject obstacle in obstacles) {
            MeshCollider meshCollider = obstacle.GetComponent<MeshCollider>();
            if (meshCollider != null) {
                Vector3[] vertices = meshCollider.sharedMesh.vertices;
                foreach (Vector3 vertex in vertices) {
                    Vector3 screenPos = topDownCamera.WorldToScreenPoint(obstacle.transform.TransformPoint(vertex));
                    int x = Mathf.RoundToInt(screenPos.x);
                    int y = Mathf.RoundToInt(screenPos.y);
                    if (x >= 0 && x < imageWidth && y >= 0 && y < imageHeight) {
                        pixels[y * imageWidth + x] = Color.black;
                    }
                }
            }
        }

        // Guarda la imagen como archivo JPEG
        byte[] imageBytes = texture.EncodeToJPG();
        System.IO.File.WriteAllBytes("GeneratedImage.jpg", imageBytes);

        // Limpia los recursos
        RenderTexture.active = null;
        topDownCamera.targetTexture = null;
        Destroy(renderTexture);
    }

    private void Approach1()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");
        Bounds bounds = new Bounds(obstacles[0].transform.position, Vector3.zero);
        foreach (GameObject obstacle in obstacles) {
            bounds.Encapsulate(obstacle.GetComponent<Renderer>().bounds);
        }
        Vector3 center = bounds.center;
        Debug.Log(center);


        // Get the dimensions of the terrain
        Vector3 terrainSize = terrain.GetComponent<Renderer>().bounds.size;
        int imageWidth = Mathf.RoundToInt(terrainSize.x);
        int imageHeight = Mathf.RoundToInt(terrainSize.z);

        // Create a white background image
        Texture2D generatedImage = new Texture2D(imageWidth, imageHeight);
        Color[] pixels = new Color[imageWidth * imageHeight];
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = Color.white;
        }
        generatedImage.SetPixels(pixels);
        generatedImage.Apply();

        // Find all game objects with the "obstacle" tag
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");

        // Draw black shapes on the image based on obstacle colliders
        foreach (GameObject obstacle in obstacles) {
            Collider obstacleCollider = obstacle.GetComponent<Collider>();
            if (obstacleCollider != null) {
                // Convert world space coordinates to image space
                Bounds bounds = obstacleCollider.bounds;
                Vector3 min = terrain.transform.InverseTransformPoint(bounds.min);
                Vector3 max = terrain.transform.InverseTransformPoint(bounds.max);

                // Draw a black rectangle on the image
                int xMin = Mathf.RoundToInt(min.x);
                int xMax = Mathf.RoundToInt(max.x);
                int yMin = Mathf.RoundToInt(min.z);
                int yMax = Mathf.RoundToInt(max.z);

                for (int x = xMin; x <= xMax; x++) {
                    for (int y = yMin; y <= yMax; y++) {
                        generatedImage.SetPixel(x, y, Color.black);
                    }
                }
            }
        }

        // Save the image as a JPEG file
        byte[] imageBytes = generatedImage.EncodeToJPG();
        System.IO.File.WriteAllBytes("GeneratedImage.jpg", imageBytes);


        // Get the dimensions of the terrain
        Vector3 terrainSize = terrain.GetComponent<Renderer>().bounds.size;
        int imageWidth = Mathf.RoundToInt(terrainSize.x);
        int imageHeight = Mathf.RoundToInt(terrainSize.z);

        // Create a white background image
        Texture2D generatedImage = new Texture2D(imageWidth, imageHeight);
        Color[] pixels = new Color[imageWidth * imageHeight];
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = Color.white;
        }
        generatedImage.SetPixels(pixels);
        generatedImage.Apply();

        // Find all game objects with the "obstacle" tag
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");

        // Draw black shapes on the image based on obstacle colliders
        foreach (GameObject obstacle in obstacles) {
            Collider obstacleCollider = obstacle.GetComponent<Collider>();
            if (obstacleCollider != null) {
                // Convert world space coordinates to image space
                Bounds bounds = obstacleCollider.bounds;
                Vector3 min = terrain.transform.InverseTransformPoint(bounds.min);
                Vector3 max = terrain.transform.InverseTransformPoint(bounds.max);

                // Draw a black rectangle on the image
                int xMin = Mathf.RoundToInt(min.x);
                int xMax = Mathf.RoundToInt(max.x);
                int yMin = Mathf.RoundToInt(min.z);
                int yMax = Mathf.RoundToInt(max.z);

                for (int x = xMin; x <= xMax; x++) {
                    for (int y = yMin; y <= yMax; y++) {
                        generatedImage.SetPixel(x, y, Color.black);
                    }
                }
            }
        }

        // Save the image as a JPEG file
        byte[] imageBytes = generatedImage.EncodeToJPG();
        System.IO.File.WriteAllBytes("GeneratedImage.jpg", imageBytes);


        // Get the dimensions of the terrain
        Vector3 terrainSize = terrain.GetComponent<Renderer>().bounds.size;
        int imageWidth = Mathf.RoundToInt(terrainSize.x);
        int imageHeight = Mathf.RoundToInt(terrainSize.z);

        // Create a white background image
        Texture2D generatedImage = new Texture2D(imageWidth, imageHeight);
        Color[] pixels = new Color[imageWidth * imageHeight];
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = Color.white;
        }
        generatedImage.SetPixels(pixels);

        // Find all game objects with the "obstacle" tag
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");

        // Draw black shapes on the image based on obstacle mesh colliders
        foreach (GameObject obstacle in obstacles) {
            MeshCollider obstacleMeshCollider = obstacle.GetComponent<MeshCollider>();
            if (obstacleMeshCollider != null) {
                // Convert world space coordinates to image space
                Bounds bounds = obstacleMeshCollider.bounds;
                Vector3 boundsMin = bounds.min * resolution;
                Vector3 boundsMax = bounds.max * resolution;
                Vector3 min = terrain.transform.InverseTransformPoint(boundsMin);
                Vector3 max = terrain.transform.InverseTransformPoint(boundsMax);

                // Draw a black rectangle on the image
                int xMin = Mathf.RoundToInt(min.x);
                int xMax = Mathf.RoundToInt(max.x);
                int yMin = Mathf.RoundToInt(min.z);
                int yMax = Mathf.RoundToInt(max.z);

                for (int x = xMin; x <= xMax; x++) {
                    for (int y = yMin; y <= yMax; y++) {
                        generatedImage.SetPixel(x, y, Color.black);
                    }
                }
            } else {
                Debug.LogWarning($"Obstacle {obstacle.name} does not have a MeshCollider.");
            }
        }
        generatedImage.Apply();

        // Save the image as a JPEG file
        byte[] imageBytes = generatedImage.EncodeToPNG();
        System.IO.File.WriteAllBytes("GeneratedImage.png", imageBytes);
    }
    */

}
