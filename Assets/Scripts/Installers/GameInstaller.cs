using Core;
using Solitaire.Dragging;
using Zenject;

namespace Solitaire.Installers
{
    public class GameInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameController>().AsSingle();
            Container.Bind<DragController>().AsSingle();
        }
    }
}