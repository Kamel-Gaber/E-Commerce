using E_CommerceApi.Dto;

namespace E_CommerceApi.Repository.ImageRepository
{
    public interface IImageRepository
    {
        public string GetFilePath(string productCode);
        public Task<StatusModel> UploadImage(IFormFile formFile, string productCode);
        public Task<StatusModel> MultiUploadImage(IFormFileCollection fileCollection, string productCode);
        public StatusModel GetImage(string productCode);
        public List<string> GetMultiImage(string productCode);
        public MemoryStream Download(string productCode);
        public StatusModel Remove(string productCode);
        public StatusModel MultiRemove(string productCode);

    }
}
