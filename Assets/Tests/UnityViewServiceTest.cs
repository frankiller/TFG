//using Entitas.Unity;
//using NSubstitute;
//using NUnit.Framework;
//using NUnit.Framework.Internal;
//using Shouldly;
//using UnityEngine;

//namespace Tests
//{
//    public class UnityViewServiceTest
//    {
//        IViewService unityViewService;
//        IUnityEngineWrapper unityEngineWrapperMock;
//        Contexts contexts;
//        GameEntity entity;

//        [SetUp]
//        public void Initialize()
//        {
//            unityEngineWrapperMock = Substitute.For<IUnityEngineWrapper>();

//            contexts = Contexts.sharedInstance;
//            contexts.game.ReplaceUnityEngineWrapper(unityEngineWrapperMock);

//            unityViewService = new UnityViewService();
//        }

//        [Test]
//        public void ShouldImplementIViewServiceInterface()
//        {
//            unityViewService.ShouldBeAssignableTo<IViewService>();
//        }

//        [Test]
//        public void LoadAssetShouldInstantiateAGameObjectWithEntityLinked()
//        {
//            string fakeAssetName = "fakeAssetName";

//            var fakeGameObject = new GameObject("FakeObject");
//            fakeGameObject.AddComponent<UnityGameView>();

//            unityEngineWrapperMock.Load(fakeAssetName).Returns(fakeGameObject);
//            unityEngineWrapperMock.Instantiate(fakeGameObject).Returns(fakeGameObject);

//            entity = contexts.game.CreateEntity();

//            unityViewService.LoadAsset(contexts, entity, fakeAssetName);

//            var gameObjectLink = fakeGameObject.GetEntityLink(); 
            
//            gameObjectLink.entity.ShouldBeSameAs(entity);
//        }

//        [Test]
//        public void LoadAssetShouldInstantiateAGameObjectWithEventListenersRegistered()
//        {
//            string fakeAssetName = "fakeAssetName";

//            var fakeGameObject = new GameObject("FakeObject");
//            fakeGameObject.AddComponent<UnityGameView>();
//            fakeGameObject.AddComponent<PositionListener>();

//            unityEngineWrapperMock.Load(fakeAssetName).Returns(fakeGameObject);
//            unityEngineWrapperMock.Instantiate(fakeGameObject).Returns(fakeGameObject);

//            entity = contexts.game.CreateEntity();

//            unityViewService.LoadAsset(contexts, entity, fakeAssetName);

//            //entity.hasPositionListener.ShouldBeTrue();
//        }

//        [Test]
//        public void LoadAssetShouldCallLogMethodWhenAGameObjectIsNotInstantiated()
//        {
//            string fakeAssetName = "fakeAssetName";

//            unityViewService.LoadAsset(contexts, entity, fakeAssetName);

//            contexts.game.unityEngineWrapper.Instance.Received().Log("BadLoadGameObject: fakeAssetName");
//        }
//    }
//}
