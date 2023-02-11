using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameGlobal
{

    public static class Scene {
        public static readonly int MAIN_MENU = 0;
        public static readonly int ARENA_MENU = 1;
        public static readonly int MULTIPLAYER_MENU = 2;
        public static readonly int SETTINGS_MENU = 3;

        public static readonly int ARENA = 4;
        public static readonly int GAME_OVEW = 5;
    }

    public static class Util {

        public static string buildMessage(System.Type type, string message) {
            return "[" + type.Name + "] >> " + message;
        }

    }

}
