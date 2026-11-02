using System;

namespace Codebound.Core
{
    public class MenuNavigator
    {
        public MenuScreen Current { get; private set; } = MenuScreen.Title;

        public event Action<MenuScreen> ScreenChanged;

        public void Show(MenuScreen screen)
        {
            if (screen == Current) return;
            Current = screen;
            ScreenChanged?.Invoke(screen);
        }

        public void HandleBack()
        {
            Show(Current == MenuScreen.Title ? MenuScreen.ConfirmQuit : MenuScreen.Title);
        }
    }
}
