﻿using nksrv.Utils;

namespace nksrv.LobbyServer.Msgs.System
{
    [PacketPath("/system/checkversion")]
    public class CheckClientVersion : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            var req = await ReadData<CheckVersionRequest>();
            var response = new CheckVersionResponse();
            response.Availability = 0;    // None = 0, Available = 1, Mandatory = 2

          await  WriteDataAsync(response);
        }
    }
}
