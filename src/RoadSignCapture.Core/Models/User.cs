using System;
using System.Collections.Generic;

namespace RoadSignCapture.Core.Models;

public partial class User
{
    public string Email { get; set; } = null!;

    public string? DisplayName { get; set; }

    public int CompanyId { get; set; }

    public DateTime Created { get; set; }

    public DateTime Updated { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Sign> Signs { get; set; } = new List<Sign>();
}
