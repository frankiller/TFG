using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class TimerSystem : SystemBase
{
    private EntityQuery _timerQuery;
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    private const float TIME_SCALE = 1f;

    protected float TimeScale => TIME_SCALE;
    private float ScaledTime => Time.DeltaTime * TimeScale;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        _timerQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<Timer>());
    }

    protected override void OnUpdate()
    {
        var job = new UpdateTimerJob
        {
            EntityTypeHandle = GetEntityTypeHandle(),
            TimerTypeHandle = GetComponentTypeHandle<Timer>(),
            CurrentTimeInterval = ScaledTime,
            CommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter()
        };

        Dependency = job.ScheduleParallel(_timerQuery, Dependency);
        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    [BurstCompile]
    private struct UpdateTimerJob : IJobChunk
    {
        [ReadOnly] public EntityTypeHandle EntityTypeHandle;
        [ReadOnly] public float CurrentTimeInterval;

        public ComponentTypeHandle<Timer> TimerTypeHandle;
        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var entities = chunk.GetNativeArray(EntityTypeHandle);
            var timers = chunk.GetNativeArray(TimerTypeHandle);

            for (var i = 0; i < entities.Length; ++i)
            {
                var timer = timers[i];

                timer.ElapsedTime += CurrentTimeInterval;

                if (timer.IsDone)
                {
                    var sortKey = firstEntityIndex + i;
                    CommandBuffer.RemoveComponent<Timer>(sortKey, entities[i]);
                }

                timers[i] = timer;
            }
        }
    }
}

public enum ChronometerAction
{
    StandBy,
    Run,
    Pause,
    Stop,
    Reset,
    IncreaseTime
}

public struct ChronometerData : IComponentData
{
    public ChronometerAction Action;
    public float PenaltyAmountTime;
}

public class ChronometerUpdateSystem : SystemBase
{
    private EntityQuery _chronometerQuery;
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    private const float TIME_SCALE = 1f;

    protected float TimeScale => TIME_SCALE;
    private float ScaledTime => Time.DeltaTime * TimeScale;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        _chronometerQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<Chronometer>());
    }

    protected override void OnUpdate()
    {
        var gameManagerEntity = GetSingletonEntity<GameManagerTag>();

        var job = new UpdateChronometerJob
        {
            EntityTypeHandle = GetEntityTypeHandle(),
            ChronometerTypeHandle = GetComponentTypeHandle<Chronometer>(),
            ChronometerData = GetSingleton<ChronometerData>(),
            CurrentTimeInterval = ScaledTime,
            CommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
            GameManagerEntity = gameManagerEntity,
            HasInGameTag = EntityManager.HasComponent<InGameTag>(gameManagerEntity)
        };

        Dependency = job.ScheduleParallel(_chronometerQuery, Dependency);
        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    [BurstCompile]
    private struct UpdateChronometerJob : IJobChunk
    {
        [ReadOnly] public EntityTypeHandle EntityTypeHandle;
        [ReadOnly] public float CurrentTimeInterval;
        [ReadOnly] public bool HasInGameTag;

        public EntityCommandBuffer.ParallelWriter CommandBuffer;
        public ComponentTypeHandle<Chronometer> ChronometerTypeHandle;
        public ChronometerData ChronometerData;
        public Entity GameManagerEntity;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var entities = chunk.GetNativeArray(EntityTypeHandle);
            var chronometers = chunk.GetNativeArray(ChronometerTypeHandle);

            for (var i = 0; i < entities.Length; ++i)
            {
                var chronometer = chronometers[i];
                var sortKey = firstEntityIndex + i;

                switch (ChronometerData.Action)
                {
                    case ChronometerAction.StandBy:
                        if (HasInGameTag)
                        {
                            ChronometerData.Action = ChronometerAction.Run;
                            CommandBuffer.SetComponent(sortKey, GameManagerEntity, ChronometerData);
                        }
                        break;

                    case ChronometerAction.Run:
                        chronometer.Run(CurrentTimeInterval);
                        break;

                    case ChronometerAction.Pause: //Cuando se pausa?
                        if (chronometer.IsPaused) continue;
                        chronometer.Pause();
                        break;

                    case ChronometerAction.Stop: //Se para cuando se acierta el máximo de preguntas
                        chronometer.Stop();
                        
                        ChronometerData.Action = ChronometerAction.StandBy;
                        CommandBuffer.SetComponent(sortKey, GameManagerEntity, ChronometerData);
                        break;

                    case ChronometerAction.Reset: //Cuuando se resetea?
                        chronometer.Reset();
                        break;

                    case ChronometerAction.IncreaseTime:
                        if (chronometer.IsPaused) continue;
                        chronometer.IncreaseTime(ChronometerData.PenaltyAmountTime);

                        ChronometerData.Action = ChronometerAction.Run;
                        CommandBuffer.SetComponent(sortKey, GameManagerEntity, ChronometerData);
                        break;

                    default:
                        chronometer.Pause();
                        break;
                }

                chronometers[i] = chronometer;
            }
        }
    }
}

public class ChronometerManagerSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<InGameTag>();
    }

    protected override void OnUpdate()
    {
        var chronometer = GetSingleton<Chronometer>();
        var chronometerLabelData = EntityManager.GetComponentObject<UiChronometerTextData>(GetSingletonEntity<MenuManagerTag>());

        var elapsedTime = chronometer.ElapsedTime;
        var secondsRemainder = Mathf.Floor(elapsedTime % 60 * 100) / 100.0f;
        var minutes = (int)(elapsedTime / 60) % 60;

        chronometerLabelData.Value.text = Math.Abs(secondsRemainder - 60f) < .5f ? $"{minutes + 1:00} : 00" : $"{minutes:00} : {secondsRemainder:00}";
    }
}
