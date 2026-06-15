using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseProject.DataModels;

public partial class Photo
{
    public int PhotoId { get; set; }

    public string Title { get; set; } = null!;

    public string FileName { get; set; } = null!;

    [Column(TypeName = "MEDIUMBLOB")]
    public byte[] ImageData { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
