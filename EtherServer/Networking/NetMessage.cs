﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EtherServer.Networking
{
    public enum NetMessage
    {
        Nickname = 1,
        GetPlayers = 2,
        PlayerConnected = 3,
        PlayerDisconnected = 4,
        UpdatePosition = 5,
        SpawnEnemy = 6,
        GetEnemies = 7,
        UpdateEnemyPosition = 8
    }
}
