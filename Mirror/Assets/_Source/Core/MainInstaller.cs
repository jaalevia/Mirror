using LobbySystem;
using PlayerSystem;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private InputListener _inputListener;
    [SerializeField] private MainMenu _mainMenu;

    public override void InstallBindings()
    {
        Container
            .Bind<InputListener>()
            .FromInstance(_inputListener)
            .AsSingle();
        Container
            .Bind<MainMenu>()
            .FromInstance(_mainMenu)
            .AsSingle();
    }
}