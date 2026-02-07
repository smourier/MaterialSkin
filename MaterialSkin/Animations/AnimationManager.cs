namespace MaterialSkin.Animations;

internal class AnimationManager
{
    public event EventHandler? OnAnimationFinished;
    public event EventHandler? OnAnimationProgress;

    private const double _minValue = 0;
    private const double _maxValue = 1;

    private readonly List<double> _animationProgresses;
    private readonly List<Point> _animationSources;
    private readonly List<AnimationDirection> _animationDirections;
    private readonly List<object[]> _animationDatas;
    private readonly Timer _animationTimer = new() { Interval = 5, Enabled = false };

    public AnimationManager(bool singular = true)
    {
        _animationProgresses = [];
        _animationSources = [];
        _animationDirections = [];
        _animationDatas = [];

        Increment = 0.03;
        SecondaryIncrement = 0.03;
        AnimationType = AnimationType.Linear;
        InterruptAnimation = true;
        Singular = singular;

        if (Singular)
        {
            _animationProgresses.Add(0);
            _animationSources.Add(new Point(0, 0));
            _animationDirections.Add(AnimationDirection.In);
        }

        _animationTimer.Tick += AnimationTimerOnTick;
    }

    public bool InterruptAnimation { get; set; }
    public double Increment { get; set; }
    public double SecondaryIncrement { get; set; }
    public AnimationType AnimationType { get; set; }
    public bool Singular { get; set; }

    private void AnimationTimerOnTick(object? sender, EventArgs eventArgs)
    {
        for (var i = 0; i < _animationProgresses.Count; i++)
        {
            UpdateProgress(i);

            if (!Singular)
            {
                if (_animationDirections[i] == AnimationDirection.InOutIn && _animationProgresses[i] == _maxValue)
                {
                    _animationDirections[i] = AnimationDirection.InOutOut;
                }
                else if (_animationDirections[i] == AnimationDirection.InOutRepeatingIn && _animationProgresses[i] == _minValue)
                {
                    _animationDirections[i] = AnimationDirection.InOutRepeatingOut;
                }
                else if (_animationDirections[i] == AnimationDirection.InOutRepeatingOut && _animationProgresses[i] == _minValue)
                {
                    _animationDirections[i] = AnimationDirection.InOutRepeatingIn;
                }
                else if (
                    (_animationDirections[i] == AnimationDirection.In && _animationProgresses[i] == _maxValue) ||
                    (_animationDirections[i] == AnimationDirection.Out && _animationProgresses[i] == _minValue) ||
                    (_animationDirections[i] == AnimationDirection.InOutOut && _animationProgresses[i] == _minValue))
                {
                    _animationProgresses.RemoveAt(i);
                    _animationSources.RemoveAt(i);
                    _animationDirections.RemoveAt(i);
                    _animationDatas.RemoveAt(i);
                }
            }
            else
            {
                if (_animationDirections[i] == AnimationDirection.InOutIn && _animationProgresses[i] == _maxValue)
                {
                    _animationDirections[i] = AnimationDirection.InOutOut;
                }
                else if (_animationDirections[i] == AnimationDirection.InOutRepeatingIn && _animationProgresses[i] == _maxValue)
                {
                    _animationDirections[i] = AnimationDirection.InOutRepeatingOut;
                }
                else if (_animationDirections[i] == AnimationDirection.InOutRepeatingOut && _animationProgresses[i] == _minValue)
                {
                    _animationDirections[i] = AnimationDirection.InOutRepeatingIn;
                }
            }
        }

        OnAnimationProgress?.Invoke(this, EventArgs.Empty);
    }

