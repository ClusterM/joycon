using System;

namespace wtf.cluster.JoyCon.HomeLed;

/// <summary>
/// Home LED dimming step.
/// </summary>
public class HomeLedDimmingStep
{
    private byte ledBrightness = 0x0F;
    /// <summary>
    /// LED brightness, 0-15. 0 = 0%/OFF, 15 = 100%.
    /// </summary>
    public byte LedBrightness
    {
        get => ledBrightness;
        set
        {
            if (value > 0x0F)
            {
                throw new ArgumentOutOfRangeException(nameof(LedBrightness), "Value must be between 0 and 15.");
            }

            ledBrightness = value;
        }
    }

    private byte transitionDuration = 0x0F;
    /// <summary>
    /// Transition duration as multiplier of <see cref="HomeLedDimmingPattern.StepDurationBase"/>, 0-15.
    /// </summary>
    public byte TransitionDuration
    {
        get => transitionDuration;
        set
        {
            if (value > 0x0F)
            {
                throw new ArgumentOutOfRangeException(nameof(TransitionDuration), "Value must be between 0 and 15.");
            }

            transitionDuration = value;
        }
    }

    private byte pauseDuration = 0x0F;
    /// <summary>
    /// Pause between transitions as multiplier of <see cref="HomeLedDimmingPattern.StepDurationBase"/>, 1-15.
    /// </summary>
    public byte PauseDuration
    {
        get => pauseDuration;
        set
        {
            if (value is 0 or > 0x0F)
            {
                throw new ArgumentOutOfRangeException(nameof(PauseDuration), "Value must be between 1 and 15.");
            }

            pauseDuration = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeLedDimmingStep"/> class with default values.
    /// </summary>
    public HomeLedDimmingStep()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeLedDimmingStep"/> class.
    /// </summary>
    /// <param name="ledBrightness">The LED brightness, ranging from 0 to 15. 0 = 0%/OFF, 15 = 100%.</param>
    /// <param name="transitionDuration">The transition duration as a multiplier of <see cref="HomeLedDimmingPattern.StepDurationBase"/>, ranging from 0 to 15.</param>
    /// <param name="pauseDuration">The pause between transitions as a multiplier of <see cref="HomeLedDimmingPattern.StepDurationBase"/>, ranging from 1 to 15.</param>
    public HomeLedDimmingStep(byte ledBrightness, byte transitionDuration, byte pauseDuration)
    {
        LedBrightness = ledBrightness;
        TransitionDuration = transitionDuration;
        PauseDuration = pauseDuration;
    }
}
