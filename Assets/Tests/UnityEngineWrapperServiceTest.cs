//using NUnit.Framework;
//using Shouldly;
//using UnityEngine;

//namespace Tests
//{
//    public class UnityEngineWrapperServiceTest
//    {
//        IUnityEngineWrapper unityEngineWrapper;

//        [SetUp]
//        public void Initialize()
//        {
//            unityEngineWrapper = new UnityEngineWrapperService();
//        }

//        [Test]
//        public void LoadShouldReturnAGameObject()
//        {
//            unityEngineWrapper.Load("Player").ShouldNotBe(null);
//        }

//        [Test]
//        public void LoadShouldReturnNullWhenGameObjectCouldNotBeLoaded()
//        {
//            unityEngineWrapper.Load("fakeAssetName").ShouldBe(null);
//        }

//        [Test]
//        public void InstantiateShouldReturnAGameObject()
//        {
//            unityEngineWrapper.Instantiate(new GameObject()).ShouldNotBe(null);
//        }

//        [Test]
//        public void InstantiateShouldReturnNullIfGameObjectCouldNotBeInstantiated()
//        {
//            unityEngineWrapper.Instantiate(null).ShouldBe(null);
//        }
//    }
//}
