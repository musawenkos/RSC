using RoadSignCapture.Core.Companies.Commands;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Projects.Commands
{
    public class ProjectHandler
    {
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        public ProjectHandler(IProjectService projectService,IUserService userService) 
        { 
            _projectService = projectService;
            _userService = userService;
        }
        public async Task<Result> CreatHandleAsync(ProjectCommand command)
        {
            List<User?> users = [];
            var project = command.ProjectName;
            if (project == null)
                return Result.Failure("Project data is null.");

            var listUsers = command.SelectedUsers;

            foreach (var user in listUsers)
            {
                var u = _userService.GetUserBy(user);
                if (u == null) continue;
                users.Add(u.Result);
            }

            var newProject = new Core.Models.Project
            {
                ProjectName = project,
                Description = command.ProjectDescription,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                Users = users!,
            };
            await _projectService.AddAsync(newProject);
            await _userService.SaveChangesAsync();
            return Result.SuccessResult();
            
        }


    }

    public record Result(bool Success, Project? Project = null, string? Error = null)
    {
        public static Result SuccessResult(Project? p = null)
        {
            return new(true, Project: p);
        }

        public static Result Failure(string error) => new(false, Error: error);
    }
}
