using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Continuum.Management
{
    /// <summary>
    /// Gestisce i suoni del gioco
    /// </summary>
    public static class SoundManager
    {
        /// <summary>
        /// I sound effects
        /// </summary>
        private static Dictionary<string, SoundEffect> soundEffects;

        /// <summary>
        /// Le istanze dei sound effects (quelle che vengono riprodotte)
        /// </summary>
        private static Dictionary<string, List<SoundEffectInstance>> soundInstances;

        /// <summary>
        /// Il content manager
        /// </summary>
        private static ContentManager content;

        /// <summary>
        /// Il settaggio del gioco
        /// </summary>
        //private static GameSetting setting;

        /// <summary>
        /// Inizializza il SoundManager
        /// </summary>
        /// <param name="contentManager">Il content manager che serve per caricare i suoni</param>
        public static void Initialize(ContentManager contentManager/*, GameSetting gameSetting */)
        {
            soundInstances = new Dictionary<string, List<SoundEffectInstance>>();
            soundEffects = new Dictionary<string, SoundEffect>();
            content = contentManager;
            //setting = gameSetting;
        }

        /// <summary>
        /// Carica un suono da file
        /// </summary>
        /// <param name="soundName">Il nome dell'asset</param>
        public static void LoadSound(string soundName)
        {
            soundEffects.Add(soundName, content.Load<SoundEffect>(soundName));
            soundInstances.Add(soundName, new List<SoundEffectInstance>());
        }

        /// <summary>
        /// Riproduce un suono
        /// </summary>
        /// <param name="soundName">Il nome del suono</param>
        /// <param name="isLooped">Indica se il suono va eseguito a ripetizione</param>
        public static void PlaySound(string soundName, float volume, bool isLooped, bool isMusic)
        {
            //if (setting.SoundEffects && !isMusic || setting.Music && isMusic)
            //{
                List<SoundEffectInstance> soundList = soundInstances[soundName];
                SoundEffectInstance sound = null;
                foreach (SoundEffectInstance s in soundList)
                {
                    if (s.State == SoundState.Stopped)
                    {
                        sound = s;
                        break;
                    }
                }
                if (sound == null)
                {
                    sound = soundEffects[soundName].CreateInstance();
                    soundList.Add(sound);
                    sound.IsLooped = isLooped;
                }
                sound.Volume = volume;
                sound.Play();
            //}
        }

        /// <summary>
        /// Riproduce un suono
        /// </summary>
        /// <param name="soundName">Il nome del suono</param>
        /// <param name="volume">Il volume del suono</param>
        public static void PlaySound(string soundName, float volume)
        {
            PlaySound(soundName, volume, false, false);
        }

        /// <summary>
        /// Riproduce un suono
        /// </summary>
        /// <param name="soundName">Il nome del suono</param>
        public static void PlaySound(string soundName)
        {
            PlaySound(soundName, 1, false, false);
        }

        /// <summary>
        /// termina la riproduzione di un suono
        /// </summary>
        /// <param name="soundName">Il nome del suono</param>
        /// <param name="stopAllInstances">Se true, stoppa tutte le istanze del suono selezionato</param>
        public static void StopSound(string soundName, bool stopAllInstances)
        {
            foreach (SoundEffectInstance s in soundInstances[soundName])
            {
                s.Stop();
                if (!stopAllInstances)
                    break;
            }
        }

        /// <summary>
        /// termina la riproduzione di un suono
        /// </summary>
        /// <param name="soundName">Il nome del suono</param>
        public static void StopSound(string soundName)
        {
            StopSound(soundName, false);
        }

        /// <summary>
        /// Termina la riproduzione di tutti i suoni
        /// </summary>
        public static void StopAllSounds()
        {
            foreach (KeyValuePair<string, List<SoundEffectInstance>> kvp in soundInstances)
            {
                foreach (SoundEffectInstance s in kvp.Value)
                {
                    s.Stop();
                }
            }
        }
    }
}
