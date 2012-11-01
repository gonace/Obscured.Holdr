namespace Obscured.Holdr.BLL
{
    public interface IEncryptionService
    {
        byte[] ConvertToBinaryWithoutKey(string data);
        byte[] ConvertToBinaryWithKey(string data, string hashKey);
        string GetMd5HashWithKey(string data, string hashKey);
        string GetMd5HashWithoutKey(string data);
    }
}