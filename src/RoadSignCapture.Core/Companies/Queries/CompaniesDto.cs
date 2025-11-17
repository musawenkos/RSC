using RoadSignCapture.Core.Users.Queries;



namespace RoadSignCapture.Core.Companies.Queries
{
    public class CompaniesDto
    {
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? FullAddress { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        public List<UserDto>? Users { get; set; }
    }
}