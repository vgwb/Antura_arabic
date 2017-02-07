using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Contains GUI system dependent functions

public class WMG_Events : WMG_GUI_Functions {

	private void AddEventTrigger(UnityAction<GameObject> action, EventTriggerType triggerType, GameObject go)
	{
		EventTrigger eventTrigger = go.GetComponent<EventTrigger>();
		if (eventTrigger == null) {
			eventTrigger = go.AddComponent<EventTrigger>();
			eventTrigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
		}
		// Create a nee TriggerEvent and add a listener
		EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
		trigger.AddListener((eventData) => action(go)); // capture and pass the event data to the listener
		// Create and initialise EventTrigger.Entry using the created TriggerEvent
		EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };
		// Add the EventTrigger.Entry to delegates list on the EventTrigger
		eventTrigger.triggers.Add(entry);
	}

	private void AddEventTrigger(UnityAction<GameObject, bool> action, EventTriggerType triggerType, GameObject go, bool state)
	{
		EventTrigger eventTrigger = go.GetComponent<EventTrigger>();
		if (eventTrigger == null) {
			eventTrigger = go.AddComponent<EventTrigger>();
			eventTrigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
		}
		// Create a nee TriggerEvent and add a listener
		EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
		trigger.AddListener((eventData) => action(go, state)); // capture and pass the event data to the listener
		// Create and initialise EventTrigger.Entry using the created TriggerEvent
		EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };
		// Add the EventTrigger.Entry to delegates list on the EventTrigger
		eventTrigger.triggers.Add(entry);
	}

	#region GraphClickEvents

	// Click events
	// Series Node
	public delegate void WMG_Click_H(WMG_Series aSeries, WMG_Node aNode);
	public event WMG_Click_H WMG_Click;
	
	public void addNodeClickEvent(GameObject go) {
		AddEventTrigger(WMG_Click_2, EventTriggerType.PointerClick, go);
	}
	
	private void WMG_Click_2(GameObject go) {
		if (WMG_Click != null) {
			WMG_Series aSeries = go.transform.parent.parent.GetComponent<WMG_Series>();
			WMG_Click(aSeries, go.GetComponent<WMG_Node>());
		}
	}
	
	// Series Link
	public delegate void WMG_Link_Click_H(WMG_Series aSeries, WMG_Link aLink);
	public event WMG_Link_Click_H WMG_Link_Click;
	
	public void addLinkClickEvent(GameObject go) {
		AddEventTrigger(WMG_Link_Click_2, EventTriggerType.PointerClick, go);
	}
	
	private void WMG_Link_Click_2(GameObject go) {
		if (WMG_Link_Click != null) {
			WMG_Series aSeries = go.transform.parent.parent.GetComponent<WMG_Series>();
			WMG_Link_Click(aSeries, go.GetComponent<WMG_Link>());
		}
	}
	
	// Series Legend Node
	public delegate void WMG_Click_Leg_H(WMG_Series aSeries, WMG_Node aNode);
	public event WMG_Click_Leg_H WMG_Click_Leg;
	
	public void addNodeClickEvent_Leg(GameObject go) {
		AddEventTrigger(WMG_Click_Leg_2, EventTriggerType.PointerClick, go);
	}
	
	private void WMG_Click_Leg_2(GameObject go) {
		if (WMG_Click_Leg != null) {
			WMG_Series aSeries = go.transform.parent.GetComponent<WMG_Legend_Entry>().seriesRef;
			WMG_Click_Leg(aSeries, go.GetComponent<WMG_Node>());
		}
	}
	
	// Series Legend Link
	public delegate void WMG_Link_Click_Leg_H(WMG_Series aSeries, WMG_Link aLink);
	public event WMG_Link_Click_Leg_H WMG_Link_Click_Leg;
	
	public void addLinkClickEvent_Leg(GameObject go) {
		AddEventTrigger(WMG_Link_Click_Leg_2, EventTriggerType.PointerClick, go);
	}
	
	private void WMG_Link_Click_Leg_2(GameObject go) {
		if (WMG_Link_Click_Leg != null) {
			WMG_Series aSeries = go.transform.parent.GetComponent<WMG_Legend_Entry>().seriesRef;
			WMG_Link_Click_Leg(aSeries, go.GetComponent<WMG_Link>());
		}
	}

	// Pie Slice
	public delegate void WMG_Pie_Slice_Click_H(WMG_Pie_Graph pieGraph, WMG_Pie_Graph_Slice aSlice);
	public event WMG_Pie_Slice_Click_H WMG_Pie_Slice_Click;
	
	public void addPieSliceClickEvent(GameObject go) {
		AddEventTrigger(WMG_Pie_Slice_Click_2, EventTriggerType.PointerClick, go);
	}
	
	private void WMG_Pie_Slice_Click_2(GameObject go) {
		if (WMG_Pie_Slice_Click != null) {
			WMG_Pie_Graph_Slice pieSlice = go.transform.parent.GetComponent<WMG_Pie_Graph_Slice>();
			if (pieSlice == null) {
				pieSlice = go.transform.parent.parent.GetComponent<WMG_Pie_Graph_Slice>();
			}
			WMG_Pie_Slice_Click(pieSlice.pieRef, pieSlice);
		}
	}

	// Pie Legend
	public delegate void WMG_Pie_Legend_Entry_Click_H(WMG_Pie_Graph pieGraph, WMG_Legend_Entry legendEntry);
	public event WMG_Pie_Legend_Entry_Click_H WMG_Pie_Legend_Entry_Click;
	
	public void addPieLegendEntryClickEvent(GameObject go) {
		AddEventTrigger(WMG_Pie_Legend_Entry_Click_2, EventTriggerType.PointerClick, go);
	}
	
	private void WMG_Pie_Legend_Entry_Click_2(GameObject go) {
		if (WMG_Pie_Legend_Entry_Click != null) {
			WMG_Pie_Graph pieGraph = go.GetComponent<WMG_Legend_Entry>().legend.theGraph.GetComponent<WMG_Pie_Graph>();
			WMG_Pie_Legend_Entry_Click(pieGraph, go.GetComponent<WMG_Legend_Entry>());
		}
	}

	#endregion

	#region GraphHoverEvents
	
	// MouseEnter events
	// Series Node
	public delegate void WMG_MouseEnter_H(WMG_Series aSeries, WMG_Node aNode, bool state);
	public event WMG_MouseEnter_H WMG_MouseEnter;
	
	public void addNodeMouseEnterEvent(GameObject go) {
		AddEventTrigger(WMG_MouseEnter_2, EventTriggerType.PointerEnter, go, true);
		AddEventTrigger(WMG_MouseEnter_2, EventTriggerType.PointerExit, go, false);
	}
	
	private void WMG_MouseEnter_2(GameObject go, bool state) {
		if (WMG_MouseEnter != null) {
			WMG_Series aSeries = go.transform.parent.parent.GetComponent<WMG_Series>();
			WMG_MouseEnter(aSeries, go.GetComponent<WMG_Node>(), state);
		}
	}
	
	// Series Link
	public delegate void WMG_Link_MouseEnter_H(WMG_Series aSeries, WMG_Link aLink, bool state);
	public event WMG_Link_MouseEnter_H WMG_Link_MouseEnter;
	
	public void addLinkMouseEnterEvent(GameObject go) {
		AddEventTrigger(WMG_Link_MouseEnter_2, EventTriggerType.PointerEnter, go, true);
		AddEventTrigger(WMG_Link_MouseEnter_2, EventTriggerType.PointerExit, go, false);
	}
	
	private void WMG_Link_MouseEnter_2(GameObject go, bool state) {
		if (WMG_Link_MouseEnter != null) {
			WMG_Series aSeries = go.transform.parent.parent.GetComponent<WMG_Series>();
			WMG_Link_MouseEnter(aSeries, go.GetComponent<WMG_Link>(), state);
		}
	}
	
	// Series Legend Node
	public delegate void WMG_MouseEnter_Leg_H(WMG_Series aSeries, WMG_Node aNode, bool state);
	public event WMG_MouseEnter_Leg_H WMG_MouseEnter_Leg;
	
	public void addNodeMouseEnterEvent_Leg(GameObject go) {
		AddEventTrigger(WMG_MouseEnter_Leg_2, EventTriggerType.PointerEnter, go, true);
		AddEventTrigger(WMG_MouseEnter_Leg_2, EventTriggerType.PointerExit, go, false);
	}
	
	private void WMG_MouseEnter_Leg_2(GameObject go, bool state) {
		if (WMG_MouseEnter_Leg != null) {
			WMG_Series aSeries = go.transform.parent.GetComponent<WMG_Legend_Entry>().seriesRef;
			WMG_MouseEnter_Leg(aSeries, go.GetComponent<WMG_Node>(), state);
		}
	}
	
	// Series Legend Link
	public delegate void WMG_Link_MouseEnter_Leg_H(WMG_Series aSeries, WMG_Link aLink, bool state);
	public event WMG_Link_MouseEnter_Leg_H WMG_Link_MouseEnter_Leg;
	
	public void addLinkMouseEnterEvent_Leg(GameObject go) {
		AddEventTrigger(WMG_Link_MouseEnter_Leg_2, EventTriggerType.PointerEnter, go, true);
		AddEventTrigger(WMG_Link_MouseEnter_Leg_2, EventTriggerType.PointerExit, go, false);
	}
	
	private void WMG_Link_MouseEnter_Leg_2(GameObject go, bool state) {
		if (WMG_Link_MouseEnter_Leg != null) {
			WMG_Series aSeries = go.transform.parent.GetComponent<WMG_Legend_Entry>().seriesRef;
			WMG_Link_MouseEnter_Leg(aSeries, go.GetComponent<WMG_Link>(), state);
		}
	}

	// Pie Slice
	public delegate void WMG_Pie_Slice_MouseEnter_H(WMG_Pie_Graph pieGraph, WMG_Pie_Graph_Slice aSlice, bool state);
	public event WMG_Pie_Slice_MouseEnter_H WMG_Pie_Slice_MouseEnter;
	
	public void addPieSliceMouseEnterEvent(GameObject go) {
		AddEventTrigger(WMG_Pie_Slice_MouseEnter_2, EventTriggerType.PointerEnter, go, true);
		AddEventTrigger(WMG_Pie_Slice_MouseEnter_2, EventTriggerType.PointerExit, go, false);
	}
	
	private void WMG_Pie_Slice_MouseEnter_2(GameObject go, bool state) {
		if (WMG_Pie_Slice_MouseEnter != null) {
			WMG_Pie_Graph_Slice pieSlice = go.transform.parent.GetComponent<WMG_Pie_Graph_Slice>();
			if (pieSlice == null) {
				pieSlice = go.transform.parent.parent.GetComponent<WMG_Pie_Graph_Slice>();
			}
			WMG_Pie_Slice_MouseEnter(pieSlice.pieRef, pieSlice, state);
		}
	}

	#endregion

	// Mouse leave events are handled automatiaclly by hover events (true / false)
	// These function stubs remain so that the code can be the same across multiple GUI systems
	
	// MouseLeave events
	// Series Node
	public void addNodeMouseLeaveEvent(GameObject go) {
		
	}
	
	// Series Link
	public void addLinkMouseLeaveEvent(GameObject go) {
		
	}
	
	// Series Legend Node
	public void addNodeMouseLeaveEvent_Leg(GameObject go) {
		
	}
	
	// Series Legend Link
	public void addLinkMouseLeaveEvent_Leg(GameObject go) {
		
	}
}
