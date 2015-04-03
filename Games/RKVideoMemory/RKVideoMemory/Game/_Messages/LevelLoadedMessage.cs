using FrozenSky;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Game
{
    [MessagePossibleSource(Constants.GAME_SCENE_NAME)]
    [MessageAsyncRoutingTargets(FrozenSkyConstants.THREAD_NAME_GUI)]
    public class LevelLoadedMessage : FrozenSkyMessage
    {
        
    }
}
