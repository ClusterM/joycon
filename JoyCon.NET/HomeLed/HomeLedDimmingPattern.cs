using System;
using System.Collections.Generic;
using System.Text;

namespace wtf.cluster.JoyCon.HomeLed;

/// <summary>
/// Data for setting the Home LED dimming pattern.
/// </summary>
public class HomeLedDimmingPattern
{
    /// <summary>
    /// Number of full dimming cycles.
    /// </summary>

    private byte stepDurationBase;
    /// <summary>
    /// Global dimming step duration base, 0-15. 8ms - 175ms. ~10ms per value. 0 = 0ms/OFF.
    /// </summary>
    public byte StepDurationBase
    {
        get => stepDurationBase;
        set
        {
            if (value > 0x0F)
            {
                throw new ArgumentOutOfRangeException(nameof(StepDurationBase), "Step duration base must be between 0 and 15.");
            }

            stepDurationBase = value;
        }
    }

    private byte startLedBrightness;
    /// <summary>
    /// Start LED brightness, 0-15. 0 = 0%/OFF, 15 = 100%.
    /// </summary>
    public byte StartLedBrightness
    {
        get => startLedBrightness;
        set
        {
            if (value > 0x0F)
            {
                throw new ArgumentOutOfRangeException(nameof(StartLedBrightness), "Start LED intensity must be between 0 and 15.");
            }

            startLedBrightness = value;
        }
    }

    private byte fullCyclesNumber;
    /// <summary>
    /// Number of full dimming cycles, 1-15. 0 = forever.
    /// </summary>
    public byte FullCyclesNumber
    {
        get => fullCyclesNumber; set
        {
            if (value > 0x0F)
            {
                throw new ArgumentOutOfRangeException(nameof(FullCyclesNumber), "Full cycles number must be between 0 and 15.");
            }
            fullCyclesNumber = value;
        }
    }

    /// <summary>
    /// A special case when only one cycle is needed and then LED brightness returns to the start value. If set to true, only first item of <see cref="HomeLedDimmingSteps"/> will be used and <see cref="FullCyclesNumber"/> will be ignored.
    /// </summary>
    public bool OnlyOneCycleAndReturnToStart { get; set; }

    /// <summary>
    /// List of dimming steps as <see cref="HomeLedDimmingStep"/> objects.
    /// </summary>
    public List<HomeLedDimmingStep> HomeLedDimmingSteps { get; set; } = new();

    internal byte[] ToBytes()
    {
        if (HomeLedDimmingSteps == null)
        {
            throw new ArgumentNullException("Cannot have null dimming steps.");
        }
        if (HomeLedDimmingSteps.Count == 0 || HomeLedDimmingSteps.Count > 15)
        {
            throw new ArgumentOutOfRangeException("Dimming steps must be between 1 and 15.");
        }
        var data = new byte[25];
        data[0] |= (byte)(stepDurationBase & 0x0F);
        data[1] |= (byte)(startLedBrightness << 4);
        if (!OnlyOneCycleAndReturnToStart)
        {
            data[0] |= (byte)(HomeLedDimmingSteps.Count << 4);
            data[1] |= (byte)(fullCyclesNumber & 0x0F);
        }

        for (int i = 0; i < HomeLedDimmingSteps.Count; i++)
        {
            var step = HomeLedDimmingSteps[i];
            data[(i / 2) * 3 + 2] |= (byte)((step.LedBrightness & 0x0F) << (i % 2 == 0 ? 4 : 0));
            data[(i / 2) * 3 + 3 + (i % 2)] |= (byte)((step.TransitionDuration & 0x0F) << 4);
            data[(i / 2) * 3 + 3 + (i % 2)] |= (byte)((step.PauseDuration & 0x0F));
        }

        return data;
    }
}
