using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelGenarator : MonoBehaviour
{
    public static LevelGenarator Instance;

    
    [SerializeField] private List<Enemy> enemyList;
    [SerializeField] private NavMeshSurface navMeshSurface;
    [Space]
    [SerializeField] private Transform lastLevelPart;
    [SerializeField] private List<Transform> levelParts;
    private List<Transform> currentLevelParts;
    private List<Transform> generatedLevelParts = new List<Transform>();

    [SerializeField] private SnapPoint nextSnapPoint;
    private SnapPoint defaultSnapPoint;

    [Space]
    [SerializeField] private float generationCooldown;
    private float cooldownTimer;
    public bool generationOver = true;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        enemyList = new List<Enemy>();
        defaultSnapPoint = nextSnapPoint;
    }
    private void Update()
    {
        if (generationOver) return;
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer < 0)
        {
            if (currentLevelParts.Count > 0)
            {
                cooldownTimer = generationCooldown;
                GenerateNextLevelPart();
            }
            else if (!generationOver)
            {
                FinishGeneration();
            }
        }
    }

    [ContextMenu("Restart Genaration")]
    public void InitGeneration()
    {
        nextSnapPoint = defaultSnapPoint;
        generationOver = false;
        // For Demo
        if (!GameManager.Instance.QuickStart)
        {
            levelParts = MissionManager.Instance.CurrentMission.MissionLevels;
        }


        currentLevelParts = new List<Transform>(levelParts);

        DestroyOldLevelPartsAndEnemies();
    }

    private void DestroyOldLevelPartsAndEnemies()
    {
        foreach (Enemy enemy in enemyList)
        {
            Destroy(enemy.gameObject);
        }

        foreach (var item in generatedLevelParts)
        {
            Destroy(item.gameObject);
        }

        generatedLevelParts = new List<Transform>();
        enemyList = new List<Enemy>();
    }

    private void FinishGeneration()
    {
        generationOver = true;
        GenerateNextLevelPart();

        navMeshSurface.BuildNavMesh();

        foreach (Enemy enemy in enemyList)
        {
            enemy.transform.parent = null;
            enemy.gameObject.SetActive(true);
        }

        MissionManager.Instance.StartMission();
    }

    [ContextMenu("Create Next Level Part")]
    private void GenerateNextLevelPart()
    {
        Transform newPart = null;
        if (generationOver)
        {
            newPart = Instantiate(lastLevelPart);
        }
        else
        {
            newPart = Instantiate(ChooseRandomPart());
        }
        generatedLevelParts.Add(newPart);

        LevelPart levelPartScript = newPart.GetComponent<LevelPart>();

        levelPartScript.SnapAndAlignPartTo(nextSnapPoint);

        if (levelPartScript.IntersectionDetected())
        {
            InitGeneration();
            return;
        }
        nextSnapPoint = levelPartScript.GetExitPoint();
        enemyList.AddRange(levelPartScript.Enemies());
    }

    private Transform ChooseRandomPart()
    {
        int randomIndex = Random.Range(0, currentLevelParts.Count);

        Transform choosenPart = currentLevelParts[randomIndex];

        currentLevelParts.RemoveAt(randomIndex);

        return choosenPart;
    }

    public Enemy GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemyList.Count);

        return enemyList[randomIndex];
    }

    public List<Enemy> GetEnemyList() => enemyList;
}
