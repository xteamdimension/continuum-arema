using Microsoft.Xna.Framework;
using Continuum.Utilities;
using Continuum.State;

namespace Continuum.Weapons
{
    /// <summary>
    /// Interfaccia per le armi del gioco.
    /// </summary>
    public interface IWeapons
    {
        /// <summary>
        /// Il livello di potenziamento dell'arma corrente
        /// </summary>
        int Level { get; }

        /// <summary>
        /// Il massimo livello di potenziamento raggiungibile dall'arma corrente
        /// </summary>
        int MaxLevel { get; }

        /// <summary>
        /// Il minimo livello di potenziamento raggiungibile dall'arma corrente
        /// </summary>
        int MinLevel { get; }

        /// <summary>
        /// Indica se l'arma in questione è del giocatore o meno
        /// </summary>
        bool isPlayerWeapon { get; }

        /// <summary>
        /// Funzione di update di ogni arma. Lancerà i bullets quando deciso dalla specifica implementazione.
        /// </summary>
        /// <param name="gameTime">Oggetto gameTime del gioco per eventuali calcoli temporizzati di update dell'arma</param>
        /// <param name="position">Posizione del proprietario di quest'arma</param>
        void Update(Vector2 position);

        /// <summary>
        /// Potenzia l'arma
        /// Restituisce se l'aggiornamento è andato a buon fine. In caso contrario eliminare l'arma dalla lista. (Arma base GUN restituisce sempre true, non è eliminabile)
        /// </summary>
        /// <param name="numOfLevels">Il numero di livelli di cui va potenziata l'arma</param>
        bool Upgrade(int numOfLevels);

        /// <summary>
        /// Restituisce il danno effettuato dall'arma nel livello di potenziamento specificato
        /// </summary>
        /// <param name="level">Il livello di potenziamento</param>
        /// <returns></returns>
        int GetDamage(int level);

        /// <summary>
        /// Restituisce la velocità dei proiettili nel livello di potenziamento specificato
        /// </summary>
        /// <param name="level">Il livello di potenziamento</param>
        /// <returns></returns>
        int GetSpeed(int level);

        /// <summary>
        /// Restituisce l'intervallo di tempo (in secondi) che intercorre tra il lancio di un proiettile e di quello successivo.
        /// </summary>
        /// <param name="level">Il livello di potenziamento</param>
        /// <returns></returns>
        float GetTimeForShooting(int level);

        /// <summary>
        /// Restituisce la durata del livello corrente dell'arma. Una volta scaduto questo tempo, l'arma subisce un downgrade 
        /// </summary>
        /// <param name="level">Il livello di potenziamento</param>
        /// <returns></returns>
        float GetLevelDuration(int level);
    }
}
