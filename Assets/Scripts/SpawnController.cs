using UnityEngine;
using UnityEngine.UI;

public class SpawnController : MonoBehaviour
{

    [System.Serializable]
    public class EnemyObject
    {
        public GameObject enemy;
        public short spawnMinWave;
    }

    [System.Serializable]
    public class SpawnEnemy
    {
        public EnemyObject[] enemies;
        public Transform spawnPos;
        public float curentSpawnRate;
        public float minSpawnRate;
        public float spawnVariant;
        [HideInInspector]
        public float nextSpawn;
        public bool delayedFirstSpawn;
        public float spawnGrowing;
        public short spawnCount;
        public bool singleSpawn;
        [HideInInspector]
        public bool spawned = false;
        public bool isTopSpawn;
        [HideInInspector]
        public Vector2 selectedSpawnPos;
        public bool hasIndicator;
    }

    [System.Serializable]
    public class SpawnPowerUp
    {
        public Transform spawn;
        public GameObject[] powerUps;
        public float spawnDist;
        public short spawnCount;
        public float spawnRate;
        [HideInInspector]
        public float nextSpaw;
        public float spawnVariant;
    }

    [System.Serializable]
    public class SpawnBoss
    {
        public GameObject boss;
        [HideInInspector]
        public bool bossSpawned = false;
        public Transform bossSpawn;
        public float waitForCall = 1.5f;
        public float nextCall;
    }

    [System.Serializable]
    public class Stage
    {
        public short spawnCount;
        [HideInInspector]
        public short spawnMax;
        public SpawnEnemy[] enemySpawns;
        public SpawnBoss bossSpawn;
        public SpawnPowerUp powerUpSpawn;
    }

    public Stage[] stages;
    public short currentStage;
    public Image stageMessage;
    public float shineDurationPerShine ;
    public float shineTotalDuration;
    private float nextShine;
    private float shiningDuration;
    public GameObject indicator;
    private RandomHelper randomHelper;
    private AudioSource musicPlayer;
    public AudioClip bossMusic;
    public AudioClip stageMusic;
    private bool bossMusicOn = false;
    private Stage stage;
    private bool pauseSpawn;

    void Start()
    {
        foreach(Stage stage in stages)
        {
            stage.spawnMax = stage.spawnCount;
        }

        randomHelper = new RandomHelper();
        foreach (SpawnEnemy spawn in stages[currentStage].enemySpawns)
        {
            spawn.nextSpawn = Time.time + spawn.curentSpawnRate;
            //Usar essa variável para criar os indicadores de spawn
            spawn.selectedSpawnPos = randomHelper.RandomSpawnPos(spawn.spawnCount, spawn.spawnPos.position.y);
            if (spawn.hasIndicator)
            {
                DoSpawnIndicator(spawn.selectedSpawnPos, spawn.nextSpawn);
            }
        }
        stages[currentStage].powerUpSpawn.nextSpaw = Time.time + stages[currentStage].powerUpSpawn.spawnRate;
        StartShine();
        musicPlayer = gameObject.GetComponent<AudioSource>();
        musicPlayer.clip = stageMusic;
        musicPlayer.Play();
    }

    void StartShine()
    {
        shiningDuration = Time.time + shineTotalDuration;
        nextShine = Time.time + shineDurationPerShine;
    }

