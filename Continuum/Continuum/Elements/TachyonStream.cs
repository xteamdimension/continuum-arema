using System;
using Microsoft.Xna.Framework;
using Continuum.State;
using Continuum.Utilities;

namespace Continuum.Elements
{
    public class TachyonStream : Animation
    {
        public float Duration;
        public float Wait;
        public float Falloff;

        public TachyonStream(int XPosition, float Duration, string SequenceTexture, GameState gameState) 
            : base(new Vector2(XPosition, Constants.SCREEN_HEIGHT / 2), SequenceTexture, 40, 2, 20, (int)(40*3/Duration), 3, 0, 0, gameState)
        {
            this.Duration = Duration;
            Wait = 0;

            StretchRatio = (float)Constants.TACHYON_STREAM_WIDTH / (float)this.Width;

            /*this.Width = (int)(stretchRatio * this.Width);
            this.Height = (int)(stretchRatio * this.Height);*/
        }

        public TachyonStream() { }

        public override Vector2 EvaluatePosition(float Delta)
        {
            if (lifeState != LifeState.DEAD && gs.playerTime.continuum > 0)
            {
                //if(Math.Abs(this.startPosition.X - gs.playerPosition.X) < Constants.TACHYON_STREAM_WIDTH / 2)

                if (Wait <= 0)
                {
                    Falloff = (-Math.Abs((Duration / 2) - elapsedTime) + (Duration/2)) / (Duration/2);
                    Tachyon t = new Tachyon((int)(CurrentPosition.X + Utility.NextRandom(-(Constants.TACHYON_STREAM_WIDTH / 2), (Constants.TACHYON_STREAM_WIDTH / 2)) * Falloff), Utility.NextRandom(Constants.MIN_TACHYON_SPEED, Constants.MAX_TACHYON_SPEED), TextureConstant.TACHYON, gs);
                    gs.tachyons.AddFirst(t);
                    gs.collisions.Insert(t);
                    Wait = Utility.NextRandom(Constants.MIN_TACHYON_DELAY_INDEX, Constants.MAX_TACHYON_DELAY_INDEX) / (Falloff + Constants.MIN_TACHYON_DELAY_INDEX);
                }
                if(gs.playerTime.continuum > 0)
                    Wait = Wait - gs.playerTime.elapsedContinuumTime;
            }
            return base.EvaluatePosition(Delta);
        }
    }
}
