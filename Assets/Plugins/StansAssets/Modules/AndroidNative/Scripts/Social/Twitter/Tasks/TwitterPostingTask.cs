using UnityEngine;
using System;
using System.Collections;

public class TwitterPostingTask : AsyncTask {

	private string 		_status = "";
	private Texture2D 	_texture = null;

	private TwitterManagerInterface _controller;


	public event Action<TWResult> ActionComplete = delegate{};

	public static TwitterPostingTask Cretae() {
		return	new GameObject("TwitterPositngTask").AddComponent<TwitterPostingTask>();
	}


	public void Post(string status, Texture2D texture, TwitterManagerInterface controller) {
		_status = status;
		_texture = texture;
		_controller = controller;


		if(_controller.IsInited) {
			OnTWInited(null);
		} else {

			_controller.OnTwitterInitedAction += OnTWInited;
			_controller.Init();
		}

	}



	private void OnTWInited(TWResult result) {
		_controller.OnTwitterInitedAction -= OnTWInited;

		if(_controller.IsAuthed) {
			OnTWAuth(new TWResult(true, "Auth Success"));
		} else {
			_controller.OnAuthCompleteAction += OnTWAuth;
			_controller.AuthenticateUser();
		}
	}
	
	
	private void OnTWAuth(TWResult result) {

		_controller.OnAuthCompleteAction -= OnTWAuth;

		if(result.IsSucceeded) {
			_controller.OnPostingCompleteAction +=  OnPost;

			if(_texture != null) {
				_controller.Post(_status, _texture);
			} else  {
				_controller.Post(_status);
			}
		} else {
			TWResult res =  new TWResult(false, "Auth Failed");
			ActionComplete(res);
		}



	

	}


	
	private void OnPost(TWResult res) {
		_controller.OnPostingCompleteAction -=  OnPost;
		ActionComplete(res);
	}
	

}
