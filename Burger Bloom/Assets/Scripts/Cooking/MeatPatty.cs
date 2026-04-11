using UnityEngine;

public class MeatPatty : Ingredient
{
    [Header("Cooking Settings")]
    [SerializeField] private float _cookTime = 8f;
    [SerializeField] private float _overcookTime = 4f;
    [SerializeField] private float _bornTime = 3f;

    [Header("Visuals")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Color _rawColor = new Color(0.9f, 0.5f, 0.4f);
    [SerializeField] private Color _cookedColor = new Color(0.4f, 0.2f, 0.1f);
    [SerializeField] private Color _burntColor = new Color(0.1f, 0.08f, 0.05f);
    [SerializeField] private ParticleSystem _steamVFX;
    [SerializeField] private ParticleSystem _smokeVFX;
    [SerializeField] private ParticleSystem _fireVFX;

    private StateMachine<MeatState> _fsm;
    private float _heatTimer;
    private bool _onGrill;

    public MeatState State => _fsm.Current;
    public bool IsCooked => _fsm.Current == MeatState.Cooked;
    public bool IsBurnt => _fsm.Current == MeatState.Burnt;

    private bool _side1Done = false;
    private bool _flipped = false;

    public bool ReadyToFlip => _side1Done && !_flipped;
    public bool FullyCooked => _flipped && _fsm.Current == MeatState.Cooked;

    private new void Awake()
    {
        base.Awake();
        InitFSM();
    }

    private void InitFSM()
    {
        _fsm = new StateMachine<MeatState>(MeatState.Raw);

        _fsm.RegisterEnter(MeatState.Raw, () => SetColor(_rawColor));
        _fsm.RegisterEnter(MeatState.Cooking, OnStartCooking);
        _fsm.RegisterEnter(MeatState.Cooked, OnCooked);
        _fsm.RegisterEnter(MeatState.Overcooked, OnOvercooked);
        _fsm.RegisterEnter(MeatState.Burnt, OnBurnt);

        _fsm.OnStateChanged += (_, next) =>
        EventBus.Publish(new OnPattyStateChanged { State = next });
    }

    private void Update()
    {
        if (!_onGrill) return;
        _fsm.Tick();

        _heatTimer += Time.deltaTime;

        switch (_fsm.Current)
        {
            case MeatState.Cooking:
                float cookProgress = _heatTimer / (_cookTime * 0.5f);
                SetColor(Color.Lerp(_rawColor, _cookedColor, cookProgress));
                if (_heatTimer >= _cookTime * 0.5f)
                {
                    _side1Done = true;
                }
                if (_flipped && _heatTimer >= _cookTime)
                    _fsm.TransitionTo(MeatState.Cooked);
                break;

            case MeatState.Cooked:
                if (_heatTimer >= _overcookTime)
                    _fsm.TransitionTo(MeatState.Overcooked);
                break;

            case MeatState.Overcooked:
                if (_heatTimer >= _bornTime)
                    _fsm.TransitionTo(MeatState.Burnt);
                break;
        }
    }

    public void PlaceOnGrill()
    {
        _onGrill = true;
        _heatTimer = 0f;
        _fsm.TransitionTo(MeatState.Cooking);
    }

    public void RemoveFromGrill()
    {
        _onGrill = false;
        StopAllVFX();
    }

    public void Flip()
    {
        if (!_side1Done || _flipped) return;
        _flipped = true;
        _heatTimer = 0f;
        transform.Rotate(Vector3.forward, 180f);
    }

    private void OnStartCooking()
    {
        _heatTimer = 0f;
        _steamVFX?.Play();
    }

    private void OnCooked()
    {
        _heatTimer = 0f;
        SetColor(_cookedColor);
        _steamVFX?.Stop();
    }

    private void OnOvercooked()
    {
        _heatTimer = 0f;
        _smokeVFX?.Play();
    }

    private void OnBurnt()
    {
        SetColor(_burntColor);
        _smokeVFX?.Stop();
        _fireVFX?.Play();
    }

    private void StopAllVFX()
    {
        _steamVFX?.Stop();
        _smokeVFX?.Stop();
        _fireVFX?.Stop();
    }

    private void SetColor(Color c)
    {
        if (_renderer != null)
            _renderer.material.color = c;
    }
}
