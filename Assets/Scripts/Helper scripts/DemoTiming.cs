using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class DemoTiming : MonoBehaviour {

    public Text     SpeedSliderText;
    public Slider   SpeedSlider;

    public Text     FPSSliderText;
    public Slider   FPSSlider;


    Vector2 mVelocity = Vector2.right;

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
    }


    void Update() {
        tTimeout += Time.deltaTime;
        if(FPSSlider.value>Mathf.Epsilon) {
            if (tTimeout >= 1f/ FPSSlider.value) {
                transform.position = mPosition;
                tTimeout = 0f;
            }
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
}
