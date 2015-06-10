using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp;
using SeeingSharp.Util;

namespace RKVideoMemory.Game
{
    [MessagePossibleSource(Constants.GAME_SCENE_NAME)]
    [MessageAsyncRoutingTargets(SeeingSharpConstants.THREAD_NAME_GUI)]
    public class GameEndReachedMessage : SeeingSharpMessage
    {
        public GameEndReachedMessage()
        {
        }
    }
}