    public bool IsAnimating() => _animationTimer.Enabled;
    public void StartNewAnimation(AnimationDirection animationDirection, object[]? data = null) => StartNewAnimation(animationDirection, new Point(0, 0), data);
    public void StartNewAnimation(AnimationDirection animationDirection, Point animationSource, object[]? data = null)
    {
        if (!IsAnimating() || InterruptAnimation)
        {
            if (Singular && _animationDirections.Count > 0)
            {
                _animationDirections[0] = animationDirection;
            }
            else
            {
                _animationDirections.Add(animationDirection);
            }

            if (Singular && _animationSources.Count > 0)
            {
                _animationSources[0] = animationSource;
            }
            else
            {
                _animationSources.Add(animationSource);
            }

            if (!(Singular && _animationProgresses.Count > 0))
            {
                switch (_animationDirections[^1])
                {
                    case AnimationDirection.InOutRepeatingIn:
                    case AnimationDirection.InOutIn:
                    case AnimationDirection.In:
                        _animationProgresses.Add(_minValue);
                        break;

                    case AnimationDirection.InOutRepeatingOut:
                    case AnimationDirection.InOutOut:
                    case AnimationDirection.Out:
                        _animationProgresses.Add(_maxValue);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            if (Singular && _animationDatas.Count > 0)
            {
                _animationDatas[0] = data ?? [];
            }
            else
            {
                _animationDatas.Add(data ?? []);
            }
        }

        _animationTimer.Start();
    }

    public void UpdateProgress(int index)
    {
        switch (_animationDirections[index])
        {
            case AnimationDirection.InOutRepeatingIn:
            case AnimationDirection.InOutIn:
            case AnimationDirection.In:
                IncrementProgress(index);
                break;

            case AnimationDirection.InOutRepeatingOut:
            case AnimationDirection.InOutOut:
            case AnimationDirection.Out:
                DecrementProgress(index);
                break;

            default:
                throw new NotSupportedException();
        }
    }

    private void IncrementProgress(int index)
    {
        _animationProgresses[index] += Increment;
        if (_animationProgresses[index] > _maxValue)
        {
            _animationProgresses[index] = _maxValue;

            for (var i = 0; i < GetAnimationCount(); i++)
            {
                if (_animationDirections[i] == AnimationDirection.InOutIn)
                {
                    return;
                }

                if (_animationDirections[i] == AnimationDirection.InOutRepeatingIn)
                {
                    return;
                }

                if (_animationDirections[i] == AnimationDirection.InOutRepeatingOut)
                {
                    return;
                }

                if (_animationDirections[i] == AnimationDirection.InOutOut && _animationProgresses[i] != _maxValue)
                {
                    return;
                }

                if (_animationDirections[i] == AnimationDirection.In && _animationProgresses[i] != _maxValue)
                {
                    return;
                }
            }

            _animationTimer.Stop();
            OnAnimationFinished?.Invoke(this, EventArgs.Empty);
        }
    }

    private void DecrementProgress(int index)
    {
        _animationProgresses[index] -= (_animationDirections[index] == AnimationDirection.InOutOut || _animationDirections[index] == AnimationDirection.InOutRepeatingOut) ? SecondaryIncrement : Increment;
        if (_animationProgresses[index] < _minValue)
        {
            _animationProgresses[index] = _minValue;

            for (var i = 0; i < GetAnimationCount(); i++)
            {
                if (_animationDirections[i] == AnimationDirection.InOutIn)
                {
                    return;
                }

                if (_animationDirections[i] == AnimationDirection.InOutRepeatingIn)
                {
                    return;
                }

                if (_animationDirections[i] == AnimationDirection.InOutRepeatingOut)
                {
                    return;
                }

                if (_animationDirections[i] == AnimationDirection.InOutOut && _animationProgresses[i] != _minValue)
                {
                    return;
                }

                if (_animationDirections[i] == AnimationDirection.Out && _animationProgresses[i] != _minValue)
                {
                    return;
                }
            }

            _animationTimer.Stop();
            OnAnimationFinished?.Invoke(this, EventArgs.Empty);
        }
    }

    public double GetProgress()
    {
        if (!Singular)
            throw new InvalidOperationException();

        if (_animationProgresses.Count == 0)
            throw new InvalidOperationException();

        return GetProgress(0);
    }

    public double GetProgress(int index)
    {
        if (!(index < GetAnimationCount()))
            throw new IndexOutOfRangeException(nameof(index));

        return AnimationType switch
        {
            AnimationType.Linear => AnimationLinear.CalculateProgress(_animationProgresses[index]),
            AnimationType.EaseInOut => AnimationEaseInOut.CalculateProgress(_animationProgresses[index]),
            AnimationType.EaseOut => AnimationEaseOut.CalculateProgress(_animationProgresses[index]),
            AnimationType.CustomQuadratic => AnimationCustomQuadratic.CalculateProgress(_animationProgresses[index]),
            _ => throw new NotSupportedException()
        };
    }

    public Point GetSource(int index)
    {
        if (!(index < GetAnimationCount()))
            throw new IndexOutOfRangeException();

        return _animationSources[index];
    }

    public Point GetSource()
    {
        if (!Singular)
            throw new InvalidOperationException();

        if (_animationSources.Count == 0)
            throw new InvalidOperationException();

        return _animationSources[0];
    }

    public AnimationDirection GetDirection()
    {
        if (!Singular)
            throw new InvalidOperationException();

        if (_animationDirections.Count == 0)
            throw new InvalidOperationException();

        return _animationDirections[0];
    }

    public AnimationDirection GetDirection(int index)
    {
        if (!(index < _animationDirections.Count))
            throw new IndexOutOfRangeException();

        return _animationDirections[index];
    }

    public object[] GetData()
    {
        if (!Singular)
            throw new InvalidOperationException();

        if (_animationDatas.Count == 0)
            throw new InvalidOperationException();

        return _animationDatas[0];
    }

    public object[] GetData(int index)
    {
        if (!(index < _animationDatas.Count))
            throw new IndexOutOfRangeException();

        return _animationDatas[index];
    }

    public int GetAnimationCount() => _animationProgresses.Count;
    public void SetProgress(double progress)
    {
        if (!Singular)
            throw new InvalidOperationException();

        if (_animationProgresses.Count == 0)
            throw new InvalidOperationException();

        _animationProgresses[0] = progress;
    }

    public void SetDirection(AnimationDirection direction)
    {
        if (!Singular)
            throw new InvalidOperationException();

        if (_animationProgresses.Count == 0)
            throw new InvalidOperationException();

        _animationDirections[0] = direction;
    }

    public void SetData(object[] data)
    {
        if (!Singular)
            throw new InvalidOperationException();

        if (_animationDatas.Count == 0)
            throw new InvalidOperationException();

        _animationDatas[0] = data;
    }
}