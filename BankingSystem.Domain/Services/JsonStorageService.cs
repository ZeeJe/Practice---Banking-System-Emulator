using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BankingSystem.Domain.Services
{
    public class JsonStorageService
    {
        private readonly JsonSerializerOptions _options;

        public JsonStorageService()
        {
            // Налаштовуємо поведінку JSON під наші потреби
            _options = new JsonSerializerOptions
            {
                WriteIndented = true, // Форматує JSON з відступами (красивий вивід)
                PropertyNameCaseInsensitive = true, // Ігнорує регістр при десеріалізації
                
                // Якщо раптом ми спробуємо кинути доменну модель з циклом напрямую, 
                // цей хак дозволить їй не впасти, а проставити мета-теги $id та $ref.
                // Проте для DTO це не знадобиться, що є найкращою практикою!
                ReferenceHandler = ReferenceHandler.IgnoreCycles 
            };
        }

        // Запис даних у файл
        public void SaveToFile<T>(string filePath, T data)
        {
            string jsonString = JsonSerializer.Serialize(data, _options);
            File.WriteAllText(filePath, jsonString);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[Storage]: Дані успішно серіалізовано та збережено у '{filePath}'");
            Console.ResetColor();
        }

        // Читання даних з файлу
        public T? LoadFromFile<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл {filePath} не знайдено!");
            }

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(jsonString, _options);
        }
    }
}