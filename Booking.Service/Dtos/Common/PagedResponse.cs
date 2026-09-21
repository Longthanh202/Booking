namespace Booking.Service.Dtos.Common;

public abstract class PagedResponse<TItem>
{
    public List<TItem> Data { get; set; } = new();
    public int TotalRow { get; set; }
    public int TotalPage { get; set; }
}
