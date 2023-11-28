using UnityEngine;

public class CollectibleSpawnManager : EntitySpawnManager
{
    [SerializeField] CollectibleData[] collectibleData;
    [SerializeField] float spawnInterval = 1f;

    EntitySpawner<CollectibleBase> spawner;

    CountDownTimer spawnTimer;
    int counter;

    public override void Spawn() => spawner.Spawn();

    protected override void Awake()
    {
        base.Awake();

        spawner = new EntitySpawner<CollectibleBase>(new EntityFactory<CollectibleBase>(collectibleData), spawnPointStrategy);

        spawnTimer = new CountDownTimer(spawnInterval);
        spawnTimer.OnTimerStop += () =>
        {
            if(counter++ >= spawnPoints.Length)
            {
                spawnTimer.Stop();
                return;
            }
            Spawn();
            spawnTimer.Start();
        };
    }
    private void Start() => spawnTimer.Start();
    private void Update() => spawnTimer.Tick(Time.deltaTime);
}