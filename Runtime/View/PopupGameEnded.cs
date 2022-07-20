using AsyncUtils;
using Match3.Core;

namespace Match3.View
{
    public abstract class PopupGameEnded : AsyncPopupInitializable<(GameController, bool)>
    {
    }
}