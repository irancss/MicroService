namespace MarketingService.Domain.Enums;

public enum CampaignStatus
{
    Draft = 0,
    Active = 1,
    Paused = 2,
    Completed = 3,
    Cancelled = 4
}

public enum CampaignType
{
    Email = 0,
    Social = 1,
    Display = 2,
    Search = 3,
    Video = 4,
    Influencer = 5
}

public enum SegmentationType
{
    Demographic = 0,
    Behavioral = 1,
    Geographic = 2,
    Psychographic = 3,
    Transactional = 4
}

public enum LandingPageStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2
}
