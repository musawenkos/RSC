using System;
using System.Collections.Generic;

namespace RoadSignCapture.Core.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? FullAddress { get; set; }

    public string? ContactNumber { get; set; }

    public DateTime Created { get; set; }

    public DateTime Updated { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