    private void FixedUpdate()
    {
        if (stages.Length < currentStage)
            return;

        stage = stages[currentStage];

        if (stage.spawnCount == 0)
        {
            //TODO up in one stage on all enemies are destroyed and spawn count == 0
            if (stage.bossSpawn.boss == null)
            {
                currentStage++;
                StartShine();
            } else
            {
                SpawnBoss spawnBoss = stage.bossSpawn;
                spawnBoss.nextCall = spawnBoss.nextCall == 0 ? Time.time + spawnBoss.waitForCall : spawnBoss.nextCall;
                if (!spawnBoss.bossSpawned && Time.time > spawnBoss.nextCall)
                {
                    if (bossMusicOn == false)
                    {
                        bossMusicOn = true;
                        musicPlayer.Stop();
                        Invoke("WhyDoIHearBossMusic", 3f);
                    }
                    stage.bossSpawn.bossSpawned = true;
                    pauseSpawn = true;
                    Invoke("SurpriseMotherfuckerTheBossIsHere", 6f);
                }
            }
        }

        if (Time.time > stage.powerUpSpawn.nextSpaw)
        {
            SpawnPowerUp spawnPowerUp = stage.powerUpSpawn;

            spawnPowerUp.nextSpaw = Time.time
                + stage.powerUpSpawn.spawnRate
                + Random.Range(-stage.powerUpSpawn.spawnVariant, stage.powerUpSpawn.spawnVariant);
            DoSpawn(spawnPowerUp.powerUps, randomHelper.RandomSpawnPos(spawnPowerUp.spawnCount, spawnPowerUp.spawn.position.y));
        }

        if (!pauseSpawn)
        {
            foreach (SpawnEnemy spawn in stage.enemySpawns)
            {
                if (Time.time > spawn.nextSpawn
                    && (!spawn.singleSpawn || (spawn.singleSpawn && !spawn.spawned)))
                {
                    if (stage.spawnCount > 0 || spawn.isTopSpawn)
                    {
                        if (!spawn.isTopSpawn)
                        {
                            stage.spawnCount--;
                        }
                        spawn.spawned = true;
                        DoEnemySpawn(spawn.enemies, spawn.selectedSpawnPos, stage);
                        spawn.selectedSpawnPos = randomHelper.RandomSpawnPos(spawn.spawnCount, spawn.spawnPos.position.y);
                        spawn.nextSpawn = Time.time + spawn.curentSpawnRate + Random.Range(-spawn.spawnVariant, spawn.spawnVariant);
                        float newSpawnRate = spawn.curentSpawnRate - spawn.spawnGrowing;
                        spawn.curentSpawnRate = newSpawnRate >= spawn.minSpawnRate ? newSpawnRate : spawn.minSpawnRate;
                        if (spawn.hasIndicator)
                        {
                            DoSpawnIndicator(spawn.selectedSpawnPos, spawn.nextSpawn);
                        }
                    }
                }
            }
        }

        if (Time.time < shiningDuration)
        {
            if (Time.time > nextShine)
            {
                nextShine = Time.time + shineDurationPerShine;
                stageMessage.enabled = !stageMessage.enabled;
            }
        } else
        {
            stageMessage.enabled = false;
        }
    }

    void DoEnemySpawn(EnemyObject[] enemyObjects, Vector2 spawnPos, Stage stage)
    {
        if (enemyObjects.Length == 0)
            return;
        int randomEnemy = Random.Range(0, enemyObjects.Length);

        while (enemyObjects[randomEnemy].spawnMinWave > stage.spawnMax - stage.spawnCount)
        {
            randomEnemy = Random.Range(0, enemyObjects.Length);
        }

        GameObject obj = Instantiate(enemyObjects[randomEnemy].enemy, spawnPos, Quaternion.identity);
    }

    void DoSpawn(GameObject[] objects, Vector2 spawnPos)
    {
        if (objects.Length == 0)
            return;
        //-spawnMaxX + (spawnMaxX * 2 / (spawnCount - 1) * selectedSpawn)
        int randomEnemy = Random.Range(0, objects.Length);
        GameObject obj = Instantiate(objects[randomEnemy], spawnPos, Quaternion.identity);
    }

    void DoSpawnIndicator(Vector2 spawnPos, float indicatorDuration)
    {
        float spawnY = spawnPos.y;
        spawnY += spawnY > 0 ? -0.75f : 0.75f;
        GameObject indicatorObj = Instantiate(indicator, new Vector2(spawnPos.x, spawnY), Quaternion.identity);
        Destroy(indicatorObj, indicatorDuration);
    }

    private void WhyDoIHearBossMusic()
    {
            musicPlayer.clip = bossMusic;
            musicPlayer.Play();
            musicPlayer.volume = 0.8f;
    }

    void SurpriseMotherfuckerTheBossIsHere()
    {
        Instantiate(stage.bossSpawn.boss, stage.bossSpawn.bossSpawn.position, Quaternion.identity);
        pauseSpawn = false;
    }
}
