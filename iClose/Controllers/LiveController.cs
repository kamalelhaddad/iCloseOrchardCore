using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders.Physical;
using Newtonsoft.Json;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.FileStorage;
using OrchardCore.FileStorage.FileSystem;
using OrchardCore.Media;

namespace iClose.Controllers {
    [ApiController]
    [Route("api/editor")]
    [Authorize(AuthenticationSchemes = "Api"), IgnoreAntiforgeryToken, AllowAnonymous]
    public class LiveController : Controller {
        private readonly IContentManager _contentManager;
        private readonly IContentItemDisplayManager _contentItemDisplayManager;
        private readonly IMediaNameNormalizerService _mediaNameNormalizerService;
        private readonly IMediaFileStore _mediaFileStore;
        private readonly string _fileSystemPath = "wwwroot\\admin";
        public LiveController(IContentManager contentManager, IContentItemDisplayManager contentItemDisplayManager,
            IMediaNameNormalizerService mediaNormalizerService, IMediaFileStore mediaFileStore) {
            _contentManager = contentManager;
            _contentItemDisplayManager = contentItemDisplayManager;
            _mediaNameNormalizerService = mediaNormalizerService;
            _mediaFileStore = mediaFileStore;
        }
       
        public IActionResult Index() {
            return Ok("success");
        }

        [HttpPost]
        [Route("newPage")]
        public async Task<IActionResult> NewPage(NewPageDto dto) {
            var fileStore = new FileSystemStore(_fileSystemPath);
            var fileInfo = new FileInfo(Path.Combine(_fileSystemPath, dto.File));
            var fileName = _mediaNameNormalizerService.NormalizeFileName(fileInfo.Name);
            //var stream = new StreamContent(new MemoryStream(fileInfo));
            if (!string.IsNullOrWhiteSpace(dto.StartTemplateUrl)) {
                //var templatePath = Path.Combine(_fileSystemPath, dto.StartTemplateUrl.Replace("demo", ""));
                await CopyFileAsync(dto.StartTemplateUrl.Replace("demo", ""), dto.File);
            }
            else {
                //CreateFileFromStreamAsync(dto.File, fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read), false);
                await CopyFileAsync("new-page-blank-template.html", dto.File);
            }
            
            var mediaFile = fileStore.GetFileInfoAsync(dto.File);
            return Ok(mediaFile);
        }

        public Task CopyFileAsync(string srcPath, string dstPath) {
            var physicalSrcPath = GetPhysicalPath(srcPath);

            if(!System.IO.File.Exists(physicalSrcPath)) {
                throw new FileStoreException($"The file '{srcPath}' does not exist.");
            }

            var physicalDstPath = GetPhysicalPath(dstPath);

            if(System.IO.File.Exists(physicalDstPath) || Directory.Exists(physicalDstPath)) {
                throw new FileStoreException($"Cannot copy file because the destination path '{dstPath}' already exists.");
            }

            System.IO.File.Copy(GetPhysicalPath(srcPath), GetPhysicalPath(dstPath));

            return Task.CompletedTask;
        }

        public string CreateFileFromStreamAsync(string path, Stream inputStream, bool overwrite = false) {
            var physicalPath = GetPhysicalPath(path);

            if(!overwrite && System.IO.File.Exists(physicalPath)) {
                throw new FileStoreException($"Cannot create file '{path}' because it already exists.");
            }

            if(Directory.Exists(physicalPath)) {
                throw new FileStoreException($"Cannot create file '{path}' because it already exists as a directory.");
            }

            // Create directory path if it doesn't exist.
            var physicalDirectoryPath = Path.GetDirectoryName(physicalPath);
            Directory.CreateDirectory(physicalDirectoryPath);

            var fileInfo = new FileInfo(physicalPath);
            using (var outputStream = fileInfo.Create()) {
                inputStream.CopyToAsync(outputStream);
            }

            return path;
        }

        public async Task<dynamic> GetFileInfoAsync(string path) {
            var physicalPath = GetPhysicalPath(path);

            var fileInfo = new PhysicalFileInfo(new FileInfo(physicalPath));

            if(fileInfo.Exists) {
                return new { path, fileInfo };
            }

            return null;
        }

        public string NormalizePath(string path) {
            if(path == null)
                return null;

            return path.Replace('\\', '/').Trim('/', ' ');
        }

        public object CreateFileResult(IFileStoreEntry mediaFile) {
            //_contentTypeProvider.TryGetContentType(mediaFile.Name, out var contentType);

            return new {
                name = mediaFile.Name,
                size = mediaFile.Length,
                lastModify = mediaFile.LastModifiedUtc.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,
                folder = mediaFile.DirectoryPath,
                url = _mediaFileStore.MapPathToPublicUrl(mediaFile.Path),
                mediaPath = mediaFile.Path,
                mime = "application/octet-stream",
                mediaText = String.Empty,
                anchor = new { x = 0.5f, y = 0.5f }
            };
        }

        private string GetPhysicalPath(string path) {
            path = NormalizePath(path);

            var physicalPath = String.IsNullOrEmpty(path) ? _fileSystemPath : Path.Combine(_fileSystemPath, path);

            //// Verify that the resulting path is inside the root file system path.
            //var pathIsAllowed = Path.GetFullPath(physicalPath).StartsWith(_fileSystemPath, StringComparison.OrdinalIgnoreCase);
            //if(!pathIsAllowed) {
            //    throw new FileStoreException($"The path '{path}' resolves to a physical path outside the file system store root.");
            //}

            return physicalPath;
        }
    }

    public class NewPageDto {
        public string StartTemplateUrl { get; set; }
        public string Title { get; set; }
        public string File { get; set; }
        public string Folder { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
