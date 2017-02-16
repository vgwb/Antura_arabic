using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GP_RTM_Room {

	public string id = string.Empty;
	public string creatorId = string.Empty;
	public GP_RTM_RoomStatus status = GP_RTM_RoomStatus.ROOM_VARIANT_DEFAULT;
	public long creationTimestamp = 0;

	public List<GP_Participant> participants = new List<GP_Participant>();


	public GP_RTM_Room() {
		participants = new List<GP_Participant>();
	}

	public void AddParticipant(GP_Participant p) {
		participants.Add(p);
	}

	public GP_Participant GetParticipantById(string id) {
		foreach(GP_Participant p in participants) {
			if(p.id.Equals(id)) {
				return p;
			}
		}

		return null;
	}
}
