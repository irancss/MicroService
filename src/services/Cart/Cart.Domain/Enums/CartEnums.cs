namespace Cart.Domain.Enums;

public enum CartType
{
    Active = 1,
    NextPurchase = 2
}

public enum CartOperationType
{
    AddItem = 1,
    RemoveItem = 2,
    UpdateQuantity = 3,
    MoveItem = 4,
    ClearCart = 5,
    MergeCart = 6
}

public enum AbandonmentAction
{
    DoNothing = 1,
    MoveToNextPurchase = 2,
    SendEmail = 3,
    SendSms = 4,
    SendEmailAndSms = 5
}

public enum NotificationChannel
{
    Email = 1,
    Sms = 2,
    Push = 3
}

public enum CartEventType
{
    ItemAdded = 1,
    ItemRemoved = 2,
    ItemMoved = 3,
    CartAbandoned = 4,
    CartRecovered = 5,
    NextPurchaseActivated = 6,
    CartMerged = 7,
    CartCleared = 8
}
