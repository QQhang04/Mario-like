using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("PLAYER TWO/Platformer Project/Misc/Panel")]
public class Panel : MonoBehaviour, IEntityContact
{
	public enum PanelMode
	{
		PressToActivate,
		ToggleOnPress
	}
	
	public PanelMode panelMode = PanelMode.PressToActivate;
	private IPanelUpdateStrategy updateStrategy;

	private void InitializeStrategy() => updateStrategy = PanelUpdateStrategyFactory.CreateStrategy(panelMode);

	public bool autoToggle;
	public bool requireStomp;
	public bool requirePlayer;
	public AudioClip activateClip;
	public AudioClip deactivateClip;


	public UnityEvent OnActivate;
	public UnityEvent OnDeactivate;

	protected Collider m_collider;
	protected Collider m_entityActivator;
	protected Collider m_otherActivator;
	
	public Collider Collider => m_collider;
	public Collider EntityActivator { get => m_entityActivator; set => m_entityActivator = value; }
	public Collider OtherActivator { get => m_otherActivator; set => m_otherActivator = value; }

	protected AudioSource m_audio;
	
	public bool activated { get; protected set; }
	
	public virtual void Activate()
	{
		Debug.Log("ACtivate");
		if (!activated)
		{
			if (activateClip)
			{
				m_audio.PlayOneShot(activateClip);
			}

			activated = true;
			OnActivate?.Invoke();
		}
	}
	
	public virtual void Deactivate()
	{
		Debug.Log("DeActivate");
		if (activated)
		{
			if (deactivateClip)
			{
				m_audio.PlayOneShot(deactivateClip);
			}

			activated = false;
			OnDeactivate?.Invoke();
		}
	}

	protected virtual void Start()
	{
		gameObject.tag = GameTag.Panel;
		m_collider = GetComponent<Collider>();
		m_audio = GetComponent<AudioSource>();

		InitializeStrategy();
	}

	protected virtual void Update()
	{	
		updateStrategy.UpdatePanel(this);
	}

	public void OnEntityContact(Entity entity)
	{
		if (entity.velocity.y <= 0 && entity.IsPointUnderStep(m_collider.bounds.max))
		{
			if ((!requirePlayer || entity is Player) &&
				(!requireStomp || (entity as Player).states.IsCurrentOfType(typeof(StompPlayerState))))
			{
				m_entityActivator = entity.controller;
			}
		}
	}

	protected virtual void OnCollisionStay(Collision collision)
	{
		if (!(requirePlayer || requireStomp) && !collision.collider.CompareTag(GameTag.Player))
		{
			m_otherActivator = collision.collider;
		}
	}
}