using NSubstitute;
using Shouldly;
using UnityEngine;

class UnityViewServiceTest : EntitasTests
{
    void when_loading_asset()
    {
        IViewService unityViewService = null;
        IUnityEngineWrapper unityEngineWrapperMock = null;
        GameEntity entity;

        before = () =>
        {
            Setup();

            unityEngineWrapperMock  = Substitute.For<IUnityEngineWrapper>();
            Contexts.game.ReplaceUnityEngineWrapper(unityEngineWrapperMock);

            unityViewService = new UnityViewService();

            
        };

        it["should implement IViewService interface"] = () =>
        {
            unityViewService.ShouldBeAssignableTo<IViewService>();
        };

        it[""] = () =>
        {

        };
    }
}
