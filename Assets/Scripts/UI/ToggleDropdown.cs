using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleDropdown : Dropdown
{
	public List<int> activeValues;
	int lastValue = -1;

	public ToggleDropdown() : base() {
		activeValues = new List<int>();
		onValueChanged.AddListener(delegate {
			if(value > 0) {
				if(activeValues.Contains(value)) {
					activeValues.Remove(value);
					lastValue = -1;
				}
				else {
					activeValues.Add(value);
					lastValue = value;
				}
				SetValueWithoutNotify(0);
			}
		});
	}

	public override void OnPointerClick(PointerEventData eventData) {
		Show();
		Transform cont = transform.Find("Dropdown List/Viewport/Content");
		for(int i = 0; i + 1 < cont.childCount; i++) {
			Transform t = cont.GetChild(i + 1);
			t.GetComponent<Toggle>().SetIsOnWithoutNotify(activeValues.Contains(i));
		}
		SetValueWithoutNotify(0);
	}
}
