/****
Tran Hoang Phat - 49.01.104.107
****/

using LibraryDomain.Entities;
using LibraryDomain.Enums;

public class Reservation
{
    public Guid Id { get; private set; }
    public DateTime ReservedDate { get; private set; }
    public DateTime? ReadyDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public ReservationStatus Status { get; private set; }
    public Guid ReaderId { get; private set; }
    public virtual Reader Reader { get; private set; } = null!;
    public Guid BookItemId { get; private set; }
    public virtual BookItem BookItem { get; private set; } = null!;

    private Reservation() { }

    public Reservation(Reader reader, BookItem bookItem)
    {
        Id = Guid.NewGuid();
        ReaderId = reader.Id;
        Reader = reader;
        BookItemId = bookItem.Id;
        BookItem = bookItem;
        ReservedDate = DateTime.Now;

        Status = (bookItem.Status == BookStatus.Available)
                 ? ReservationStatus.Ready
                 : ReservationStatus.Pending;

        if (Status == ReservationStatus.Ready)
        {
            MarkAsReady(3); 
        }
    }

    public void MarkAsCompleted()
    {
        Status = ReservationStatus.Completed;
    }

    public void NotifyAvailable()
    {
        Status = ReservationStatus.Ready;

        ExpiryDate = DateTime.Now.AddDays(3);
    }

    public void MarkAsReady(int daysToHold)
    {
        Status = ReservationStatus.Ready;
        ReadyDate = DateTime.Now;
        ExpiryDate = ReadyDate.Value.AddDays(daysToHold);

        BookItem.MarkAsReserved();
    }

    public void Cancel()
    {
        Status = ReservationStatus.Canceled; 
        BookItem.MarkAsAvailable();
    }

    public void MarkAsExpired()
    {
        Status = ReservationStatus.Expired;
        BookItem.MarkAsAvailable();
    }

    public void Complete()
    {
        Status = ReservationStatus.Completed;
    }
}