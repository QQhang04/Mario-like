using UnityEngine;

public class ToggleOnPressStrategy : IPanelUpdateStrategy
{
    private bool canToggle = true; // 避免持续触发

    public void UpdatePanel(Panel panel)
    {
        var center = panel.Collider.bounds.center;
        var contactOffset = Physics.defaultContactOffset + 0.1f;
        var size = panel.Collider.bounds.size + Vector3.up * contactOffset;
        var bounds = new Bounds(center, size);

        bool intersectsEntity = panel.EntityActivator && bounds.Intersects(panel.EntityActivator.bounds);
        bool intersectsOther = panel.OtherActivator && bounds.Intersects(panel.OtherActivator.bounds);

        bool isPressed = intersectsEntity || intersectsOther;

        if (isPressed && canToggle)
        {
            if (panel.activated)
                panel.Deactivate();
            else
                panel.Activate();

            canToggle = false; // 防止重复切换
        }
        else if (!isPressed)
        {
            canToggle = true; // 玩家离开后才能再次切换
        }

        if (!intersectsEntity) panel.EntityActivator = null;
        if (!intersectsOther) panel.OtherActivator = null;
    }
}