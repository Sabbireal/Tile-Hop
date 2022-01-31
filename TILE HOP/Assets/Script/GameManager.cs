using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static bool isBallRunning = false;
    public static bool isGameOver = false;

    [SerializeField]
    private GameObject EntryTiles;

    [SerializeField]
    private GameObject[] tiles;
    int targetTileNumber = 0;
    int totalTile = 0;

    [Header("Materials")]
    [SerializeField]
    private Material activeTileColor_face;
    [SerializeField]
    private Material deactiveTileColor_face;

    [Space(10)]
    [SerializeField]
    private Transform resetPoint;

    [SerializeField]
    private Color[] colors;
    int nextColor = 1;
    bool changeSettingOnce = false;

    [Space(10)]
    [SerializeField]
    private GameObject blink;

    //events
    public event Action<Transform, float> UpdateTarget;
    public event Action<float> sentScore;
    public event Action<int> sentDiamond;

    //for tiles randomization
    bool tileMoveIgnored = false;
    int rand;
    Vector3 nextTilePos;
    float[] distances = { 0.85f, 1.25f, 2f, 3f, 4f };
    float[] angles = { 45f };

    //Gameplay Scores;
    private float score = 0;
    private int diamond = 0;
    private int scoreMultipliyer = 2;

    // Start is called before the first frame update
    void Start()
    {
        isBallRunning = false;
        isGameOver = false;

        nextTilePos = EntryTiles.transform.position;
        FindObjectOfType<BallManager>().callForNextTarget += calledForNextTarget;

        initiate();
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0) && !isBallRunning && !isGameOver) {
            startRun();
        }

        if (changeSettingOnce) {
            changeTheme();
            increseSpeed();
            changeSettingOnce = false;
        }

        if (isBallRunning) {
            score += Time.deltaTime * scoreMultipliyer;
            sentScore?.Invoke(score);
        }
    }

    void startRun() {
        isBallRunning = true;
        EntryTiles.transform.GetChild(2).GetComponent<MeshRenderer>().material = deactiveTileColor_face;
        EntryTiles.transform.GetChild(3).GetChild(0).GetComponent<ParticleSystem>().Play();
        EntryTiles.GetComponent<Animator>().enabled = true;
        UpdateTarget?.Invoke(tiles[targetTileNumber].transform.GetChild(0), tiles[targetTileNumber].GetComponent<tileProperties>().angle);
        targetTileNumber++;
    }

    void calledForNextTarget() {
        if (tiles.Length == targetTileNumber) targetTileNumber = 0;

        UpdateTarget?.Invoke(tiles[targetTileNumber].transform.GetChild(0), tiles[targetTileNumber].GetComponent<tileProperties>().angle);

        if (targetTileNumber - 2 >= 0) tileMoveIgnored = true;

        if (tileMoveIgnored) {
            if (targetTileNumber - 2 >= 0)
                repositionAndMoveLaterOn(tiles[targetTileNumber - 2]);
            else if (targetTileNumber == 1)
                repositionAndMoveLaterOn(tiles[tiles.Length - 1]);
            else if (targetTileNumber == 0)
                repositionAndMoveLaterOn(tiles[tiles.Length - 2]);
        }

        totalTile++;
        if (totalTile % 20 == 0)
        {
            changeSettingOnce = true;

        }

        targetTileNumber++;

    }

    void initiate() {
        for (int i = 0; i < tiles.Length;) {
            repositionAndMoveWithoutAnim(tiles[i]);
            i++;
        }
    }

    void repositionAndMoveLaterOn(GameObject gameObject) {
        if (totalTile % 20 == 0)
            gameObject.GetComponent<tileProperties>().showOrHideDiamonds(true);

        findRandandReset(gameObject, true, false);
        Vector3 tempNewpos = new Vector3(gameObject.transform.position.x, nextTilePos.y, nextTilePos.z);
        gameObject.transform.position = tempNewpos;
    }

    void repositionAndMoveWithoutAnim(GameObject gameObject)
    {
        findRandandReset(gameObject, false, true);
        gameObject.transform.position = nextTilePos;
    }
    void findRandandReset(GameObject gameObject, bool doVisual = false, bool isRandzero = false)
    {
        if (isRandzero)
            rand = 0;
        else
            rand = UnityEngine.Random.Range(0, distances.Length);

        nextTilePos += new Vector3(0, 0, distances[rand]);
        gameObject.GetComponent<tileProperties>().angle = angles[0];

        float tempX = UnityEngine.Random.Range(-0.8f, 0.8f);
        Vector3 newResetPosition = new Vector3(tempX, resetPoint.transform.position.y, resetPoint.transform.position.z);

        gameObject.transform.position = newResetPosition;

        if (doVisual)
        {
            gameObject.GetComponent<Animator>().enabled = false;

            Transform transform = gameObject.transform;
            transform.GetChild(3).GetChild(0).GetComponent<ParticleSystem>().Stop();
            transform.GetChild(2).GetComponent<MeshRenderer>().material = activeTileColor_face;
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            transform.GetChild(0).localPosition = Vector3.zero;
            transform.GetChild(1).localPosition = Vector3.zero;
            transform.GetChild(2).localPosition = Vector3.zero;
        }
    }

    void changeTheme() {
        if (nextColor == colors.Length) nextColor = 0;

        if (RenderSettings.skybox.HasProperty("_Tint"))
            RenderSettings.skybox.SetColor("_Tint", colors[nextColor]);

        nextColor++;
        StartCoroutine(blinkForSomeSec());
    }
    IEnumerator blinkForSomeSec() {
        blink.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        blink.SetActive(false);
    }
    void increseSpeed() {
        Physics.gravity = new Vector3(0, Physics.gravity.y - 1f, 0);
    }
    private void OnDestroy()
    {
        if (RenderSettings.skybox.HasProperty("_Tint"))
            RenderSettings.skybox.SetColor("_Tint", colors[0]);
    }
    
    //Getter Setter
    public void setDiamond()
    {
        this.diamond = diamond + 1;
        sentDiamond?.Invoke(diamond);
    }

    public Material getDeactiveTilesColour() {
        return deactiveTileColor_face;
    }
}
