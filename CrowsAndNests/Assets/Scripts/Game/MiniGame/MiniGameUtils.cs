using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace MiniGameUtils
{

    // rozhrani pro minihru
    interface MiniGame
    {
        public string getName();
        public void initGame(MiniGameContext cntx);
        public void updateGame();
        public void runGame();
        public void stopGame();
        public void endGame();
    }

    // struktura uchovavajici potrebne data o hraci
    public struct Player
    {
        public string Name { get; set; } /** jmeno hrace */
        // skore hrace
        public int Score { get; set; }
        // pocet zivotu hrace
        public int Lives { get; set; }
        // pokud je "true" hrac je zivi, pokud "false" uz zemrel v dane minihre a jeho pohled kamery je nastaven na spectator mod
        public bool IsLiving { get; set; }
        // reference na model hrace
        public GameObject ModelRef { get; set; }
        // reference na cinemachine kontroler kamery hrace (pokud jde o hrace ktery hraje z jineho PC bude NULL)
        public CinemachineFreeLook cinemachineFreeLook { get; set; }
    }

    // Kontext mini hry, obsahuje vsechny data potrebne pro chod minihry v arene
    public class MiniGameContext
    {
        // pole vsech hnizd areny
        public GameObject[] Nests { get; private set; }
        // pole vsech spawnu areny
        public Transform[] Spawns { get; private set; }
        // list z hraci
        public List<Player> Players { get; set; }

        // list obsahuje indexy jiz pouzitych spawnu  
        private List<int> usedSpawns;

        public MiniGameContext(GameObject[] nests, Transform[] spawns)
        {
            this.Nests = nests;
            this.Spawns = spawns;
            this.Players = new List<Player>();
            this.usedSpawns = new List<int>();
            Debug.Log(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Context created. Nest count: " +
                this.Nests.Length.ToString() + ", Spawns: " + this.Spawns.Length.ToString()));
        }

        public void randomSpawnPlayer(Player player)
        {
            if (player.ModelRef == null || usedSpawns.Count >= Spawns.Length)
            {
                // selhalo
                setPlayerCameraToObject(player, null); //>>>>>>>>>>>>>>>>>>>>>>>>>>> null -> spectator
                return;
            }

            // nahodne vybere spawn na kterem se jeste nenachazi zadny hrac
            int rnd = -1;
            int cnt = 50;
            do
            {
                rnd = Random.Range(0, Spawns.Length);
                cnt++;
                if (cnt <= 0)
                {
                    // selhalo
                    setPlayerCameraToObject(player, null); //>>>>>>>>>>>>>>>>>>>>>>>>>>> null -> spectator
                    return;
                }
            } while (usedSpawns.Contains(rnd));
            usedSpawns.Add(rnd);

            // nastavi mu pozici spawnu
            player.ModelRef.transform.position = Spawns[rnd].transform.position;
            player.ModelRef.transform.rotation = Spawns[rnd].transform.rotation;

            // aktivuje hraci model ve hre
            player.ModelRef.SetActive(true);

            // nastavi kameru hrace na sledovani jeho modelu
            setPlayerCameraToObject(player, player.ModelRef);

            // log
            Vector3 pos = Spawns[rnd].transform.position;
            Debug.Log(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Player [" + player.Name + "] spawned on: " + pos.x + ", " + pos.y + ", " + pos.z));
        }

        public void killPlayer(Player player)
        {
            // snizi hraci pocet zivotu, pokud jiz nema zivoty konci ve hre a muze se jen divat
            if (player.Lives == 0)
            {
                return;
            }
            player.Lives = Mathf.Max(0, player.Lives - 1);

            // deaktivuje hraci jeho model ze hry
            if (player.ModelRef != null)
            {
                player.ModelRef.SetActive(false);
            }

            // po uplinuti kratekho casu mu nastavy pohled na spectator kameru
            setPlayerCameraToObject(player, null); //>>>>>>>>>>>>>>>>>>>>>>>>>>> null -> spectator

            // log
            Debug.Log(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Player [" + player.Name + "] killed"));
        }

        public void addPlayerLives(Player player, int lives)
        {
            // prida hraci definovany pocet zivotu
            player.Lives += lives;

            // log
            Debug.Log(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Player [" + player.Name + "] added " + lives.ToString() + " lives"));
        }

        public void hideNest(int id)
        {
            if(id < 0 || id >= Nests.Length) {
                return;
            }

            // deaktivuje/skryje hnizdo
            GameObject nest = Nests[id];
            nest.SetActive(false);

            // zobrazi efekt skryti hnizda
            //TODO>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>TODO

            // log
            Debug.Log(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Nest [" + id + "] is now deactivated"));
        }

        public void showNest(int id)
        {
            if(id < 0 || id >= Nests.Length) {
                return;
            }

            // znovu aktivuje hnizdo
            GameObject nest = Nests[id];
            nest.SetActive(true);

            // zobrazi efekt zobrazeni hnizda
            //TODO>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>TODO

            // log
            Debug.Log(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Nest [" + id + "] is now active"));
        }

        public void resetArena()
        {
            // aktivuje vsechny hnizda
            foreach (GameObject nest in this.Nests)
            {
                nest.SetActive(true);
            }

            // deaktivuje modely vsech hracu
            foreach (Player p in this.Players)
            {
                if (p.ModelRef != null)
                {
                    p.ModelRef.SetActive(false);
                }
            }

            // uvoleni vsech spawnu
            usedSpawns.Clear();

            // log
            Debug.Log(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Arene reset done"));
        }

        public void setPlayerCameraToObject(Player player, GameObject obj)
        {
            if (player.cinemachineFreeLook == null)
            {
                Debug.LogError(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Failed to set player camer look on objects. Camera is null."));
                return;
            }
            // nastavi pohled kamery hrace na zvoleny objekt
            //TODO>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>TODO
        }
    }

}