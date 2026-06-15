using System.ComponentModel.DataAnnotations;

namespace CourseProject.DataModels;

public partial class Event
{
    public int EventId { get; set; }
    public int? PhotoId { get; set; }

    [MaxLength(50)]
    [Required(ErrorMessage = "Обязательно для заполнения")]
    public string Title { get; set; } = null!;

    [MaxLength(500)]
    [Required(ErrorMessage = "Обязательно для заполнения")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Обязательно для заполнения")]
    public sbyte AvailableSpace { get; set; }

    [Required(ErrorMessage = "Обязательно для заполнения")]
    [DisplayFormat(DataFormatString = "{0:dd MMMM в HH:mm}", ApplyFormatInEditMode = false)]
    public DateTime EventDate { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
    public DateOnly? UpdatedOn { get; set; }

    public string? Status { get; set; } = null!;

    public virtual Photo? Photo { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
