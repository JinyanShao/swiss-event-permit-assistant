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
    YesNoUnknown IsPublicEvent = YesNoUnknown.Yes,
    BeverageMode BeverageMode = BeverageMode.NoBeverages,
    FoodMode FoodMode = FoodMode.NoFood,
    AlcoholMode AlcoholMode = AlcoholMode.NoAlcohol,
    YesNoUnknown HasAmplifiedMusicOrSound = YesNoUnknown.No,
    TimeOnly? EventEndTime = null,
    YesNoUnknown HasTemporaryInstallations = YesNoUnknown.No,
    YesNoUnknown AffectsTrafficOrParking = YesNoUnknown.No,
    YesNoUnknown HasProcessionOrRoute = YesNoUnknown.No,
    YesNoUnknown IsSportCompetitionOnPublicRoad = YesNoUnknown.No,
    bool NeedsMunicipalMaterialOrDecorations = false,
    bool NeedsAdvertisingBannerOrPublicPosting = false,
    YesNoUnknown UsesGasGrillOrHeater = YesNoUnknown.No,
    YesNoUnknown HasLiabilityInsurance = YesNoUnknown.Yes,
    string? EventName = null);
