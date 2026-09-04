namespace SwissEventPermitAssistant.Domain.Profiles;

public enum Commune
{
    VilleDeFribourg,
    Other,
    Unknown
}

public enum VenueKind
{
    PublicSpace,
    PrivateVenue,
    NotSure
}

public enum YesNoUnknown
{
    No,
    Yes,
    Unknown
}

public enum BeverageMode
{
    NoBeverages,
    BeveragesSold,
    BeveragesFree,
    NotSure
}

public enum FoodMode
{
    NoFood,
    CookedFoodSold,
    OtherFoodSoldOrUnsure,
    FoodFree,
    NotSure
}

public enum AlcoholMode
{
    NoAlcohol,
    AlcoholSold,
    AlcoholFree,
    NotSure
}

public sealed record EventProfile(
    Commune Commune,
    DateOnly EventDate,
    int? ExpectedAttendance,
    VenueKind VenueKind,
    YesNoUnknown IsPublicEvent = YesNoUnknown.Unknown,
    BeverageMode BeverageMode = BeverageMode.NotSure,
    FoodMode FoodMode = FoodMode.NotSure,
    AlcoholMode AlcoholMode = AlcoholMode.NotSure,
    YesNoUnknown HasAmplifiedMusicOrSound = YesNoUnknown.Unknown,
    TimeOnly? EventEndTime = null,
    YesNoUnknown HasTemporaryInstallations = YesNoUnknown.Unknown,
    YesNoUnknown AffectsTrafficOrParking = YesNoUnknown.Unknown,
    YesNoUnknown HasProcessionOrRoute = YesNoUnknown.Unknown,
    YesNoUnknown IsSportCompetitionOnPublicRoad = YesNoUnknown.Unknown,
    bool NeedsMunicipalMaterialOrDecorations = false,
    bool NeedsAdvertisingBannerOrPublicPosting = false,
    YesNoUnknown UsesGasGrillOrHeater = YesNoUnknown.Unknown,
    YesNoUnknown HasLiabilityInsurance = YesNoUnknown.Unknown,
    string? EventName = null);
