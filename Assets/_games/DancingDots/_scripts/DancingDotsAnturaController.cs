using UnityEngine;
using System.Collections;
using System;

namespace EA4S.DancingDots
{

	public enum AnturaContollerState
	{
		SLEEPING, WALKING, ROTATETING, BARKING, SNIFFING, ATLETTER, ATHOME, DANCING
	}

	public class DancingDotsAnturaController : MonoBehaviour
	{
		private bool movingToDestination = false; //When true Antura will move towards the setted destination
		private float movementSpeed = 10; //Movement speed
		private float rotationSpeed = 180; //Rotation speed by degree
		private AnturaAnimationController antura;
		private bool rotatingToTarget;
		private Vector3 destination;

		private Vector3 startPosition;
		private Transform targetToLookAt;

		public Transform homeLocation;
		public Transform letterLocation;

		public AnturaContollerState status;
	
		void Awake()
		{

			antura = gameObject.GetComponent<AnturaAnimationController>();
			startPosition = antura.gameObject.transform.position;

			antura.State = AnturaAnimationStates.sitting;
			antura.WalkingSpeed = 0; //walk-0, run-1
			status = AnturaContollerState.SLEEPING;

			AnturaNextDesicion();

		}

		private void MoveToNewDestination(Vector3 dest)
		{
			destination = dest;
			movingToDestination = true;
			status = AnturaContollerState.WALKING;
		}

		private void MoveTo(Vector3 dest)
		{
			Vector3 maxMovement = dest - gameObject.transform.position;
			Vector3 partialMovement = maxMovement.normalized * movementSpeed * Time.deltaTime;

			if (partialMovement.sqrMagnitude >= maxMovement.sqrMagnitude) //if we reached the destination
			{
				//position on the destination
				gameObject.transform.Translate(maxMovement, Space.World);

				movingToDestination = false;

				if (letterLocation.position == dest)
				{
					status = AnturaContollerState.ATLETTER;
				}
				else
				{
					status = AnturaContollerState.ATHOME;
				}

				AnturaNextDesicion();

			}
			else //make the progress for this frame
			{
				gameObject.transform.Translate(partialMovement, Space.World);
//				gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, Quaternion.LookRotation(maxMovement), rotationSpeed * Time.deltaTime);

			}
		}

		private void RotateTowards(Transform target)
		{
			Quaternion maxRotate = Quaternion.RotateTowards(gameObject.transform.rotation, Quaternion.LookRotation(target.position - transform.position), 180);
			Quaternion partialRotate = Quaternion.RotateTowards(gameObject.transform.rotation, Quaternion.LookRotation(target.position - transform.position), rotationSpeed*Time.deltaTime);
			Vector3 temp;
			float maxAngle=0;
			float partialAngle=0;

			maxRotate.ToAngleAxis(out maxAngle,out temp);
			partialRotate.ToAngleAxis(out partialAngle, out temp);

			if (partialAngle >= maxAngle) //if we reached the destination
			{
				//rotate on the destination
				gameObject.transform.rotation = maxRotate;

				rotatingToTarget = false;

				if (letterLocation == target)
				{
					status = AnturaContollerState.ATLETTER;
				}
				else
				{
					status = AnturaContollerState.ATHOME;
				}

				AnturaNextDesicion();


			}
			else //make the progress for this frame
			{
				//rotate
				gameObject.transform.rotation = partialRotate;

				//m_oAntura.SetAnimation(m_eAnimationOnMoving);


			}
		}

		private void AnturaNextDesicion()
		{
			switch (status) {
			case AnturaContollerState.ATLETTER:
				antura.State = AnturaAnimationStates.dancing;
				break;
			case AnturaContollerState.ATHOME:
				break;
			case AnturaContollerState.SNIFFING:
				break;
			case AnturaContollerState.BARKING:
				break;
			case AnturaContollerState.DANCING:
				break;
			case AnturaContollerState.SLEEPING:
				MoveToNewDestination(letterLocation.position);
				antura.State = AnturaAnimationStates.walking;
				status = AnturaContollerState.WALKING;
				break;
			default:
				break;
			}

		}

		void Update()
		{
			if (movingToDestination)
			{
				MoveTo(destination);
			}

			if(rotatingToTarget)
			{
				RotateTowards(targetToLookAt);
			}

//			if (m_eAnturaState == AnturaContollerState.BARKING)
//			{
//				m_fBarkTimeProgress += Time.deltaTime;
//				if (m_fBarkTimeProgress >= m_fBarkTime)
//				{
//					AnturaNextTransition();
//				}
//			}

		}

	}
}


