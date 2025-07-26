namespace IdentityServer.Models;

public class TokenViewModel
{
    public string? Subject { get; set; } // شناسه کاربر
    public string? ClientId { get; set; } // شناسه کلاینت (مثلاً web.client)
    public string? SessionId { get; set; } // شناسه Session
    public DateTime CreationTime { get; set; }
    public DateTime? ExpirationTime { get; set; }
    public string? Type { get; set; } // مثلاً refresh_token
    public List<string> Scopes { get; set; } = new(); // Scopeهای این توکن
    public bool IsActive { get; set; }
}

public class TokensListViewModel
{
    public List<TokenViewModel> Tokens { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalCount { get; set; }
    public string? SearchTerm { get; set; }
    public string? TokenType { get; set; }
    public bool? IsActive { get; set; }
    
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public int PageSize => 20;
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}

public class TokenDetailViewModel
{
    public string? Subject { get; set; }
    public string? ClientId { get; set; }
    public string? SessionId { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? ExpirationTime { get; set; }
    public string? Type { get; set; }
    public List<string> Scopes { get; set; } = new();
    public bool IsActive { get; set; }
    public Dictionary<string, string> Claims { get; set; } = new();
    public string? RawToken { get; set; }
}
