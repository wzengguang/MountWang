using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace Wang
{
    public class CustomOptionsVM : OptionsVM
    {


        public CustomOptionsVM(bool autoHandleClose, bool openedFromMultiplayer, Action<GameKeyOptionVM> onKeybindRequest, Action onBrightnessExecute = null)
            : base(autoHandleClose, openedFromMultiplayer, onKeybindRequest, onBrightnessExecute)
        {


        }

        public CustomOptionsVM(bool openedFromMultiplayer, Action onClose, Action<GameKeyOptionVM> onKeybindRequest, Action onBrightnessExecute = null)
            : base(openedFromMultiplayer, onClose, onKeybindRequest, onBrightnessExecute)
        {
        }
    }
}
