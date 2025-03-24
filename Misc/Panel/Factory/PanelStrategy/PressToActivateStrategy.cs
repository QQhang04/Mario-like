using UnityEngine;

public class PressToActivateStrategy : IPanelUpdateStrategy
{
    public void UpdatePanel(Panel panel)
    {
        var center = panel.Collider.bounds.center;
        var contactOffset = Physics.defaultContactOffset + 0.1f;
        var size = panel.Collider.bounds.size + Vector3.up * contactOffset;
        var bounds = new Bounds(center, size);

        bool intersectsEntity = panel.EntityActivator && bounds.Intersects(panel.EntityActivator.bounds);
        bool intersectsOther = panel.OtherActivator && bounds.Intersects(panel.OtherActivator.bounds);

        bool shouldActivate = intersectsEntity || intersectsOther;
        bool shouldDeactivate = !shouldActivate && panel.autoToggle;

        if (shouldActivate && !panel.activated)
        {
            panel.Activate();
        }
        else if (shouldDeactivate && panel.activated)
        {
            panel.Deactivate();
        }

        if (!intersectsEntity) panel.EntityActivator = null;
        if (!intersectsOther) panel.OtherActivator = null;
    }
}