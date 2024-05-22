using E_CommerceApi.Dto;

namespace E_CommerceApi.Repository.ImageRepository
{
    public class ImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment _environment;

        public ImageRepository(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public string GetFilePath(string productCode)
        {
            return _environment.WebRootPath + "\\Upload\\Product\\" + productCode;
        }
        // upload single Image
        public async Task<StatusModel> UploadImage(IFormFile formFile, string productCode)
        {
            StatusModel statusModel = new StatusModel();
            try
            {
                string FilePath = GetFilePath(productCode);
                if (!System.IO.File.Exists(FilePath))
                {
                    System.IO.Directory.CreateDirectory(FilePath);
                }
                string ImagePath = $"{FilePath}\\{productCode}.png";
                if (System.IO.File.Exists(ImagePath))
                {
                    System.IO.File.Delete(ImagePath);
                }
                using (FileStream stream = System.IO.File.Create(ImagePath))
                {
                    await formFile.CopyToAsync(stream);

                    statusModel.Flag = true;
                    statusModel.Message = "Uploaded Successfully";
                    return statusModel;
                }
            }
            catch (Exception ex)
            {
                statusModel.Flag = false;
                statusModel.Message = ex.Message;
                return statusModel;
            }
        }
        // upload multiple Images
        public async Task<StatusModel> MultiUploadImage(IFormFileCollection fileCollection, string productCode)
        {
            StatusModel statusModel = new StatusModel();
            int passCount = 0, errorCount = 0;
            try
            {
                string FilePath = GetFilePath(productCode);
                if (!System.IO.File.Exists(FilePath))
                {
                    System.IO.Directory.CreateDirectory(FilePath);
                }
                int count = 1;
                foreach (var file in fileCollection)
                {
                    string ImagePath = $"{FilePath}\\{productCode}-0{count++}.png";
                    if (System.IO.File.Exists(ImagePath))
                    {
                        System.IO.File.Delete(ImagePath);
                    }

                    using (FileStream stram = System.IO.File.Create(ImagePath))
                    {
                        await file.CopyToAsync(stram);
                        passCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                errorCount++;
            }
            statusModel.Flag = true;
            statusModel.Message = $"{passCount} Files Uploaded && {errorCount} Files Failed";
            return statusModel;
        }
        // get single Image Path
        public StatusModel GetImage(string productCode)
        {
            StatusModel statusModel = new StatusModel();

            string Imageurl = string.Empty;
            string FilePath = GetFilePath(productCode);
            string ImagePath = $"{FilePath}\\{productCode}.png";
            if (System.IO.File.Exists(ImagePath))
            {
                Imageurl = $"/Upload/Product/{productCode}/{productCode}.png"; // do not forget to add hosturl at first 

                statusModel.Flag = true;
                statusModel.Message = Imageurl;
            }
            else
            {
                statusModel.Flag = false;
                statusModel.Message = "Not Found";
            }
            return statusModel;
        }
        // get multiple Images Path
        public List<String> GetMultiImage(string productCode)
        {
            List<string> Imageurl = new List<string>();
            try
            {
                string FilePath = GetFilePath(productCode);
                if (System.IO.Directory.Exists(FilePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(FilePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        string fileName = fileInfo.Name;
                        string ImagePath = $"{FilePath}\\{fileName}";
                        if (System.IO.File.Exists(ImagePath))
                        {
                            Imageurl.Add(ImagePath);
                        }
                    }
                    return Imageurl;
                }
                else
                {
                    return Imageurl; // Not Found
                }

            }
            catch (Exception ex)
            {
                return Imageurl;
            }
        }
        // download single Image
        public MemoryStream Download(string productCode)
        {
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                string FilePath = GetFilePath(productCode);
                string ImagePath = $"{FilePath}/{productCode}.png";
                if (System.IO.File.Exists(ImagePath))
                {
                    using (FileStream fileStream = new FileStream(ImagePath, FileMode.Open))
                    {
                        fileStream.CopyTo(memoryStream);
                    }
                    memoryStream.Position = 0;
                    return memoryStream;
                }
                else
                {
                    return memoryStream;
                }
            }
            catch (Exception ex)
            {
                return memoryStream;
            }
        }
        // remove single Image
        public StatusModel Remove(string productCode)
        {
            StatusModel statusModel = new StatusModel();
            string FilePath = GetFilePath(productCode);
            string ImagePath = $"{FilePath}/{productCode}.png";
            if (System.IO.File.Exists(ImagePath))
            {
                System.IO.File.Delete(ImagePath);
                statusModel.Flag = true;
                statusModel.Message = "Deleted Successfully";
            }
            else
            {
                statusModel.Flag = false;
                statusModel.Message = "This Image Not Found";
            }
            return statusModel;
        }
        // remove multiple Images
        public StatusModel MultiRemove(string productCode)
        {
            StatusModel statusModel = new StatusModel();
            string FilePath = GetFilePath(productCode);
            if (System.IO.Directory.Exists(FilePath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(FilePath);
                FileInfo[] fileInfos = directoryInfo.GetFiles();
                foreach (FileInfo fileInfo in fileInfos)
                {
                    fileInfo.Delete();
                }
                statusModel.Flag = true;
                statusModel.Message = "Deleted Successfully";
            }
            else
            {
                statusModel.Flag = true;
                statusModel.Message = "This Image Not Found";
            }
            return statusModel;
        }
    }
}
