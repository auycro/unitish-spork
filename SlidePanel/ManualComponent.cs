using UnityEngine;
using System.Collections;

public class ManualComponent : MonoBehaviour {
	[SerializeField]private UnityEngine.UI.Button button_left;
	[SerializeField]private UnityEngine.UI.Button button_right;
	[SerializeField]private RectTransform grid;
	[SerializeField]private GameObject[] content;
	[SerializeField]private GameObject title;
	[SerializeField]private GameObject page;
	[SerializeField]private int index;
	[SerializeField]private float content_width = 493.0f;
	[SerializeField]private bool isMoving = false;

	// Update is called once per frame
	void Update () {
		if (isMoving) {
			button_left.enabled = button_right.enabled = false;
		} else {
			button_left.enabled = button_right.enabled = true;
			if (index == 0) {
				button_left.gameObject.SetActive(false);
			} else {
				button_left.gameObject.SetActive (true);
			}

			if (index == (content.Length - 1)) {
				button_right.gameObject.SetActive (false);
			} else {
				button_right.gameObject.SetActive (true);
			}
		}
	}

	public void ButtonLeftOnClick(){
		StartCoroutine(MoveContent (index, index-1));
	}

	public void ButtonRightOnClick(){
		StartCoroutine(MoveContent (index, index+1));
	}

	IEnumerator MoveContent(int from_content, int to_content){
		isMoving = true;
		Vector3 new_position;
		//RectTransform grid = this.transform.Find ("Wrapper/MASK/Grid").GetComponent<RectTransform>();

		if (from_content < to_content) {
			//Go next
			new_position = grid.transform.localPosition - new Vector3(content_width,0,0);
		} else {
			//Go back
			new_position = grid.transform.localPosition + new Vector3(content_width,0,0);
		}

		float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
		float increment = 0.05f; //The amount of change to apply.
		while (true) {
			progress += increment;
			grid.transform.localPosition = Vector3.Lerp(grid.transform.localPosition, new_position, progress);

			if (progress > 1) {
				isMoving = false;
				index = to_content;
				break;
			}
			yield return null;
		}
	}
}
