using UnityEngine;
using System.Collections;
using System;

public class AN_PlusButton {

	private int _ButtonId = 0;
	private TextAnchor _anchor = TextAnchor.MiddleCenter;
	private int _x;
	private int _y;

	private bool _IsShowed = true;


	public Action ButtonClicked = delegate {};

	
	public AN_PlusButton(string url, AN_PlusBtnSize btnSize, AN_PlusBtnAnnotation annotation) {

		_ButtonId = nextId;
		AN_PlusButtonProxy.createPlusButton(_ButtonId, url, (int)btnSize, (int) annotation);
		AN_PlusButtonsManager.Instance.RegisterButton(this);
	}
	

	public void SetGravity(TextAnchor btnAnchor) {
		_anchor = btnAnchor;
		AN_PlusButtonProxy.setGravity((int)gravity, _ButtonId);
	}
	
	
	public void SetPosition(int btnX, int btnY) {
		_x = btnX;
		_y = btnY;
		AN_PlusButtonProxy.setPosition(_x, _y, _ButtonId);
	}
	
	
	public void Show() {
		_IsShowed = true;
		AN_PlusButtonProxy.show(_ButtonId);
	}
	
	
	public void Hide() {
		_IsShowed = false;
		AN_PlusButtonProxy.hide(_ButtonId);
	}
	
	
	public void Refresh() {
		AN_PlusButtonProxy.refresh(_ButtonId);
	}




	public int ButtonId {
		get {
			return _ButtonId;
		}
	}

	public int x {
		get {
			return _x;
		}
	}

	public int y {
		get {
			return _y;
		}
	}

	public bool IsShowed {
		get {
			return _IsShowed;
		}
	}

	public TextAnchor anchor {
		get {
			return _anchor;
		}
	}


	public GoogleGravity gravity {
		get {
			switch(_anchor) {
			case TextAnchor.LowerCenter:
				return GoogleGravity.BOTTOM | GoogleGravity.CENTER;
			case TextAnchor.LowerLeft:
				return GoogleGravity.BOTTOM | GoogleGravity.LEFT;
			case TextAnchor.LowerRight:
				return GoogleGravity.BOTTOM | GoogleGravity.RIGHT;
				
			case TextAnchor.MiddleCenter:
				return GoogleGravity.CENTER;
			case TextAnchor.MiddleLeft:
				return GoogleGravity.CENTER | GoogleGravity.LEFT;
			case TextAnchor.MiddleRight:
				return GoogleGravity.CENTER | GoogleGravity.RIGHT;
				
			case TextAnchor.UpperCenter:
				return GoogleGravity.TOP | GoogleGravity.CENTER;
			case TextAnchor.UpperLeft:
				return GoogleGravity.TOP | GoogleGravity.LEFT;
			case TextAnchor.UpperRight:
				return GoogleGravity.TOP | GoogleGravity.RIGHT;
			}
			
			return GoogleGravity.TOP;
		}
	}



	
	public void FireClickAction() {
		ButtonClicked();
	}

	private static int _nextId = 0;
	private static int nextId {
		get {
			_nextId++;
			return _nextId;
		}
	}
}
