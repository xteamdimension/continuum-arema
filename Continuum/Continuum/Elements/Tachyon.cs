using Microsoft.Xna.Framework;
using Continuum.State;
using Continuum.Utilities;

namespace Continuum.Elements
{
    public class Tachyon : TimeTraveler
    {
        private float scale;

        public override Rectangle DestinationRectangle
        {
            get
            {
                return Utility.newRectangleFromCenterPosition(new Vector2((int)CurrentPosition.X, (int)CurrentPosition.Y), (int)(Width * scale), (int)(Height * scale));
            }
        }

        public Tachyon(int XPosition, float Speed, string TextureName, GameState gameState)
        {
            scale = Utility.NextRandom(0.5f, 1f);
            InitializeTimeTraveler(new Vector2(XPosition, 0), Speed, TextureName, gameState);
        }

        public Tachyon() { }

        public override Vector2 EvaluatePosition(float Delta)
        {
            Vector2 evaluate = startPosition;
            evaluate.Y = startPosition.Y + Delta;
            return evaluate;
        }

        public override void HasCollided(int Value, object Arg)
        {
            lifeState = LifeState.DEAD;
        }
    }
}
