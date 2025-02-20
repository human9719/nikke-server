﻿using nksrv.Utils;
using Swan.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nksrv.LobbyServer.Msgs.User
{
    [PacketPath("/user/scenario/exist")]
    public class GetUserScenarioExist : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            var req = await ReadData<ReqExistScenario>();

            // TODO: Check response from real server

            var response = new ResExistScenario();

            var user = GetUser();

            foreach (var item in req.ScenarioGroupIds)
            {
                Logger.Info("check scenario " + item);
                foreach (var completed in user.CompletedScenarios)
                {
                    // story thingy was completed
                    if (completed == item)
                    {
                        Logger.Info(item + " is completed");
                        response.ExistGroupIds.Add(item);
                    }
                }
            }

            await WriteDataAsync(response);
        }
    }
}
