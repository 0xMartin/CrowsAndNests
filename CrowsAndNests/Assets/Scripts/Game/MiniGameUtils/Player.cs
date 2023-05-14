using Cinemachine;
using UnityEngine;

namespace Game.MiniGameUtils 
{

    /// <summary>
    /// struktura uchovavajici potrebne data o hraci
    /// </summary>
    public class Player
    {
        public string Name { get; set; } /** jmeno hrace */
        public float Score { get; set; } /** skore hrace */
        public int Lives { get; set; } /** pocet zivotu hrace (i hrac ktery ma 0 zivotu muze hrat v dalsi hre, pokud ma vsak zaporny pocet zivotu je jiz ze hry vyrazen) */
        public bool IsLiving { get; set; } /** pokud je "true" hrac je zivi, pokud "false" uz zemrel v dane minihre a jeho pohled kamery je nastaven na spectator mod */
        public GameObject ModelRef { get; set; } /** reference na model hrace */
        public CinemachineFreeLook CinemachineFreeLook { get; set; } /** reference na cinemachine kontroler kamery hrace (pokud jde o hrace ktery hraje z jineho PC bude NULL) */
    }

}