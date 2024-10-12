using Solitaire.Cards;
using UnityEngine;
using Zenject;

namespace Solitaire.Installers
{
    [CreateAssetMenu(menuName = "SO/Installers/CardVisualInstaller", fileName = "CardVisualInstaller", order = 0)]
    public class CardVisualInstaller: ScriptableObjectInstaller
    {
        [SerializeField] private CardView.VisualSettings _settings;

        public override void InstallBindings()
        {
            Container.BindInstance(_settings);
        }
    }
}