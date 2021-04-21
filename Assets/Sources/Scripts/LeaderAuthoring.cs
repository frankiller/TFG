using System;
using Unity.Entities;
using UnityEngine;

[AddComponentMenu("Custom Authoring/Leader Authoring")]
public class LeaderAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    //Can be a list of follower objects, ¿camera switching?
    [NonSerialized] public GameObject followerObject;

    private void Awake() 
    {
        followerObject = Camera.main.gameObject;
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        followerObject = Camera.main.gameObject;

        FollowEntity followEntity = followerObject.GetComponent<FollowEntity>();
        
        if (followEntity == null)
        {
            followEntity = followerObject.AddComponent<FollowEntity>();
        }

        followEntity.entityToFollow = entity;
    }
}
