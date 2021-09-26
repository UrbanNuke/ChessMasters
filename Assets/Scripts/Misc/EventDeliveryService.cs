﻿using System;

namespace Misc
{
    public class EventDeliveryService
    {
        public event Action OnUICameraButtonClicked;
        public void UICameraButtonClicked() => OnUICameraButtonClicked?.Invoke();
        public event Action<GameMode> OnUIGameStart;
        public void UIGameStart(GameMode mode) => OnUIGameStart?.Invoke(mode);
        public event Action<bool> OnUICameraButtonDisabled;
        public void UICameraButtonEnabled(bool state) => OnUICameraButtonDisabled?.Invoke(state);
        
        public event Action OnPlayerCheck;
        public void PlayerCheck() => OnPlayerCheck?.Invoke();
        public event Action OnPlayerCheckmate;
        public void PlayerCheckmate() => OnPlayerCheckmate?.Invoke();

        public event Action OnSwitchPlayerSide;
        public void SwitchPlayerSide() => OnSwitchPlayerSide?.Invoke();
    }
}