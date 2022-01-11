namespace TalepWebProject.Entities.Models
{
    public class ResultMessage
    {
        public bool IsSucceed { get; set; }
        public string Message { get; set; }
        public ResultMessage() => IsSucceed = true;
        public ResultMessage(bool isSucceed, string message = null)
        {
            IsSucceed = isSucceed;
            Message = message;
            if (isSucceed && message == null)
                Message = "İşlem başarıyla gerçekleştirildi.";
            else if (!isSucceed && message == null)
                Message = "Bir hata oluştu.";
        }
    }
}
