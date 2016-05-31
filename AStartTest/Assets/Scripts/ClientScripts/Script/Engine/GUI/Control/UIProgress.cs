using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIProgress : MonoBehaviour {
    public Image _image;
    public Text _text;

    private const float DEFAULT_SPEED = 0.01f;
    private float _destValue = 0;
    private float _speed = DEFAULT_SPEED;
    private float _currentValue = 0;
    private bool _isTransition = false;

    public void SetText(string text)
    {
        if (_text != null) {
            _text.text = text;
        }
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetValue(float value)
    {
        _destValue = value;
        _isTransition = true;
    }

    public void UpdateProgress(float value)
    {
        _image.fillAmount = value;
    }

    public void Reset()
    {
        _currentValue = 0;
        _destValue = 0;
        _speed = DEFAULT_SPEED;
        _isTransition = false;

        UpdateProgress(0);
    }

    public void Update()
    {
        if (!_isTransition) {
            return;
        }

        if (_currentValue + _speed >= _destValue) {
            _isTransition = false;
            _currentValue = _destValue;
            UpdateProgress(_currentValue);
            return;
        }

        _currentValue += _speed;
        UpdateProgress(_currentValue);
    }
}
