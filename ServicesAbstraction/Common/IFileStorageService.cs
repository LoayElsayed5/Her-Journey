using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstraction.Common
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string objectName);
        Task DeleteFileAsync(string objectName);
    }
}
