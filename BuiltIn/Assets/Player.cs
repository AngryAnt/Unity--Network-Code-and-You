using UnityEngine;
using System.Collections;


[RequireComponent (typeof (NetworkView))]
[RequireComponent (typeof (Rigidbody))]
public class Player : MonoBehaviour
{
	const float kInterpolationSpeed = 100.0f;


	enum MoveDirection
	{
		West,
		Stop,
		East,

		NorthWest,
		North,
		NorthEast,

		SouthWest,
		South,
		SouthEast
	};


	NetworkPlayer owner;
	bool isMine = false;
	MoveDirection direction = MoveDirection.Stop;
	Vector3 targetPosition, targetVelocity;
	Quaternion targetRotation;
	float speed = 10.0f;


	public NetworkPlayer Owner
	{
		get
		{
			return owner;
		}
	}


	void Awake ()
	{
		targetPosition = transform.position;
		targetRotation = transform.rotation;
		targetVelocity = Vector3.zero;
	}


	public void Spawn (NetworkPlayer player)
	{
		owner = player;
		networkView.RPC ("OnSpawn", player);
	}


	[RPC]
	void OnSpawn ()
	{
		isMine = true;
		Transform cameraTransform = Camera.main.transform.root;
		cameraTransform.parent = transform;
		cameraTransform.localPosition = Vector3.zero;
		cameraTransform.localRotation = Quaternion.identity;
	}


	[RPC]
	void OnNavigate (int newDirection)
	{
		direction = (MoveDirection)newDirection;
	}


	void Update ()
	{
		if (isMine)
		{
			MoveDirection newDirection =
				(Input.GetAxis ("Vertical") > 0) ? MoveDirection.North :
					(Input.GetAxis ("Vertical") < 0) ? MoveDirection.South :
						MoveDirection.Stop;

			if (Input.GetAxis ("Horizontal") > 0)
			{
				newDirection++;
			}
			else if (Input.GetAxis ("Horizontal") < 0)
			{
				newDirection--;
			}

			if (newDirection != direction)
			{
				networkView.RPC ("OnNavigate", RPCMode.Server, (int)newDirection);
			}

			direction = newDirection;
		}

		if (!Network.isServer)
		{
			PredictPosition (Time.deltaTime);

			rigidbody.position = Vector3.Lerp (rigidbody.position, targetPosition, Time.deltaTime * kInterpolationSpeed);
			rigidbody.rotation = Quaternion.Slerp (rigidbody.rotation, targetRotation, Time.deltaTime * kInterpolationSpeed);
		}
		else
		{
			targetPosition = rigidbody.position;
			targetRotation = rigidbody.rotation;
		}
	}


	void FixedUpdate ()
	{
		if (Network.isServer)
		{
			switch (direction)
			{
				case MoveDirection.West:
					targetVelocity = transform.right * -1.0f;
				break;
				case MoveDirection.Stop:
					targetVelocity = Vector3.zero;
				break;
				case MoveDirection.East:
					targetVelocity = transform.right;
				break;
				case MoveDirection.NorthWest:
					targetVelocity = (transform.forward + transform.right * -1.0f).normalized;
				break;
				case MoveDirection.North:
					targetVelocity = transform.forward;
				break;
				case MoveDirection.NorthEast:
					targetVelocity = (transform.forward + transform.right).normalized;
				break;
				case MoveDirection.SouthWest:
					targetVelocity = (transform.forward * -1.0f + transform.right * -1.0f).normalized;
				break;
				case MoveDirection.South:
					targetVelocity = transform.forward * -1.0f;
				break;
				case MoveDirection.SouthEast:
					targetVelocity = (transform.forward * -1.0f + transform.right).normalized;
				break;
			}

			targetVelocity *= speed;
		}

		rigidbody.AddForce (targetVelocity - rigidbody.velocity, ForceMode.VelocityChange);
	}


	void PredictPosition (float time)
	{
		targetPosition += (targetVelocity - targetVelocity.normalized * rigidbody.drag) * time;
	}


	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		stream.Serialize (ref targetPosition);
		stream.Serialize (ref targetRotation);
		stream.Serialize (ref targetVelocity);

		PredictPosition ((float)(Network.time - info.timestamp));
	}
}
