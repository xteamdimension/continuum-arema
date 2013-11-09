using System;
using System.Collections.Generic;
using Continuum.Utilities;
using Continuum.State;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Continuum.Management
{
    public class LevelManager
    {
        //Variabili private
        private GameState gs;
        private LevelReader myReader;
        private LevelElement element;
        private Dictionary<string, DynamicNormalRandomVariable> randomVariablesDictionary;

        /// <summary>
        /// Titolo del livello corrente
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// Sottotitolo del livello corrente
        /// </summary>
        public string Subtitle { get; private set; }
        /// <summary>
        /// Durata del livello corrente espressa in secondi
        /// </summary>
        public int Duration { get; private set; }
        /// <summary>
        /// Il numero di layer di background disponibili per questo livello
        /// </summary>
        public int NumberOfBackgroundLevels { get; private set; }

        public bool isLevelFinished { get; private set; }

        public LevelManager(GameState gs, string levelFile)
        {
            this.gs = gs;
            myReader = new LevelReader("Levels/" + levelFile);

            element = myReader.Current;
            Title = element.Attribute("title");
            Subtitle = element.Attribute("subtitle");
            Duration = Convert.ToInt32(element.Attribute("duration"));
            NumberOfBackgroundLevels = Convert.ToInt32(element.Attribute("numlevels"));
            isLevelFinished = false;
            myReader.MoveNext();
        }

        public void getTextures(ContentManager cm)
        {
            element = myReader.Current;
            if (element.Name == "texturesDeclaration")
            {
                myReader.MoveNext();
                element = myReader.Current;
            }
            else return;
            
            List<Texture2D> tmpList = new List<Texture2D>();
            int textureindex = 0;
            while (element.Name == "texture")
            {
                tmpList.Add(cm.Load<Texture2D>(element.Attribute("path")));
                gs.textureIndices.Add(textureindex, element.Attribute("id"));
                textureindex++;
                myReader.MoveNext();
                element = myReader.Current;
            }

            gs.textures = tmpList.ToArray();

            gs.SetPlayerBounds();
        }

        public void getRandomVariables()
        {
            element = myReader.Current;
            if(element.Name == "randomVariablesDeclaration"){
                myReader.MoveNext();
                element = myReader.Current;
            }
            else return;
            
            randomVariablesDictionary = new Dictionary<string,DynamicNormalRandomVariable>();

            while (element.Name == "randomVariable")
            {
                string id = element.Attribute("id");
                float mean = Utility.StringToFloat(element.Attribute("mean"));
                float standardDeviation = Utility.StringToFloat(element.Attribute("standardDeviation"));
                float? meanIncrementPerMinute = element.Attribute("meanIncrementPerMinute") == null ? null : (float?)Utility.StringToFloat(element.Attribute("meanIncrementPerMinute"));
                float? maxValue = element.Attribute("maxValue") == null ? null : (float?)Utility.StringToFloat(element.Attribute("maxValue"));
                float? minValue = element.Attribute("minValue") == null ? null : (float?)Utility.StringToFloat(element.Attribute("minValue"));
                randomVariablesDictionary.Add(id, new DynamicNormalRandomVariable(mean, standardDeviation, meanIncrementPerMinute, maxValue, minValue));
                myReader.MoveNext();
                element = myReader.Current;
            }
        }

        public void goToStartLevel()
        {
            element = myReader.Current;
            if (element.Name == "startlevel")
            {
                myReader.MoveNext();
                element = myReader.Current;
            }
            else
            {
                throw new Exception("Errore di sintassi nel file di livello");
            }
        }

        public void Update()
        {
            if (gs.levelTime.continuum > 0)
            {
                if(!isLevelFinished) {
                    element = myReader.Current;
                    while (!isLevelFinished && (element.Attribute("timestamp") == null || Convert.ToInt32(element.Attribute("timestamp")) <= gs.levelTime.time))
                    {
                        switch (element.Name)
                        {
                            case "backgroundTexture":
                                gs.newBackgroundTexture(Convert.ToInt32(element.Attribute("level")), Convert.ToInt32(element.Attribute("speed")), element.Attribute("texture"), element.Attribute("transitionTexture"));
                                break;
                            case "asteroid":
                                gs.newAsteroid(Convert.ToInt32(element.Attribute("xposition")), Convert.ToInt32(element.Attribute("speed")), Convert.ToInt32(element.Attribute("life")), element.Attribute("texture"));
                                break;
                            case "endlevel":
                                isLevelFinished = true;
                                break;
                            case "animation":
                                gs.newAnimation(new Vector2(Convert.ToInt32(element.Attribute("x")), Convert.ToInt32(element.Attribute("y"))), element.Attribute("texture"), Convert.ToInt32(element.Attribute("frames")), Convert.ToInt32(element.Attribute("rows")), Convert.ToInt32(element.Attribute("cols")), Convert.ToInt32(element.Attribute("fps")), Utility.StringToFloat(element.Attribute("rotation")), Utility.StringToFloat(element.Attribute("rotationspeed")));
                                break;
                            case "enemy":
                                string powerUpTypeString = element.Attribute("powerup");
                                PowerUpType powerUpType;
                                switch(powerUpTypeString){
                                    case "Gun":
                                        powerUpType = PowerUpType.GUN;
                                        break;
                                    case "Rocket":
                                        powerUpType = PowerUpType.ROCKET;
                                        break;
                                    case null:
                                        powerUpType = PowerUpType.NONE;
                                        break;
                                    default:
                                        throw new InvalidCastException("Argomento non valido");
                                }
                                gs.newEnemy(new Vector2(Convert.ToInt32(element.Attribute("x")), Convert.ToInt32(element.Attribute("y"))), Utility.StringToFloat(element.Attribute("speed")), element.Attribute("texture"), element.Attribute("weapon"), Convert.ToInt32(element.Attribute("life")), powerUpType);
                                break;
                            case "tachyonStream":
                                gs.newTachyonStream(Convert.ToInt32(element.Attribute("xposition")), Utility.StringToFloat(element.Attribute("duration")), element.Attribute("texture")); 
                                break;
                            case "asteroidRandomizer":
                                float probabilityA = Utility.StringToFloat(element.Attribute("launchProbabilityPerSecond"));
                                float? probabilityIncrementPerMinuteA = element.Attribute("probabilityIncrementPerMinute") == null ? null : (float?)Utility.StringToFloat(element.Attribute("probabilityIncrementPerMinute"));
                                float? probabilityMaxA = element.Attribute("probabilityMax") == null ? null : (float?)Utility.StringToFloat(element.Attribute("probabilityMax"));
                                DynamicNormalRandomVariable speedRVA;
                                DynamicNormalRandomVariable lifeRVA;
                                randomVariablesDictionary.TryGetValue(element.Attribute("speedRandomVariable"), out speedRVA);
                                randomVariablesDictionary.TryGetValue(element.Attribute("lifeRandomVariable"), out lifeRVA);
                                gs.newAsteroidRandomizer(probabilityA, probabilityIncrementPerMinuteA, probabilityMaxA, speedRVA, lifeRVA, element.Attribute("texture"));
                                break;
                            case "enemyRandomizer":
                                float probabilityE = Utility.StringToFloat(element.Attribute("launchProbabilityPerSecond"));
                                float? probabilityIncrementPerMinuteE = element.Attribute("probabilityIncrementPerMinute") == null ? null : (float?)Utility.StringToFloat(element.Attribute("probabilityIncrementPerMinute"));
                                float? probabilityMaxE = element.Attribute("probabilityMax") == null ? null : (float?)Utility.StringToFloat(element.Attribute("probabilityMax"));
                                float? powerUpProbabilityPerLaunch = element.Attribute("powerUpProbabilityPerLaunch") == null ? null : (float?)Utility.StringToFloat(element.Attribute("powerUpProbabilityPerLaunch"));
                                float? rocketPowerUpProbability = element.Attribute("rocketPowerUpProbability") == null ? null : (float?)Utility.StringToFloat(element.Attribute("rocketPowerUpProbability"));
                                float? granadePowerUpProbability = element.Attribute("granadePowerUpProbability") == null ? null : (float?)Utility.StringToFloat(element.Attribute("granadePowerUpProbability"));
                                DynamicNormalRandomVariable speedRVE;
                                DynamicNormalRandomVariable lifeRVE;
                                randomVariablesDictionary.TryGetValue(element.Attribute("speedRandomVariable"), out speedRVE);
                                randomVariablesDictionary.TryGetValue(element.Attribute("lifeRandomVariable"), out lifeRVE);
                                gs.newEnemyRandomizer(probabilityE, probabilityIncrementPerMinuteE, probabilityMaxE, powerUpProbabilityPerLaunch, rocketPowerUpProbability, granadePowerUpProbability, speedRVE, lifeRVE, element.Attribute("weapon"), element.Attribute("texture"));
                                break;
                            case "tachyonStreamRandomizer":
                                float probabilityT = Utility.StringToFloat(element.Attribute("launchProbabilityPerSecond"));
                                float? probabilityIncrementPerMinuteT = element.Attribute("probabilityIncrementPerMinute") == null ? null : (float?)Utility.StringToFloat(element.Attribute("probabilityIncrementPerMinute"));
                                float? probabilityMaxT = element.Attribute("probabilityMax") == null ? null : (float?)Utility.StringToFloat(element.Attribute("probabilityMax"));
                                DynamicNormalRandomVariable durationRVT;
                                randomVariablesDictionary.TryGetValue(element.Attribute("durationRandomVariable"), out durationRVT);
                                gs.newTachyonStreamRandomizer(probabilityT, probabilityIncrementPerMinuteT, probabilityMaxT, durationRVT, element.Attribute("texture"));
                                break;
                        }
                        if (!isLevelFinished)
                        {
                            myReader.MoveNext();
                            element = myReader.Current;
                        }
                    }
                }
            }
            else
            {
                while (myReader.Previous.Attribute("timestamp") != null && Convert.ToInt32(myReader.Previous.Attribute("timestamp")) >= gs.levelTime.time)
                {
                    if (isLevelFinished)
                        isLevelFinished = false;
                    myReader.MovePrevious();
                }
            }
        }
    }
}
