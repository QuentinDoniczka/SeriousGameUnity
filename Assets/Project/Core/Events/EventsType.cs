// _Project/Core/Events/GameEvents.cs
namespace Project.Core.Events
{
    public enum GameEventType
    {
        SceneLoaded,
        GameStarted,
        GamePaused,
        GameResumed,
    }
    public enum NavigationEventType
    {
        ToLogin,
        ToRegister,
        ToGame,
        ToMainMenu
    }
}