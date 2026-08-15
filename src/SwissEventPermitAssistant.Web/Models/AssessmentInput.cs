using System.ComponentModel.DataAnnotations;
using SwissEventPermitAssistant.Domain.Profiles;

namespace SwissEventPermitAssistant.Web.Models;

public sealed class AssessmentInput
{
    public string? EventName { get; set; }

    [Required]
    public DateOnly? EventDate { get; set; }

    public int? ExpectedAttendance { get; set; }

    public Commune Commune { get; set; } = Commune.VilleDeFribourg;

    public VenueKind VenueKind { get; set; } = VenueKind.NotSure;

    public YesNoUnknown IsPublicEvent { get; set; } = YesNoUnknown.Yes;

    public BeverageMode BeverageMode { get; set; } = BeverageMode.NoBeverages;

    public FoodMode FoodMode { get; set; } = FoodMode.NoFood;

    public AlcoholMode AlcoholMode { get; set; } = AlcoholMode.NoAlcohol;

    public YesNoUnknown HasAmplifiedMusicOrSound { get; set; } = YesNoUnknown.No;

    public TimeOnly? EventEndTime { get; set; }

    public YesNoUnknown HasTemporaryInstallations { get; set; } = YesNoUnknown.No;

    public YesNoUnknown AffectsTrafficOrParking { get; set; } = YesNoUnknown.No;

    public YesNoUnknown HasProcessionOrRoute { get; set; } = YesNoUnknown.No;

    public YesNoUnknown IsSportCompetitionOnPublicRoad { get; set; } = YesNoUnknown.No;

    public bool NeedsMunicipalMaterialOrDecorations { get; set; }

    public bool NeedsAdvertisingBannerOrPublicPosting { get; set; }

    public YesNoUnknown UsesGasGrillOrHeater { get; set; } = YesNoUnknown.No;

    public YesNoUnknown HasLiabilityInsurance { get; set; } = YesNoUnknown.Unknown;

    public YesNoUnknown PrivateVenueOwnerAuthorizationAvailable { get; set; } = YesNoUnknown.Unknown;

    public EventProfile ToEventProfile() =>
        new(
            Commune,
            EventDate ?? DateOnly.FromDateTime(DateTime.Today),
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
            PrivateVenueOwnerAuthorizationAvailable,
            EventName);
}
