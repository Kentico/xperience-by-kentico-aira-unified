using CMS.ContentEngine;

using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Admin.Base.Internal;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;

namespace Kentico.Xperience.Aira.Assets;

public interface IAiraAiraAssetService
{
    Task HandleFileUpload(IFormFileCollection files);
}

internal class AiraAssetService : IAiraAiraAssetService
{
    private readonly ContentItemAssetUploaderComponent contentItemAssetUploader;
    private readonly IDataProtectionProvider dataProtectionProvider;

    public AiraAssetService(ContentItemAssetUploaderComponent contentItemAssetUploader, IDataProtectionProvider dataProtectionProvider)
    {
        this.contentItemAssetUploader = contentItemAssetUploader;
        this.dataProtectionProvider = dataProtectionProvider;
    }

    public async Task HandleFileUpload(IFormFileCollection files)
    {
        foreach (var file in files)
        {
            var identifier = Guid.NewGuid();
            var dateIssued = DateTime.Now;
            string fileIdentifier = dataProtectionProvider.GetProtectedValue(
                $"{identifier};{dateIssued}",
                nameof(ContentItemAssetUploaderComponent),
                contentItemAssetUploader.Guid.ToString()
            );

            await contentItemAssetUploader.UploadChunk(new UploadChunkCommandArguments
            {
                FileSize = file.Length,
                FileIdentifier = fileIdentifier,
                FileName = file.Name,
                ChunkId = 1,
                ChunkData = await GetFileBytes(file)
            }, new CancellationToken());

            var result = await contentItemAssetUploader.CompleteUpload(new CompleteUploadCommandArguments
            {
                FileName = file.FileName,
                FileIdentifier = fileIdentifier,
                FileSize = file.Length
            }, new CancellationToken());
        }
    }

    private async Task<byte[]> GetFileBytes(IFormFile formFile)
    {
        using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
