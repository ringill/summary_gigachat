public static class MainClass
{
    // Настройки API
    const string API_URL = "https://gigachat.devices.sberbank.ru/api/v1/chat/completions";
    const string API_KEY = "eyJjdHkiOiJqd3QiLCJlbmMiOiJBMjU2Q0JDLUhTNTEyIiwiYWxnIjoiUlNBLU9BRVAtMjU2In0.vgt1ESO24YPAhwymFg-_sCd8tCUfdYSYhjpJGXgjwnd5Q2le-qQwY8zCWwJ1lh3ovEvXV_5Dv4Uzup_gs6lPOY4kHziVCAhUJTxc_kWGGw4vbG6Qgkj-QHC1TDDz2ke3cpKQGYgaeBe81hQ3CxzZ7WHhU5AfimSobhxMNpgza0tN4NJD8NeCjZE0NTKFPpmoECbdnipufyES1JR1Wd_dtZqJs09r5ZNPl0yg0YsYuE68ux1eBFxb-YEKXysMS_qwbmxfdDIU1ShY8fuMhxzyiYdltCIHeGGuJKn6UC4A_JBhcPP0A2zjLRIIFhEy3sItvlQbwbiQ8WFbbQRUdGrFjA.9RIU4fspKMRmcCIJqmQF0Q.qptd0gsn-lLQEbypcrrhO9zVKS4x_7tbgi92XWpUsutsZkdXxmGFxvpj05jjiuOLG5bpj51SYWybpqcoPpabpHI4Pj_tAHrTjnjVFiV-jP9uxYKE6eZvkRS4_Ox0jXqxbueFXIbQ3YosT2BOQ1wlvD4nLRWHskdNLBvhKcaboyG0v1nAcZQOE0X5ds1AwmYVFovb9pYxFC38GTDfEZQKQ4Mq0D7qKLcnPAnFOMAZfMPgqosOPMgSVTa2RvsQsbp7JBIIqM15MBd5-ml7vvUOoXNiHWUW6dQW35E_T6foUfUAbr62q_ZO_FSzLXYGhIoIaPxcboVh6jFgPvjg28ux9riKdzWCcO3HEPMOvpAzH5KEmsbd0A58URSQhsVoV3YLQjUCPhRvdKCjrqyicf-2M6k4lcqN1-KiwVJV4Qih-i8aP4hvN7d6ImvhsLulq4UMx5JiJ7BOkfSWGnKBjpCWv7m63iA_KAZPDH9WYtZaCc6paMzEkjqvfGYm18l54CGc2yPTP8L_E7EM_dwYpZxm0o-zQDiKjIoxhIQmF7DrE51ASsKSHFF77Yh8zOZAzE6UwttPsqCuf4bmGzkf_7Srte-0YBqrJ1RoiY707WtpjPxfCEpIBC-lafiTm5AyYhdKqZfZ_QIIKWdJYnwW9LsMGarqaOKdGUUPoqZ1iRjd3nY7Mj5HasEhYK9WwhT0ee1LyEihbxzoJ6FxEe2ijY8Lyz_Svg4u3eRMdrp-dvFog5Y.mD50lgr5w3wUFcCdwDH8RWUK1d5-41BeL40TyyWo7k8";

    public static void Main()
    {
        var documentationGenerator = new DocumentationGenerator(API_URL, API_KEY);
        // Обработка всех файлов в проекте
        var files = Directory.GetFiles("../../../../", "*Tests.cs", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            documentationGenerator.ProcessFile(file).GetAwaiter().GetResult();
        }
    }
}
