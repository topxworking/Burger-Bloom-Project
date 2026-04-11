using UnityEngine;
using UnityEngine.UI;

public class PatienceBar : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private Color _fillColor = new Color(0.2f, 0.85f, 0.2f);
    [SerializeField] private Color _urgentColor = new Color(0.9f, 0.15f, 0.1f);
    [SerializeField] private float _floatHeight = 2.2f;
    [SerializeField] private Canvas _canvas;

    private Customer _customer;
    private Transform _cam;

    public void Init(Customer customer)
    {
        _customer = customer;
        _cam = Camera.main?.transform;
        transform.SetParent(customer.transform);
        transform.localPosition = Vector3.up * _floatHeight;

        if (_canvas)
            _canvas.renderMode = RenderMode.WorldSpace;
    }

    private void Update()
    {
        if (_customer == null) return;

        if (_cam) transform.LookAt(transform.position + _cam.forward);

        float ratio = _customer.PatienceRadio;
        if (_fillImage)
        {
            _fillImage.fillAmount = ratio;
            _fillImage.color = Color.Lerp(_urgentColor, _fillColor, ratio);
        }

        bool show = _customer.Order != null &&
            _customer.PatienceRadio > 0f;
        gameObject.SetActive(show);
    }
}
