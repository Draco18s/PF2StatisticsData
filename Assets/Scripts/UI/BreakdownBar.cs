using Assets.draco18s.ui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakdownBar : MonoBehaviour
{
	private Image[] bits;
    // Start is called before the first frame update
    void Start() {
		bits = new Image[20];
        for(int i=0;i<20;i++) {
			GameObject go = new GameObject(i.ToString(), typeof(RectTransform), typeof(Image));
			RectTransform rt = go.GetComponent<RectTransform>();
			rt.SetParent(transform);
			rt.anchorMin = new Vector2(i * 10 / 200f, 0);
			rt.anchorMax = new Vector2((i + 1) * 10 / 200f, 1);
			rt.anchoredPosition = new Vector2(0, 0);
			rt.offsetMin = new Vector2(0, 0);
			rt.offsetMax = new Vector2(0, 0);
			Image img = go.GetComponent<Image>();
			bits[i] = img;
		}
		transform.Find("TenNotch").SetAsLastSibling();
    }

	public void SetBitColors(Color[] colors) {
		if(colors.Length != bits.Length) throw new System.Exception("Invalid array size!");
		for(int i = 0; i < 20; i++) {
			bits[i].color = colors[i];
		}
	}

	public void SetNotches(int low, int high) {
		low++;
		RectTransform rt = (RectTransform)transform.Find("CritNotch");
		rt.anchoredPosition = new Vector2(high * 10 - 105, 11);
		rt.SetAsLastSibling();
		Image i = rt.GetComponent<Image>();
		i.AddHover(p => {
			Main.ShowTooltip(rt.position, high+"+ might crit");
		});
		i.enabled = false;// high > 0;
		RectTransform rt2 = (RectTransform)transform.Find("MissNotch");
		rt2.anchoredPosition = new Vector2(low * 10 - 105, -11);
		rt2.SetAsLastSibling();
		i = rt2.GetComponent<Image>();
		i.AddHover(p => {
			Main.ShowTooltip(rt2.position, low+"+ always succeeds");
		});
		i.enabled = low > 0 && low < 21;
	}
}
