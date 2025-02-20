﻿using nksrv.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nksrv.LobbyServer.Msgs.Ranking
{
    [PacketPath("/ranking/updateserverreward")]
    public class UpdateServerReward : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            var req = await ReadData<ReqUpdateRankingServerReward>();
            var response = new ResUpdateRankingServerReward();


          await  WriteDataAsync(response);
        }
    }
}
