using System.Collections;
using UnityEngine;

public class DeliveryBox : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private AnimationCurve _dropCurve;
    [SerializeField] private float _dropDuration = 1.2f;
    //[SerializeField] private float _dropHeight = 3f;

    [Header("Visuals")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private TMPro.TextMeshProUGUI _labelText;
    [SerializeField] private ParticleSystem _landVFX;

    [Header("Auto-Open")]
    [SerializeField] private float _autoOpenDelay = 5f;

    private IngredientType _contentType;
    private int _quantity = 10;
    private bool _opened;

    public static DeliveryBox Spawn(
        DeliveryBox prefab,
        IngredientType type,
        int quantity,
        Vector3 deliveryPoint)
    {
        var box = Instantiate(prefab,
            deliveryPoint + Vector3.up * 4f,
            Quaternion.Euler(0, Random.Range(0, 360f), 0));

        box._contentType = type;
        box._quantity = quantity;
        box.StartCoroutine(box.DropAnimation(deliveryPoint));
        return box;
    }

    private IEnumerator DropAnimation(Vector3 target)
    {
        Vector3 start = transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / _dropDuration;
            float y = Mathf.Lerp(start.y, target.y, _dropCurve.Evaluate(t));
            transform.position = new Vector3(target.x, y, target.z);
            yield return null;
        }

        transform.position = target;
        _landVFX.Play();

        Camera.main?.GetComponent<CameraShake>()?.Shake(0.3f, 0.15f);

        if (_labelText != null)
            _labelText.text = $"{_contentType}\nx{_quantity}";

        yield return new WaitForSeconds(_autoOpenDelay);
        if (!_opened) OpenBox();
    }

    public void OpenBox()
    {
        if (_opened) return;
        _opened = true;

        InventorySystem.Instance.Restock(_contentType, _quantity);
        StartCoroutine(DisappearAfterDelay(2f));
    }

    private IEnumerator DisappearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        float t = 0f;
        Vector3 startScale = transform.localScale;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.5f;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        Destroy(gameObject);
    }

    public class CameraShake : MonoBehaviour
    {
        private Coroutine _shakeRoutine;

        public void Shake(float duration, float magnitude)
        {
            if (_shakeRoutine != null) StopCoroutine(_shakeRoutine);
            _shakeRoutine = StartCoroutine(DoShake(duration, magnitude));
        }

        private IEnumerator DoShake(float duration, float magnitude)
        {
            Vector3 origin = transform.localPosition;
            float t = 0f;

            while (t < duration)
            {
                t += Time.deltaTime;
                float progress = t / duration;
                float strength = magnitude * (1f - progress);
                transform.localPosition = origin + Random.insideUnitSphere * strength;
                yield return null;
            }
            transform.localPosition = origin;
        }
    }
}
