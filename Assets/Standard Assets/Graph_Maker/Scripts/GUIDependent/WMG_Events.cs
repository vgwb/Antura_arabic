using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Defines click and hover events for graphs that inherit WMG_Graph_Manager.
/// </summary>
public class WMG_Events : WMG_GUI_Functions {

	private void AddEventTrigger(UnityAction<GameObject, PointerEventData> action, EventTriggerType triggerType, GameObject go)
	{
		EventTrigger eventTrigger = go.GetComponent<EventTrigger>();
		if (eventTrigger == null) {
			eventTrigger = go.AddComponent<EventTrigger>();
			eventTrigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
		}
		// Create a nee TriggerEvent and add a listener
		EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
		trigger.AddListener((eventData) => action(go, (PointerEventData)eventData)); // capture and pass the event data to the listener
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
	public delegate void WMG_Click_H(WMG_Series aSeries, WMG_Node aNode, PointerEventData pointerEventData);
	/// <summary>
	/// Occurs when a series point is clicked. 
	/// @code
	/// graph.WMG_Click += MyClick;
	/// void MyClick(WMG_Series aSeries, WMG_Node aNode, UnityEngine.EventSystems.PointerEventData pointerEventData) {}
	/// @endcode
	/// </summary>
	public event WMG_Click_H WMG_Click;
	
	public void addNodeClickEvent(GameObject go) {
		AddEventTrigger(WMG_Click_2, EventTriggerType.PointerClick, go);
	}
	
	private void WMG_Click_2(GameObject go, PointerEventData pointerEventData) {
		if (WMG_Click != null) {
			WMG_Series aSeries = go.transform.parent.parent.GetComponent<WMG_Series>();
			WMG_Click(aSeries, go.GetComponent<WMG_Node>(), pointerEventData);
		}
	}
	
	// Series Link
	public delegate void WMG_Link_Click_H(WMG_Series aSeries, WMG_Link aLink, PointerEventData pointerEventData);
	/// <summary>
	/// Occurs when a series link / line is clicked. 
	/// @code
	/// graph.WMG_Link_Click += MyLinkClick;
	/// void MyLinkClick(WMG_Series aSeries, WMG_Link aLink, UnityEngine.EventSystems.PointerEventData pointerEventData) {}
	/// @endcode
	/// </summary>
	public event WMG_Link_Click_H WMG_Link_Click;
	
	public void addLinkClickEvent(GameObject go) {
		AddEventTrigger(WMG_Link_Click_2, EventTriggerType.PointerClick, go);
	}
	
	private void WMG_Link_Click_2(GameObject go, PointerEventData pointerEventData) {
		if (WMG_Link_Click != null) {
			WMG_Series aSeries = go.transform.parent.parent.GetComponent<WMG_Series>();
			WMG_Link_Click(aSeries, go.GetComponent<WMG_Link>(), pointerEventData);
		}
	}
	
	// Series Legend Node
	public delegate void WMG_Click_Leg_H(WMG_Series aSeries, WMG_Node aNode, PointerEventData pointerEventData);
	/// <summary>
	/// Occurs when a series legend node / swatch is clicked. 
	/// @code
	/// graph.WMG_Click_Leg += MyClickLeg;
	/// void MyClickLeg(WMG_Series aSeries, WMG_Node aNode, UnityEngine.EventSystems.PointerEventData pointerEventData) {}
	/// @endcode
	/// </summary>
	public event WMG_Click_Leg_H WMG_Click_Leg;
	
	public void addNodeClickEvent_Leg(GameObject go) {
		AddEventTrigger(WMG_Click_Leg_2, EventTriggerType.PointerClick, go);
	}
	
	private void WMG_Click_Leg_2(GameObject go, PointerEventData pointerEventData) {
		if (WMG_Click_Leg != null) {
			WMG_Series aSeries = go.transform.parent.GetComponent<WMG_Legend_Entry>().seriesRef;
			WMG_Click_Leg(aSeries, go.GetComponent<WMG_Node>(), pointerEventData);
		}
	}
	
	// Series Legend Link
	public delegate void WMG_Link_Click_Leg_H(WMG_Series aSeries, WMG_Link aLink, PointerEventData pointerEventData);
	/// <summary>
	/// Occurs when a series legend link / line is clicked. 
	/// @code
	/// graph.WMG_Link_Click_Leg += MyLinkClickLeg;
	/// void MyLinkClickLeg(WMG_Series aSeries, WMG_Link aLink, UnityEngine.EventSystems.PointerEventData pointerEventData) {}
	/// @endcode
	/// </summary>
	public event WMG_Link_Click_Leg_H WMG_Link_Click_Leg;
	
	public void addLinkClickEvent_Leg(GameObject go) {
		AddEventTrigger(WMG_Link_Click_Leg_2, EventTriggerType.PointerClick, go);
	}
	
	private void WMG_Link_Click_Leg_2(GameObject go, PointerEventData pointerEventData) {
		if (WMG_Link_Click_Leg != null) {
			WMG_Series aSeries = go.transform.parent.GetComponent<WMG_Legend_Entry>().seriesRef;
			WMG_Link_Click_Leg(aSeries, go.GetComponent<WMG_Link>(), pointerEventData);
		}
	}

	// Pie Slice
	public delegate void WMG_Pie_Slice_Click_H(WMG_Pie_Graph pieGraph, WMG_Pie_Graph_Slice aSlice, PointerEventData pointerEventData);
	/// <summary>
	/// Occurs when a pie graph slice is clicked. 
	/// @code
	/// graph.WMG_Pie_Slice_Click += MyPieClick;
	/// void MyPieClick(WMG_Pie_Graph pieGraph, WMG_Pie_Graph_Slice aSlice, UnityEngine.EventSystems.PointerEventData pointerEventData) {}
	/// @endcode
	/// </summary>
	public event WMG_Pie_Slice_Click_H WMG_Pie_Slice_Click;
	
	public void addPieSliceClickEvent(GameObject go) {
		AddEventTrigger(WMG_Pie_Slice_Click_2, EventTriggerType.PointerClick, go);
	}
	
	private void WMG_Pie_Slice_Click_2(GameObject go, PointerEventData pointerEventData) {
		if (WMG_Pie_Slice_Click != null) {
			WMG_Pie_Graph_Slice pieSlice = go.transform.parent.GetComponent<WMG_Pie_Graph_Slice>();
			if (pieSlice == null) {
				pieSlice = go.transform.parent.parent.GetComponent<WMG_Pie_Graph_Slice>();
			}
			WMG_Pie_Slice_Click(pieSlice.pieRef, pieSlice, pointerEventData);
		}
	}

	// Pie Legend
	public delegate void WMG_Pie_Legend_Entry_Click_H(WMG_Pie_Graph pieGraph, WMG_Legend_Entry legendEntry, PointerEventData pointerEventData);
	/// <summary>
	/// Occurs when a pie graph legend swatch is clicked. 
	/// @code
	/// graph.WMG_Pie_Legend_Entry_Click += MyPieLegendClick;
	/// void MyPieLegendClick(WMG_Pie_Graph pieGraph, WMG_Legend_Entry legendEntry, UnityEngine.EventSystems.PointerEventData pointerEventData) {}
	/// @endcode
	/// </summary>
	public event WMG_Pie_Legend_Entry_Click_H WMG_Pie_Legend_Entry_Click;
	
	public void addPieLegendEntryClickEvent(GameObject go) {
		AddEventTrigger(WMG_Pie_Legend_Entry_Click_2, EventTriggerType.PointerClick, go);
	}
	
	private void WMG_Pie_Legend_Entry_Click_2(GameObject go, PointerEventData pointerEventData) {
		if (WMG_Pie_Legend_Entry_Click != null) {
			WMG_Pie_Graph pieGraph = go.GetComponent<WMG_Legend_Entry>().legend.theGraph.GetComponent<WMG_Pie_Graph>();
			WMG_Pie_Legend_Entry_Click(pieGraph, go.GetComponent<WMG_Legend_Entry>(), pointerEventData);
		}
	}

	// Ring graph
	public delegate void WMG_Ring_Click_H(WMG_Ring ring, PointerEventData pointerEventData);
	/// <summary>
	/// Occurs when a ring graph ring / band is clicked. 
	/// @code
	/// graph.WMG_Ring_Click += MyRingClick;
	/// void MyRingClick(WMG_Ring ring, UnityEngine.EventSystems.PointerEventData pointerEventData) {}
	/// @endcode
	/// </summary>
	public event WMG_Ring_Click_H WMG_Ring_Click;
	
	public void addRingClickEvent(GameObject go) {
		AddEventTrigger(WMG_Ring_Click_2, EventTriggerType.PointerClick, go);
	}
	
	private void WMG_Ring_Click_2(GameObject go, PointerEventData pointerEventData) {
		if (WMG_Ring_Click != null) {
			WMG_Ring ring = go.transform.parent.GetComponent<WMG_Ring>();
			WMG_Ring_Click(ring, pointerEventData);
		}
	}

	#endregion

	#region GraphHoverEvents
	
	// MouseEnter events
	// Series Node
	public delegate void WMG_MouseEnter_H(WMG_Series aSeries, WMG_Node aNode, bool state);
	/// <summary>
	/// Occurs when a series point is hovered. 
	/// @code
	/// graph.WMG_MouseEnter += MyHover;
	/// void MyHover(WMG_Series aSeries, WMG_Node aNode, bool state) {}
	/// @endcode
	/// </summary>
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
	/// <summary>
	/// Occurs when a series link / line is hovered. 
	/// @code
	/// graph.WMG_Link_MouseEnter += MyLinkHover;
	/// void MyLinkHover(WMG_Series aSeries, WMG_Link aLink, bool state) {}
	/// @endcode
	/// </summary>
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
	/// <summary>
	/// Occurs when a series legend node / swatch is hovered. 
	/// @code
	/// graph.WMG_MouseEnter_Leg += MyHoverLeg;
	/// void MyHoverLeg(WMG_Series aSeries, WMG_Node aNode, bool state) {}
	/// @endcode
	/// </summary>
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
	/// <summary>
	/// Occurs when a series legend link / line is hovered. 
	/// @code
	/// graph.WMG_Link_MouseEnter_Leg += MyLinkHoverLeg;
	/// void MyLinkHoverLeg(WMG_Series aSeries, WMG_Link aLink, bool state) {}
	/// @endcode
	/// </summary>
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
	/// <summary>
	/// Occurs when a pie graph slice is hovered. 
	/// @code
	/// graph.WMG_Pie_Slice_MouseEnter += MyPieHover;
	/// void MyPieHover(WMG_Pie_Graph pieGraph, WMG_Pie_Graph_Slice aSlice, bool state) {}
	/// @endcode
	/// </summary>
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

	// Ring graph
	public delegate void WMG_Ring_MouseEnter_H(WMG_Ring ring, bool state);
	/// <summary>
	/// Occurs when a ring graph ring / band is hovered. 
	/// @code
	/// graph.WMG_Ring_MouseEnter += MyRingHover;
	/// void MyRingHover(WMG_Ring ring, bool state) {}
	/// @endcode
	/// </summary>
	public event WMG_Ring_MouseEnter_H WMG_Ring_MouseEnter;
	
	public void addRingMouseEnterEvent(GameObject go) {
		AddEventTrigger(WMG_Ring_MouseEnter_2, EventTriggerType.PointerEnter, go, true);
		AddEventTrigger(WMG_Ring_MouseEnter_2, EventTriggerType.PointerExit, go, false);
	}
	
	private void WMG_Ring_MouseEnter_2(GameObject go, bool state) {
		if (WMG_Ring_MouseEnter != null) {
			WMG_Ring ring = go.transform.parent.GetComponent<WMG_Ring>();
			WMG_Ring_MouseEnter(ring, state);
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
