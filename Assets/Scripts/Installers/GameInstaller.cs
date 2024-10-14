using Solitaire.Cards;
using Solitaire.Core;
using Solitaire.Dragging;
using UnityEngine;
using Zenject;

namespace Solitaire.Installers
{
    public class GameInstaller: MonoInstaller
    {
        [SerializeField] private GameController.Map _gameMap;
        [SerializeField] private GameObject _cardPrefab;
        
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<GameController>()
                .AsSingle()
                .WithArguments(_gameMap)
                .NonLazy();
            
            Container
                .BindInterfacesTo<ClassicMode>()
                .AsSingle();

            Container
                .Bind<DragController>()
                .AsSingle();
            
            Container
                .BindFactory<CardFacade.Values, CardFacade.Suits, bool, CardFacade, CardFacade.Factory>()
                .FromMonoPoolableMemoryPool(x =>
                    x.WithInitialSize(13 * 4)
                        .FromComponentInNewPrefab(_cardPrefab)
                        .UnderTransformGroup("CardPool")
                );
        }
    }
}