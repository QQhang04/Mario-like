public static class PanelUpdateStrategyFactory
{
    public static IPanelUpdateStrategy CreateStrategy(Panel.PanelMode mode)
    {
        switch (mode)
        {
            case Panel.PanelMode.PressToActivate:
                return new PressToActivateStrategy();
            case Panel.PanelMode.ToggleOnPress:
                return new ToggleOnPressStrategy();
            default:
                throw new System.ArgumentException("未知的Panel模式");
        }
    }
}