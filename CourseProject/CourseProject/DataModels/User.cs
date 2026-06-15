using System.ComponentModel.DataAnnotations;

namespace CourseProject.DataModels;

public partial class User
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    [MaxLength(20)]
    [MinLength(3, ErrorMessage = "Имя пользователя не может быть короче 3 символов")]
    [Required(ErrorMessage = "Обязательно для заполнения")]
    public string Username { get; set; } = null!;

    [MaxLength(16)]
    [MinLength(8, ErrorMessage = "Пароль должен быть длиннее 8 символов")]
    [Required(ErrorMessage = "Обязательно для заполнения")]
    public string Password { get; set; } = null!;

    [MaxLength(20)]
    [Required(ErrorMessage = "Обязательно для заполнения")]
    public string Name { get; set; } = null!;

    [MaxLength(20)]
    [Required(ErrorMessage = "Обязательно для заполнения")]
    public string Surname { get; set; } = null!;

    [MaxLength(25)]
    [Required(ErrorMessage = "Обязательно для заполнения")]
    public string Patronymic { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual Role Role { get; set; } = null!;
}
