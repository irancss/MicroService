
using ProductService.Domain.Common;

public class ProductComment : AuditableEntity
{
    public string ProductId { get; set; }
    public string UserId { get; set; }
    public string Content { get; set; }

    public CommentStatus Status { get; set; }
    public StatusCommentUser StatusCommentUser { get; set; }

}

public enum CommentStatus
{
    Pending,
    Approved,
    Rejected
}
public enum StatusCommentUser
{
    Buyed,
    NotBuyed
}