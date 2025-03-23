using UnityEngine;

public class WaypointEnemyState : EnemyState
{
	protected override void OnEnter(Enemy enemy) { }

	protected override void OnExit(Enemy enemy) { }

	protected override void OnStep(Enemy enemy)
	{
		enemy.Gravity();
		enemy.SnapToGround();

		var destination = enemy.waypoints.current.position;
		destination = new Vector3(destination.x, enemy.position.y, destination.z);
		var head = destination - enemy.position;
		var distance = head.magnitude;
		var direction = head / distance;

		if (distance <= enemy.stats.current.waypointMinDistance)
		{
			enemy.Decelerate();
			enemy.waypoints.Next();
		}
		else
		{
			enemy.Accelerate(direction, enemy.stats.current.waypointAcceleration, enemy.stats.current.waypointTopSpeed);

			if (enemy.stats.current.faceWaypoint)
			{
				enemy.FaceDirectionSmooth(direction);
			}
		}
	}

	public override void OnContact(Enemy enemy, Collider other) { }
}