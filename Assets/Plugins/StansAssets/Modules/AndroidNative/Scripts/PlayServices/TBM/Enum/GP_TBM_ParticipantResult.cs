using UnityEngine;
using System.Collections;

public enum GP_TBM_ParticipantResult  {
	MATCH_RESULT_UNINITIALIZED = -1,
	MATCH_RESULT_WIN = 0,
	MATCH_RESULT_LOSS = 1,
	MATCH_RESULT_TIE = 2,
	MATCH_RESULT_NONE = 3,
	MATCH_RESULT_DISCONNECT = 4,
	MATCH_RESULT_DISAGREED = 5
}