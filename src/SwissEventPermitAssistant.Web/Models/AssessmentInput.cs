using System.ComponentModel.DataAnnotations;
using SwissEventPermitAssistant.Domain.Profiles;

namespace SwissEventPermitAssistant.Web.Models;

public sealed class AssessmentInput
{
    public string? EventName { get; set; }

    [Required]
    public DateOnly? EventDate { get; set; }

    [Required]
    public int? ExpectedAttendance { get; set; }

    public Commune Commune { get; set; } = Commune.Unknown;

    public VenueKind VenueKind { get; set; } = VenueKind.NotSure;

    public YesNoUnknown IsPublicEvent { get; set; } = YesNoUnknown.Unknown;

    public BeverageMode BeverageMode { get; set; } = BeverageMode.NotSure;

    public FoodMode FoodMode { get; set; } = FoodMode.NotSure;

    public AlcoholMode AlcoholMode { get; set; } = AlcoholMode.NotSure;

    public YesNoUnknown HasAmplifiedMusicOrSound { get; set; } = YesNoUnknown.Unknown;

    public TimeOnly? EventEndTime { get; set; }

    public YesNoUnknown HasTemporaryInstallations { get; set; } = YesNoUnknown.Unknown;

    public YesNoUnknown AffectsTrafficOrParking { get; set; } = YesNoUnknown.Unknown;

    public YesNoUnknown HasProcessionOrRoute { get; set; } = YesNoUnknown.Unknown;

    public YesNoUnknown IsSportCompetitionOnPublicRoad { get; set; } = YesNoUnknown.Unknown;

    public bool NeedsMunicipalMaterialOrDecorations { get; set; }

    public bool NeedsAdvertisingBannerOrPublicPosting { get; set; }

    public YesNoUnknown UsesGasGrillOrHeater { get; set; } = YesNoUnknown.Unknown;

    public YesNoUnknown HasLiabilityInsurance { get; set; } = YesNoUnknown.Unknown;

    public EventProfile ToEventProfile(DateOnly defaultEventDate) =>
        new(
            Commune,
            EventDate ?? defaultEventDate,
            ExpectedAttendance,
            VenueKind,
            IsPublicEvent,
            BeverageMode,
            FoodMode,
            AlcoholMode,
            HasAmplifiedMusicOrSound,
            EventEndTime,
            HasTemporaryInstallations,
            AffectsTrafficOrParking,
            HasProcessionOrRoute,
            IsSportCompetitionOnPublicRoad,
            NeedsMunicipalMaterialOrDecorations,
            NeedsAdvertisingBannerOrPublicPosting,
            UsesGasGrillOrHeater,
            HasLiabilityInsurance,
            EventName);
}
