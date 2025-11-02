using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Signs.Commands
{
    public class SignHandler
    {
        private readonly IProjectService? _projectService;
        private readonly IUserService? _userService;
        private ISignService? _signService;
        private IPhotoService? _photoService;
        private IAudiTrailService? _audiTrailService;
        private ICacheService? _cache;
        private ILogger<SignHandler> _logger;

        public SignHandler(IProjectService projectService, IUserService userService, ISignService signService, IPhotoService photoService,IAudiTrailService audiTrailService , ICacheService cache, ILogger<SignHandler> logger)
        {
            _projectService = projectService;
            _userService = userService;
            _signService = signService;
            _photoService = photoService;
            _audiTrailService = audiTrailService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Result> CreateHandlerAsync(SignCommand command,string webRootPath)
        {
            try
            {
                if (command == null) return Result.Failure("Sign data is required");
                command.Sign!.CreatedBy = command.Sign.CreatedByUserId!;
                command.Sign!.UpdatedBy = command.Sign.CreatedByUserId!;

                await _signService!.AddAsync(command.Sign!);
                await _signService!.SaveChangesAsync();

                var signId = command.Sign!.Id;
                var webRoot = webRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string basePath = Path.Combine(webRoot, "uploads", "signs",signId.ToString());

                

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                    _logger.LogInformation("Created directory at: {Path}", basePath);
                }


                async Task<string?> SaveFileAsync(IFormFile? file, string ext)
                {
                    try {
                        if (file == null || file.Length == 0) 
                        {
                            _logger.LogInformation("The is no file {file}", file!.FileName);
                            return null; 
                        }

                        var fileName = $"{signId}_{DateTime.UtcNow:yyyyMMddHHmmss}.{ext}";
                        var filePath = Path.Combine(basePath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        return Path.Combine("uploads", "signs", signId.ToString(),fileName);
                    }
                    catch(Exception ex) 
                    {
                        _logger.LogError(ex, "Error while uploading");
                        return "Exception";
                    }
                }
                var pdfPath = await SaveFileAsync(command.PdfFile, "pdf");
                if (pdfPath != null)
                {
                    var photoPDF = new Photo
                    {
                        SignId = signId,
                        FilePath = pdfPath!,
                        Description = $"Sign PDF for {command.Sign.SignIdNumber}",
                        CapturedAt = DateTime.UtcNow,
                        CapturedBy = command.Sign.CreatedByUserId!
                    };

                    await _photoService!.AddAsync(photoPDF);
                    await _photoService.SaveChangesAsync();
                }
                else
                {
                    _logger.LogInformation("Sign PDF file was unable to be uploaded {SignIdNum}", command.Sign.SignIdNumber);
                }


                    var jpgPath = await SaveFileAsync(command.JpgFile, "jpg");
                if (jpgPath != null)
                {
                    var photoJPG = new Photo
                    {
                        Id = Guid.NewGuid(),
                        SignId = signId,
                        FilePath = jpgPath!,
                        Description = $"Sign photo for {command.Sign.SignIdNumber}",
                        CapturedAt = DateTime.UtcNow,
                        CapturedBy = command.Sign.CreatedByUserId!
                    };

                    await _photoService!.AddAsync(photoJPG);
                    await _photoService.SaveChangesAsync();
                }
                else
                {
                    _logger.LogInformation("Sign JPG file was unable to be uploaded {SignIdNum}", command.Sign.SignIdNumber);
                }
                var sgxPath = await SaveFileAsync(command.SgxFile, "sgx");
                if (sgxPath != null)
                {
                    var photoSGX = new Photo
                    {
                        Id = Guid.NewGuid(),
                        SignId = signId,
                        FilePath = sgxPath!,
                        Description = $"Sign design file for {command.Sign.SignIdNumber}",
                        CapturedAt = DateTime.UtcNow,
                        CapturedBy = command.Sign.CreatedByUserId!
                    };

                    await _photoService!.AddAsync(photoSGX);
                    await _photoService.SaveChangesAsync();
                }
                else
                {
                    _logger.LogInformation("{SignIdNum}: Sign SGX file was unable to be uploaded ", command.Sign.SignIdNumber);
                }

                var audit = new AuditTrail
                {
                    SignId = signId,
                    ActionType = command.Sign.Action,
                    Description = $"Sign {command.Sign.SignIdNumber} added with media files.",
                    PerformedBy = command.Sign.CreatedByUserId! ?? "System",
                    Timestamp = DateTime.UtcNow,
                    DeviceUsed = "Web Console",
                    LocationContext = $"{command.Sign.Latitude},{command.Sign.Longitude}"
                };

                await _audiTrailService!.AddAsync(audit);
                await _audiTrailService.SaveChangesAsync();

                _logger.LogInformation("Sign created successfully: {SignId}", command.Sign.SignIdNumber);

                var cacheKey = $"signs:{command.Sign.ProjectId}";
                await _cache!.RemoveAsync(cacheKey);
                return Result.SuccessResult(command.Sign);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while creating sign");
                return Result.Failure("An error occured while creating the sign: " + ex.Message);
            
            }

        }



    }
    public record Result(bool Success, Sign? Sign = null, string? Error = null)
    {
        public static Result SuccessResult(Sign? s = null)
        {
            return new(true, Sign: s);
        }

        public static Result Failure(string error) => new(false, Error: error);
    }
}
