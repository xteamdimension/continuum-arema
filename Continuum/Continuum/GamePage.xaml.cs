using System;
using System.Windows;
using System.Windows.Navigation;
using Continuum.Elements;
using Continuum.Management;
using Continuum.State;
using Continuum.Utilities;
using Continuum.Scores;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Continuum
{
    public partial class GamePage : PhoneApplicationPage
    {
        public ContentManager contentManager;
        
        public GameTimer gameTimer;

        int counterDraw;
        int fpsDraw;
        double lastMsDraw;

        public SpriteBatch spriteBatch;
        public LineRenderer lineRenderer;
        
        //Silverlight Rendering
        private UIElementRenderer UIrenderer;
        private Rectangle SilverlightRectangle;

        //State
        public GameState gs;

        //Effetti grafici
        public int bloodIndex;
        public int shadowIndex;
        public int scopeTextureIndex;
        private float fadeInterpolationIndex;
        public Rectangle shadowSource;
        public Rectangle bloodSource;
        public Color timeColor;
        public Color gridColor;

        //Managers
        public TimeManager timeManager;
        public LevelManager levelManager;
        public InputManager inputManager;
        public BackgroundManager backgroundManager;
        public BulletsManager bulletsManager;
        public TachyonManager tachyonManager;
        public AnimationManager animationManager;
        public AsteroidManager asteroidManager;
        public EnemyManager enemyManager;
        public CollisionDetector collisionDetector;
        public PowerUpManager powerUpManager;
        public RandomizerManager randomizerManager;
        public ExplosionParticleManager explosionParticleManager;
        public Player player;

        //Font
        private SpriteFont debugFont;

        //Debug strings
        private string debugText;
        private string debugText2;

        //Scores
        private Score[] scores;

        private bool morto = false;

        public GamePage()
        {

            // Get the content manager from the application
            contentManager = (Application.Current as App).Content;

            InitializeComponent();

            LayoutUpdated += new EventHandler(GamePage_LayoutUpdated);

            // Create a timer for this page
            gameTimer = new GameTimer();
            gameTimer.UpdateInterval = TimeSpan.Zero;
            gameTimer.Update += OnUpdate;
            gameTimer.Draw += OnDraw;

            counterDraw = 0;
            lastMsDraw = 0;
            fpsDraw = 0;

            //Setta la dimensione dello schermo a quella del device attuale
            Constants.SCREEN_WIDTH = SharedGraphicsDeviceManager.DefaultBackBufferWidth;
            Constants.SCREEN_HEIGHT = SharedGraphicsDeviceManager.DefaultBackBufferHeight;

            //Crea un nuovo GameState o utilizza quello già esistente.
            App app = ((App)App.Current);
            if (app.gs == null)
                app.gs = new GameState();
            gs = app.gs;
            levelManager = new LevelManager(gs, "RandomLevel.xml");
            timeManager = new TimeManager(gs);
            inputManager = new InputManager(gs, timeManager);
            backgroundManager = new BackgroundManager(gs, levelManager.NumberOfBackgroundLevels);
            bulletsManager = new BulletsManager(gs);
            tachyonManager = new TachyonManager(gs);
            animationManager = new AnimationManager(gs);
            asteroidManager = new AsteroidManager(gs);
            enemyManager = new EnemyManager(gs);
            powerUpManager = new PowerUpManager(gs);
            collisionDetector = new CollisionDetector(gs);
            randomizerManager = new RandomizerManager(gs);
            explosionParticleManager = new ExplosionParticleManager(gs);

            player = new Player(gs);

            gs.timeTank = 0;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (gs.playerLifeState == LifeState.DELETING)
            {
                NavigationService.GoBack();
                ((App)App.Current).gs = null;
            }
            else
            {
                if (MessageBox.Show("Press OK to Exit\nPress CANCEL to Continue", "PAUSED", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    ((App)App.Current).gs = null;
                    NavigationService.GoBack();
                }
                else
                {
                    e.Cancel = true;
                    inputManager.RecalibrateAccelerometer();
                }
                /*else
                {
                    e.Cancel = true;
                    gs.albertTime.continuum = gs.alberttimecontunuumbackup.Value;
                    gs.playerTime.continuum = gs.playertimecontunuumbackup.Value;
                    gs.levelTime.continuum = gs.leveltimecontunuumbackup.Value;
                    inputManager.RecalibrateAccelerometer();
                }*/
            }
        }

        void GamePage_LayoutUpdated(object sender, EventArgs e)
        {
            // Crea il UIElementRenderer per disegnare la pagina XAML in un Texture2D.

            // Check for 0 because when we navigate away the LayoutUpdate event
            // is raised but ActualWidth and ActualHeight will be 0 in that case.
            if ((ActualWidth > 0) && (ActualHeight > 0))
            {
                SharedGraphicsDeviceManager.Current.PreferredBackBufferWidth = (int)ActualWidth;
                SharedGraphicsDeviceManager.Current.PreferredBackBufferHeight = (int)ActualHeight;
            }

            if (null == UIrenderer)
            {
                UIrenderer = new UIElementRenderer(this, (int)ActualWidth, (int)ActualHeight);
                SilverlightRectangle = new Rectangle(0, 0, (int)ActualWidth, (int)ActualHeight);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);
            SharedGraphicsDeviceManager.Current.GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.One;

            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);
            lineRenderer = new LineRenderer(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            //Loading...
            levelManager.getTextures(contentManager);
            levelManager.getRandomVariables();
            levelManager.goToStartLevel();

            player.TextureIndex = gs.textureIndices.GetTextureIndex("playership");
            shadowIndex = gs.textureIndices.GetTextureIndex("shadow");
            bloodIndex = gs.textureIndices.GetTextureIndex("blood");
            scopeTextureIndex = gs.textureIndices.GetTextureIndex(TextureConstant.SCOPE);

            debugFont = contentManager.Load<SpriteFont>("debugFont");

            //Colore default
            timeColor = Color.Black;
            gridColor = Color.Yellow * 0.5f;
            
            // Start the timer
            gameTimer.Start();

            //Disable UserIdleDetection
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            gameTimer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            TimeTankBar.Value = 100.0f * gs.timeTank / Constants.MAX_TIME_TANK_VALUE;
            if (gs.levelTime.time <= Constants.INITIAL_BLACK_DURATION + Constants.INITIAL_FADE_DURATION)
                if (gs.levelTime.time <= Constants.INITIAL_BLACK_DURATION)
                    timeColor = Color.Black;
                else
                {
                    fadeInterpolationIndex = (gs.levelTime.time - Constants.INITIAL_BLACK_DURATION) / (float)Constants.INITIAL_FADE_DURATION;
                    timeColor = Color.Lerp(Color.Black, Color.White, fadeInterpolationIndex);
                }
            else
                if (gs.levelTime.continuum < 0)
                    timeColor = Color.Lerp(Color.White, Constants.BACK_IN_TIME_COLOR, Math.Abs(gs.levelTime.continuum));
                /*else
                    timeColor = Color.White;*/

            if (gs.PlayerLife <= Utilities.Constants.PLAYER_LIFE_CRITICAL_VALUE)
            {
                bloodSource = new Rectangle(0,0,240,400);
            }

            if (gs.playerLifeState != LifeState.DELETING)
            {
                debugText = "LevelTime: " + gs.levelTime.time + "\nPlayerTime: " + gs.playerTime.time + "\nTimeTank: " + gs.timeTank + "\nPlayerLife: " + gs.PlayerLife + "\np1=" + gs.gridHorizontalPoints[0][0] + "\np2=" + gs.gridHorizontalPoints[0][1] + "\nGranades=" + gs.PlayerGranadeCount;

                timeManager.Update(e);

                if (gs.levelTime.continuum < 0)
                    shadowSource = new Rectangle((Constants.SCREEN_WIDTH / 2) - (int)((Constants.SCREEN_WIDTH / 2) * gs.playerTime.continuum * (1f / Constants.CONTINUUM_MIN)), (Constants.SCREEN_HEIGHT / 2) - (int)((Constants.SCREEN_HEIGHT / 2) * gs.playerTime.continuum * (1f / Constants.CONTINUUM_MIN)), (Constants.SCREEN_WIDTH / 2), (Constants.SCREEN_HEIGHT / 2));

                player.Update(inputManager);
                inputManager.Update();
                levelManager.Update();
                backgroundManager.Update();
                bulletsManager.Update();
                tachyonManager.Update();
                animationManager.Update();
                asteroidManager.Update();
                enemyManager.Update();
                powerUpManager.Update();
                collisionDetector.Update();
                randomizerManager.Update();
                explosionParticleManager.Update();
            }
            else
            {
                if (!morto)
                {
                    EndRectangle.Visibility = System.Windows.Visibility.Visible;
                    NameEndTextBox.Visibility = System.Windows.Visibility.Visible;
                    SaveButton.Visibility = System.Windows.Visibility.Visible;
                    CancelButton.Visibility = System.Windows.Visibility.Visible;
                }
                morto = true;
            }

        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.CornflowerBlue);
            UIrenderer.Render();
            spriteBatch.Begin();

            for (int i = 0; i < backgroundManager.BackgroundLevels.Length && backgroundManager.BackgroundLevels[i] != null; i++)
            {
                BackgroundTexture bg = backgroundManager.BackgroundLevels[i];
                if (bg.lifeState != LifeState.DEAD && bg.lifeState != LifeState.DAMAGED)
                {
                    switch (bg.lifeState)
                    {
                        case LifeState.NORMAL:
                            spriteBatch.Draw(gs.textures[bg.TextureIndex], bg.DestinationRectangle, timeColor);
                            spriteBatch.Draw(gs.textures[bg.TextureIndex], bg.DestinationRectangle2, timeColor);
                            break;
                        case LifeState.TRANSITIONING:
                            spriteBatch.Draw(gs.textures[bg.TransitionTextureIndex.Value], bg.DestinationRectangle, timeColor);
                            spriteBatch.Draw(gs.textures[bg.TextureIndex], bg.DestinationRectangle2, timeColor);
                            break;
                        case LifeState.BEINGREPLACED:
                            spriteBatch.Draw(gs.textures[bg.TextureIndex], bg.DestinationRectangle, timeColor);
                            spriteBatch.Draw(gs.textures[bg.ReplacingTextureIndex], bg.DestinationRectangle2, timeColor);
                            break;
                    }
                }
            }

            foreach (Animation x in gs.animations)
                if (x.lifeState != LifeState.DEAD)
                {
                    spriteBatch.Draw(gs.textures[x.TextureIndex], x.DestinationRectangle, x.SourceRectangle, timeColor, x.Rotation, x.Origin, SpriteEffects.None, 0);
                }

            if(gs.playerLifeState != LifeState.DEAD)
                spriteBatch.Draw(gs.textures[player.TextureIndex], new Vector2(gs.playerPosition.X - gs.textures[0].Width / 2, gs.playerPosition.Y - gs.textures[0].Height / 2), timeColor);

            foreach (PowerUp x in gs.powerUps)
                if (x.lifeState != LifeState.DEAD)
                {
                    spriteBatch.Draw(gs.textures[x.TextureIndex], x.DestinationRectangle, x.SourceRectangle, timeColor, x.Rotation, x.Origin, SpriteEffects.None, 0);
                }

            foreach (Tachyon x in gs.tachyons)
                if (x.lifeState != LifeState.DEAD)
                {
                    spriteBatch.Draw(gs.textures[x.TextureIndex], x.DestinationRectangle, x.SourceRectangle, timeColor, x.Rotation, x.Origin, SpriteEffects.None, 0);
                }

            foreach (Bullet x in gs.bullets)
                if (x.lifeState != LifeState.DEAD)
                {
                    spriteBatch.Draw(gs.textures[x.TextureIndex], x.DestinationRectangle, x.SourceRectangle, timeColor, x.Rotation, x.Origin, SpriteEffects.None, 0);
                }

            foreach (Asteroid x in gs.asteroids)
                if (x.lifeState != LifeState.DEAD)
                {
                    spriteBatch.Draw(gs.textures[x.TextureIndex], x.DestinationRectangle, x.SourceRectangle, timeColor, x.Rotation, x.Origin, SpriteEffects.None, 0);
                }

            foreach (Enemy x in gs.enemies)
                if (x.lifeState != LifeState.DEAD)
                {
                    Color enemyColor = new Color(timeColor.ToVector3() * x.LifeColor.ToVector3());
                    spriteBatch.Draw(gs.textures[x.TextureIndex], x.DestinationRectangle, x.SourceRectangle, enemyColor, x.Rotation, x.Origin, SpriteEffects.None, 0);
                }

            foreach (ExplosionParticle x in gs.explosionParticles)
                if (x.lifeState != LifeState.DEAD)
                {
                    spriteBatch.Draw(gs.textures[x.TextureIndex], x.DestinationRectangle, x.SourceRectangle, timeColor * x.Alpha, x.Rotation, x.Origin, SpriteEffects.None, 0);
                }

            //Se ci sono da disegnare le linee
            if (timeManager.DrawLines)
            {
                if(timeManager.State == TimeState.ENTER_PLASMA_GRANADE_LAUNCHER)
                    timeColor = Color.Lerp(Color.White, Color.LightGreen, timeManager.NormalizedDelta);

                if (timeManager.State == TimeState.EXIT_PLASMA_GRANADE_LAUNCHER)
                    timeColor = Color.Lerp(Color.LightGreen, Color.White, timeManager.NormalizedDelta);

                for (int i = 0; i < gs.gridVerticalPoints.Length; i++)
                    lineRenderer.DrawLine(spriteBatch, gs.gridVerticalPoints[i][0], gs.gridVerticalPoints[i][1], 1, gridColor);

                for (int i = 0; i < gs.gridHorizontalPoints.Length; i++)
                    lineRenderer.DrawLine(spriteBatch, gs.gridHorizontalPoints[i][0], gs.gridHorizontalPoints[i][1], 1, gridColor);
            }

            //ULTRAPSEUDODEBUGLOL
            if (timeManager.State == TimeState.CALIBRATION_PLASMA_GRANADE_PATH_TARGET_POINT)
            {
                Texture2D tx = gs.textures[scopeTextureIndex];
                spriteBatch.Draw(tx, new Rectangle((int)gs.firstFingerPosition.X, (int)gs.firstFingerPosition.Y, tx.Width, tx.Height), null, Color.White, timeManager.Rotation, new Vector2(tx.Width/2, tx.Height/2), SpriteEffects.None, 0);
            }

            if (timeManager.State == TimeState.CALIBRATION_PLASMA_GRANADE_PATH_CONTROL_POINT)
            {
                Texture2D tx = gs.textures[scopeTextureIndex];
                spriteBatch.Draw(tx, new Rectangle((int)gs.firstFingerPosition.X, (int)gs.firstFingerPosition.Y, tx.Width, tx.Height), null, Color.White, timeManager.Rotation, new Vector2(tx.Width / 2, tx.Height / 2), SpriteEffects.None, 0);
                spriteBatch.Draw(tx, new Rectangle((int)gs.secondFingerPosition.X, (int)gs.secondFingerPosition.Y, tx.Width, tx.Height), null, Color.White, timeManager.Rotation, new Vector2(tx.Width / 2, tx.Height / 2), SpriteEffects.None, 0);
                Vector2 point1 = timeManager.Path.Evaluate(0);
                Vector2 point2 = Vector2.Zero;
                for (float i = 0.1f; i <= 1; i = i + 0.1f)
                {
                    point2 = timeManager.Path.Evaluate(i);
                    lineRenderer.DrawLine(spriteBatch, point1, point2, 2, Color.Azure);
                    point1 = point2;
                }
            }

            if (gs.playerTime.continuum < 0)
            {
                spriteBatch.Draw(gs.textures[shadowIndex], new Rectangle(0, 0, Constants.SCREEN_WIDTH / 2, Constants.SCREEN_HEIGHT / 2), shadowSource, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                spriteBatch.Draw(gs.textures[shadowIndex], new Rectangle(Constants.SCREEN_WIDTH / 2, 0, Constants.SCREEN_WIDTH / 2, Constants.SCREEN_HEIGHT / 2), shadowSource, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                spriteBatch.Draw(gs.textures[shadowIndex], new Rectangle(Constants.SCREEN_WIDTH / 2, Constants.SCREEN_HEIGHT / 2, Constants.SCREEN_WIDTH / 2, Constants.SCREEN_HEIGHT / 2), shadowSource, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0);
                spriteBatch.Draw(gs.textures[shadowIndex], new Rectangle(0, Constants.SCREEN_HEIGHT / 2, Constants.SCREEN_WIDTH / 2, Constants.SCREEN_HEIGHT / 2), shadowSource, Color.White, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
            }

            if (gs.PlayerLife <= Utilities.Constants.PLAYER_LIFE_CRITICAL_VALUE)
            {
                float alpha =(1 - (((float)gs.PlayerLife / (float)Utilities.Constants.PLAYER_LIFE_CRITICAL_VALUE)));
                spriteBatch.Draw(gs.textures[bloodIndex], new Rectangle(0, 0, Constants.SCREEN_WIDTH / 2, Constants.SCREEN_HEIGHT / 2), bloodSource, new Color(255,255,255)*alpha, 0, Vector2.Zero, SpriteEffects.None, 0);
                spriteBatch.Draw(gs.textures[bloodIndex], new Rectangle(Constants.SCREEN_WIDTH / 2, 0, Constants.SCREEN_WIDTH / 2, Constants.SCREEN_HEIGHT / 2), bloodSource, new Color(255, 255, 255) * alpha, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                spriteBatch.Draw(gs.textures[bloodIndex], new Rectangle(Constants.SCREEN_WIDTH / 2, Constants.SCREEN_HEIGHT / 2, Constants.SCREEN_WIDTH / 2, Constants.SCREEN_HEIGHT / 2), bloodSource, new Color(255, 255, 255) * alpha, 0, Vector2.Zero, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0);
                spriteBatch.Draw(gs.textures[bloodIndex], new Rectangle(0, Constants.SCREEN_HEIGHT / 2, Constants.SCREEN_WIDTH / 2, Constants.SCREEN_HEIGHT / 2), bloodSource, new Color(255, 255, 255) * alpha, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
            }

            spriteBatch.Draw(UIrenderer.Texture, SilverlightRectangle, SilverlightRectangle, Color.White);
            spriteBatch.DrawString(debugFont, debugText + debugText2, new Vector2(0, 0), Color.White);

            //Fade iniziale del livello
            if (gs.levelTime.time < Constants.INITIAL_BLACK_DURATION + Constants.INITIAL_FADE_DURATION)
            {
                if (gs.levelTime.time > Constants.INITIAL_BLACK_DURATION)
                {
                    spriteBatch.DrawString(debugFont, levelManager.Title, new Vector2((Constants.SCREEN_WIDTH / 2) - debugFont.MeasureString(levelManager.Title).X / 2, Constants.SCREEN_HEIGHT / 2), Color.White * (1 - fadeInterpolationIndex));
                    spriteBatch.DrawString(debugFont, levelManager.Subtitle, new Vector2(Constants.SCREEN_WIDTH / 2 - debugFont.MeasureString(levelManager.Subtitle).X / 2, (Constants.SCREEN_HEIGHT / 2) + 20), Color.White * (1 - fadeInterpolationIndex));
                }
                else
                {
                    spriteBatch.DrawString(debugFont, levelManager.Title, new Vector2(Constants.SCREEN_WIDTH / 2 - debugFont.MeasureString(levelManager.Title).X / 2, Constants.SCREEN_HEIGHT / 2), Color.White);
                    spriteBatch.DrawString(debugFont, levelManager.Subtitle, new Vector2(Constants.SCREEN_WIDTH / 2 - debugFont.MeasureString(levelManager.Subtitle).X / 2, (Constants.SCREEN_HEIGHT / 2) + 20), Color.White);
                }
            }
            

            spriteBatch.End();
            counterDraw++;
            if (e.TotalTime.Milliseconds < lastMsDraw)
            {
                fpsDraw = counterDraw;
                counterDraw = 0;
            }
            lastMsDraw = e.TotalTime.Milliseconds;
            debugText2 = "\nDraw: " + fpsDraw.ToString();
            //debugText2 += "\nX=" + inputManager.AccelerometerReading.X + "\nY=" + inputManager.AccelerometerReading.Y + "\nZ=" + inputManager.AccelerometerReading.Z;
            //debugText2 += "\nXc=" + inputManager.AccelerometerReadingCorrected.X + "\nYc=" + inputManager.AccelerometerReadingCorrected.Y + "\nZc=" + inputManager.AccelerometerReadingCorrected.Z;
            //debugText2 += "\nXc=" + inputManager.accelerometerCurrentZero.X + "\nYz=" + inputManager.accelerometerCurrentZero.Y + "\nZz=" + inputManager.accelerometerCurrentZero.Z;
        }


        //###############################################################
        //  CONTROLLI SILVERLIGHT DI DEBUG
        //###############################################################
        private void buttonLastChance_Click(object sender, RoutedEventArgs e)
        {
            if (gs.granadeNumber > 0)
                timeManager.ActivatePlasmaGranadeLauncher();
        }

        /// <summary>
        /// Save button click handle 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameEndTextBox.Text;

            if (name.Length > Utilities.Constants.MAX_NAME_LENGTH)
                name = name.Substring(0, Utilities.Constants.MAX_NAME_LENGTH);

            if (name.Trim() == "")
                name = "Player";

            scores = Score.WriteScores(gs.playerTime.time, name);
            OnBackKeyPress(new System.ComponentModel.CancelEventArgs(false));
        }

        /// <summary>
        /// Cancel button click handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackKeyPress(new System.ComponentModel.CancelEventArgs(false));
        }
    }
}