using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class DemoTiming : MonoBehaviour {

	public	bool	SimulateLatency=false;

    public Text     SpeedSliderText;
    public Slider   SpeedSlider;

    public Text     FPSSliderText;
    public Slider   FPSSlider;

	public	KeyCode	ToggleKey=KeyCode.Alpha0;

    Vector2 mVelocity = Vector2.right;

	SpriteRenderer	mSR;

    public  void    UpdateFPSSider() {
        UpdateFPSSliderNumber();
    }

    public void UpdateSpeedSider() {
        UpdateSpeedSliderNumber();
    }

    float tTimeout = 0f;

    Vector2 mPosition;

    void    UpdateFPSSliderNumber() {
        FPSSliderText.text = string.Format("FPS {0:f2}", FPSSlider.value);
    }

    void UpdateSpeedSliderNumber() {
        SpeedSliderText.text = string.Format("Speed {0:f2}", SpeedSlider.value);
    }

    private void Start() {
        mPosition = transform.position;
        UpdateFPSSliderNumber();
        UpdateSpeedSliderNumber();
		mSR = GetComponent<SpriteRenderer> ();
    }


    void Update() {
		ToggleLatency ();
        tTimeout += Time.deltaTime;
		if (SimulateLatency) {
			mSR.color = Color.yellow;
			if (FPSSlider.value > Mathf.Epsilon) {
				if (tTimeout >= 1f / FPSSlider.value) {
					transform.position = mPosition;
					tTimeout = 0f;
				}
			}
		} else {
			mSR.color = Color.white;
			transform.position = mPosition;
		}
        mPosition += mVelocity * Time.deltaTime * SpeedSlider.value;
        DoWrap();
    }

	// Update is called once per frame
	void DoWrap() {
        float tHeight = Camera.main.orthographicSize;       //Height
        float tWidth = Camera.main.aspect * tHeight;        //Width
        if (mPosition.y > tHeight) {
            mPosition.y -= tHeight * 2f;
        }
        else if (mPosition.y < -tHeight) {
            mPosition.y +=  tHeight * 2f;
        }

        if (mPosition.x > tWidth) {
            mPosition.x -=  tWidth * 2f;
        }
        else if (mPosition.x < -tWidth) {
            mPosition.x +=  tWidth * 2f;
        }
    }


	void	ToggleLatency() {
		if (Input.GetMouseButtonDown (0)) {
			Ray tRay = Camera.main.ScreenPointToRay (Input.mousePosition);               //Make a ray from screen position into the game scene
			RaycastHit2D tHit = Physics2D.Raycast (tRay.origin, tRay.direction);     //Cast ray, if it hits a game object we will know, NB only first collision is reported
			if (tHit.collider != null) {
				DemoTiming tDemo = tHit.collider.gameObject.GetComponent<DemoTiming> ();
				if (tDemo != null) {
					SimulateLatency = !SimulateLatency;
				}
			}
		}
		if (Input.GetKeyDown (ToggleKey)) {
			SimulateLatency = !SimulateLatency;
		}
	}
}
