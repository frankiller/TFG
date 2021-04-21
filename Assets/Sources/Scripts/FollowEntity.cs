using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FollowEntity : MonoBehaviour
{
    public Entity entityToFollow;
    public float3 offset = float3.zero;

    private EntityManager _entityManager;

    private void Awake() 
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void LateUpdate() 
    {
        if(entityToFollow == Entity.Null) { return; }
        
        Translation entPos = _entityManager.GetComponentData<Translation>(entityToFollow);

        transform.position = entPos.Value + offset;
    }
}
